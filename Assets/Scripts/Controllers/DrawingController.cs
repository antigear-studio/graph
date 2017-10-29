using MaterialUI;
using System;
using UnityEngine;
using System.Collections.Generic;


namespace Antigear.Graph {
    /// <summary>
    /// Manages drawing to a graph.
    /// </summary>
    public class DrawingController : MonoBehaviour, IPaperDelegate, 
    IToolbarViewDelegate, ISelectionHandlerDelegate {
        public IDrawingControllerDelegate controllerDelegate;

        // Outlets.
        public DrawingView drawingView;
        public Paper paper;

        Graph editingGraph;

        // Bookkeeping.
        Tool dragTool;
        public Drawable selectedDrawable;
        public DrawableView selectedDrawableView;
        public Drawable editingDrawable;
        public DrawableView editingDrawableView;

        // Handlers for each tool.
        readonly Dictionary<Tool, ToolHandler> toolHandlers = 
            new Dictionary<Tool, ToolHandler> {
                { Tool.StraightLine, new StraightLineToolHandler() },
                { Tool.Zoom, new ZoomToolHandler() },
                { Tool.Pan, new PanToolHandler() }
            };

        // Handlers for each object type.
        readonly Dictionary<Type, SelectionHandler> selectionHandlers = 
            new Dictionary<Type, SelectionHandler> {
                { typeof(StraightLine), new StraightLineSelectionHandler() }
            };

        /// <summary>
        /// Opens the given graph for editing. Providing a rect transform will
        /// change the animation to the graph paper expanding from there.
        /// </summary>
        /// <param name="graph">Graph.</param>
        /// <param name="animated">If set to <c>true</c> animated.</param>
        /// <param name="tile">Tile.</param>
        /// <param name="callback">Callback.</param>
        public void OpenGraph(Graph graph, bool animated, 
            RectTransform tile = null, Action callback = null) {
            editingGraph = graph;

            foreach (ToolHandler handler in toolHandlers.Values) {
                handler.SetupToolHandler(graph, drawingView);
            }

            foreach (SelectionHandler handler in selectionHandlers.Values) {
                handler.SetupSelectionHandler(graph, drawingView, this);
            }

            drawingView.gameObject.SetActive(true);
            drawingView.toolbarView.SetToolbarVisibility(true, animated);
            drawingView.SetExpansion(true, true, tile, callback);
            drawingView.toolbarView.ChangeTool(graph.activeTool, false);
            drawingView.LoadContent(graph.content, graph.preferences);
            SetBackgroundColor(graph.preferences.backgroundColor, false);
        }

        /// <summary>
        /// Cleans up the drawing interface to prepare to edit another graph.
        /// </summary>
        /// <param name="animated">Whether animation should be used.</param>
        /// <param name="tile">Tile to shrink paper back to. Can be null to
        /// play a simple push-off-screen animation instead.</param>
        /// <param name="callback">Called at the end of animation.</param>
        public void CloseGraph(bool animated, RectTransform tile = null, 
            Action callback = null) {
            drawingView.toolbarView.SetToolbarVisibility(false, animated);
            drawingView.SetExpansion(false, animated, tile, () => {
                drawingView.gameObject.SetActive(false);
                callback();
            });
        }

        void SetBackgroundColor(Color color, bool animated) {
            // Test color compatibility.
            color.a = 1;
            float white = Readability.Readability.Evaluate(Color.white, color);
            float black = Readability.Readability.Evaluate(Color.black, color);

            Color choice = Color.black;

            if (white > black) {
                choice = Color.white;
            }

            drawingView.toolbarView.SetButtonColor(choice, animated);

            if (controllerDelegate != null)
                controllerDelegate.OnUIColorChange(this, choice);

            drawingView.paper.SetBackgroundColor(color, animated);
        }

        void SelectObject(Drawable drawable, DrawableView drawableView, 
            Vector2 pos) {
            if (selectedDrawable != null &&
                selectionHandlers.ContainsKey(selectedDrawable.GetType())) {
                selectionHandlers[selectedDrawable.GetType()]
                    .OnDrawableDeselected(selectedDrawable, 
                    selectedDrawableView);
            }

            selectedDrawable = drawable;
            selectedDrawableView = drawableView;

            if (drawable != null && drawableView != null &&
                selectionHandlers.ContainsKey(selectedDrawable.GetType())) {
                selectionHandlers[selectedDrawable.GetType()]
                    .OnDrawableSelected(selectedDrawable, selectedDrawableView, 
                    pos);
            }
        }

        #region IPaperDelegate implementation

        public void OnPaperBeginDrag(Paper paper, Vector2 pos, 
            Vector2 screenPos) {
            // Switch tool. Depending on which tool we initiate different
            // actions.
            dragTool = editingGraph.activeTool;

            if (toolHandlers.ContainsKey(dragTool)) {
                toolHandlers[dragTool].OnPaperBeginDrag(pos, screenPos);
            }
        }

        public void OnPaperDrag(Paper paper, Vector2 pos, 
            Vector2 screenPos) {

            if (toolHandlers.ContainsKey(dragTool)) {
                toolHandlers[dragTool].OnPaperDrag(pos, screenPos);
            }
        }

        public void OnPaperEndDrag(Paper paper, Vector2 pos, 
            Vector2 screenPos) {
            if (toolHandlers.ContainsKey(dragTool)) {
                toolHandlers[dragTool].OnPaperEndDrag(pos, screenPos);
            }

            dragTool = Tool.Unknown;
        }

        public void OnPaperCancelDrag(Paper paper) {
            if (toolHandlers.ContainsKey(dragTool)) {
                toolHandlers[dragTool].OnPaperCancelDrag();
            }

            dragTool = Tool.Unknown;
        }

        // Implement double drag for pan + zoom with these callbacks.
        public void OnPaperBeginDoubleDrag(Paper paper, Vector2 pos1, 
            Vector2 pos2, Vector2 screenPos1, Vector2 screenPos2) {
        }

        public void OnPaperDoubleDrag(Paper paper, Vector2 pos1, Vector2 pos2, 
            Vector2 screenPos1, Vector2 screenPos2) {
        }

        public void OnPaperEndDoubleDrag(Paper paper, Vector2 pos1, 
            Vector2 pos2, Vector2 screenPos1, Vector2 screenPos2) {
        }

        public void OnPaperTap(Paper paper, Vector2 pos, Vector2 screenPos, 
            int count) {
            if (count > 0) {
                // Handle selection in current layer.
                Ray r = RectTransformUtility.ScreenPointToRay(Camera.main, 
                            screenPos);
                RaycastHit2D[] hits = Physics2D.RaycastAll(r.origin, 
                                          r.direction, 100);
                Transform activeLayer = drawingView
                    .GetGraphLayerParentTransform(editingGraph.activeLayer);

                DrawableView highestEligibleView = null;
                Drawable highestEligibleDrawable = null;
                int highestChildIndex = -1;

                foreach (var hit in hits) {
                    DrawableView v = hit.transform.GetComponent<DrawableView>();

                    if (v == null || hit.transform.parent != activeLayer)
                        continue;
                    
                    Drawable drawable = editingGraph.content[activeLayer.GetSiblingIndex()][hit.transform.GetSiblingIndex()];

                    if (!drawable.Selectible()) {
                        continue;
                    }

                    if (highestChildIndex < hit.transform.GetSiblingIndex()) {
                        highestChildIndex = hit.transform.GetSiblingIndex();
                        highestEligibleView = v;
                        highestEligibleDrawable = drawable;
                    }
                }

                // Deselect.
                SelectObject(highestEligibleDrawable, highestEligibleView, 
                    screenPos);
            }

            if (count == 2) {
                // Handle double tap action.
            }
        }

        #endregion

        #region IToolbarViewDelegate implementation

        public void OnToolChanged(Tool newTool) {
            if (toolHandlers.ContainsKey(editingGraph.activeTool))
                toolHandlers[editingGraph.activeTool].OnToolDeselected();
            
            editingGraph.activeTool = newTool;

            if (toolHandlers.ContainsKey(newTool))
                toolHandlers[newTool].OnToolSelected();
        }

        #endregion

        #region ISelectionHandlerDelegate implementation

        public void OnSelectionShouldClear(SelectionHandler handler) {
            SelectObject(null, null, Vector2.zero);
        }

        public void OnSelectionShouldEdit(SelectionHandler handler) {
            throw new NotImplementedException();
        }

        #endregion

        void Start() {
            drawingView.gameObject.SetActive(false);
            paper.paperDelegate = this;
            drawingView.toolbarView.viewDelegate = this;
        }
    }

    public interface IDrawingControllerDelegate {
        /// <summary>
        /// Called when UI color within drawing controller has changed.
        /// </summary>
        /// <param name="controller">Controller.</param>
        /// <param name="color">Color.</param>
        void OnUIColorChange(DrawingController controller, Color color);
    }
}
