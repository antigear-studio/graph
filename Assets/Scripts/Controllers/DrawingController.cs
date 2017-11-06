using MaterialUI;
using System;
using UnityEngine;
using System.Collections.Generic;


namespace Antigear.Graph {
    /// <summary>
    /// Manages drawing to a graph.
    /// </summary>
    public class DrawingController : MonoBehaviour, IPaperDelegate, 
    IToolbarViewDelegate, ISelectionHandlerDelegate, IToolHandlerDelegate, 
    ISideBarViewDelegate, IHistoryControllerDelegate, IEditHandlerDelegate {
        public IDrawingControllerDelegate controllerDelegate;

        // Outlets.
        public HistoryController historyController;
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

        // Special handler for two finger gestures.
        DoubleDragToolHandler doubleDragHandler = new DoubleDragToolHandler();

        // Selection handlers for each object type.
        readonly Dictionary<Type, SelectionHandler> selectionHandlers = 
            new Dictionary<Type, SelectionHandler> {
                { typeof(StraightLine), new StraightLineSelectionHandler() }
            };

        // Edit handlers for each object type.
        readonly Dictionary<Type, EditHandler> editHandlers = 
            new Dictionary<Type, EditHandler> {
                { typeof(StraightLine), new StraightLineEditHandler() }
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
                handler.SetupToolHandler(graph, drawingView, this);
            }

            foreach (SelectionHandler handler in selectionHandlers.Values) {
                handler.SetupSelectionHandler(graph, drawingView, this);
            }

            foreach (EditHandler handler in editHandlers.Values) {
                handler.SetupEditHandler(graph, drawingView, this);
            }

            doubleDragHandler.SetupToolHandler(graph, drawingView, this);

            historyController.SetupController(graph, drawingView);
            historyController.controllerDelegate = this;
            drawingView.gameObject.SetActive(true);

            drawingView.paper.content.anchoredPosition = 
                graph.lastVisitedPosition;
            drawingView.paper.content.localScale = 
                new Vector3(graph.lastScale, graph.lastScale, 1);
            drawingView.toolbarView.SetToolbarVisibility(true, animated);
            drawingView.SetExpansion(true, true, tile, callback);
            drawingView.toolbarView.ChangeTool(graph.activeTool, false);
            drawingView.sideBarView.SetUndoButtonVisibility(
                historyController.CanUndo());
            drawingView.sideBarView.SetRedoButtonVisibility(
                historyController.CanRedo(), true, 0.1f);
            drawingView.sideBarView.SetSnapButtonVisibility(false, true, 0.2f);
            drawingView.sideBarView.viewDelegate = this;
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
            drawingView.sideBarView.SetVisibility(false, false, false);
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
                selectionHandlers.ContainsKey(drawable.GetType())) {
                selectionHandlers[drawable.GetType()]
                    .OnDrawableSelected(drawable, drawableView, pos);
            }
        }

        void EditObject(Drawable drawable, DrawableView drawableView) {
            if (editingDrawable != null &&
                editHandlers.ContainsKey(editingDrawable.GetType())) {
                editHandlers[editingDrawable.GetType()]
                    .OnDrawableEditEnd(editingDrawable, editingDrawableView);
            }

            editingDrawable = drawable;
            editingDrawableView = drawableView;

            if (drawable != null && drawableView != null &&
                editHandlers.ContainsKey(drawable.GetType())) {
                editHandlers[drawable.GetType()]
                    .OnDrawableEditBegin(drawable, drawableView);
            }
        }

        void UpdateHistoryButtonsVisibility() {
            drawingView.sideBarView.SetUndoButtonVisibility(
                historyController.CanUndo());
            drawingView.sideBarView.SetRedoButtonVisibility(
                historyController.CanRedo());
        }

        #region IPaperDelegate implementation

        public void OnPaperBeginDrag(Paper paper, Vector2 pos, 
            Vector2 screenPos) {
            if (editingDrawable != null) {
                // Editing, so we should relay this info to edit handler.
                if (editHandlers.ContainsKey(editingDrawable.GetType())) {
                    editHandlers[editingDrawable.GetType()]
                        .OnPaperBeginDrag(paper, pos, screenPos);
                }
            } else {
                // Switch tool. Depending on which tool we initiate different
                // actions.
                dragTool = editingGraph.activeTool;

                if (toolHandlers.ContainsKey(dragTool)) {
                    toolHandlers[dragTool].OnPaperBeginDrag(pos, screenPos);
                }
            }
        }

        public void OnPaperDrag(Paper paper, Vector2 pos, Vector2 screenPos) {
            if (editingDrawable != null) {
                // Editing, so we should relay this info to edit handler.
                if (editHandlers.ContainsKey(editingDrawable.GetType())) {
                    editHandlers[editingDrawable.GetType()]
                        .OnPaperDrag(paper, pos, screenPos);
                }
            } else if (toolHandlers.ContainsKey(dragTool)) {
                toolHandlers[dragTool].OnPaperDrag(pos, screenPos);
            }
        }

        public void OnPaperEndDrag(Paper paper, Vector2 pos, 
            Vector2 screenPos) {
            if (editingDrawable != null) {
                // Editing, so we should relay this info to edit handler.
                if (editHandlers.ContainsKey(editingDrawable.GetType())) {
                    editHandlers[editingDrawable.GetType()]
                        .OnPaperEndDrag(paper, pos, screenPos);
                }
            } else if (toolHandlers.ContainsKey(dragTool)) {
                toolHandlers[dragTool].OnPaperEndDrag(pos, screenPos);
            }

            dragTool = Tool.Unknown;
        }

        public void OnPaperCancelDrag(Paper paper) {
            if (editingDrawable != null) {
                // Editing, so we should relay this info to edit handler.
                if (editHandlers.ContainsKey(editingDrawable.GetType())) {
                    editHandlers[editingDrawable.GetType()]
                        .OnPaperCancelDrag(paper);
                }
            } else if (toolHandlers.ContainsKey(dragTool)) {
                toolHandlers[dragTool].OnPaperCancelDrag();
            }

            dragTool = Tool.Unknown;
        }

        // Implement double drag for pan + zoom with these callbacks.
        public void OnPaperBeginDoubleDrag(Paper paper, Vector2 pos1, 
            Vector2 pos2, Vector2 screenPos1, Vector2 screenPos2) {
            doubleDragHandler.OnPaperBeginDoubleDrag(paper, pos1, pos2,
                screenPos1, screenPos2);
        }

        public void OnPaperDoubleDrag(Paper paper, Vector2 pos1, Vector2 pos2, 
            Vector2 screenPos1, Vector2 screenPos2) {
            doubleDragHandler.OnPaperDoubleDrag(paper, pos1, pos2, screenPos1,
                screenPos2);
        }

        public void OnPaperEndDoubleDrag(Paper paper, Vector2 pos1, 
            Vector2 pos2, Vector2 screenPos1, Vector2 screenPos2) {
            doubleDragHandler.OnPaperEndDoubleDrag(paper, pos1, pos2,
                screenPos1, screenPos2);
        }

        public void OnPaperTap(Paper paper, Vector2 pos, Vector2 screenPos, 
            int count) {
            if (editingDrawable == null) {
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
                        DrawableView v = 
                            hit.transform.GetComponent<DrawableView>();

                        if (v == null || hit.transform.parent != activeLayer)
                            continue;

                        int layer = activeLayer.GetSiblingIndex();
                        int index = hit.transform.GetSiblingIndex();
                        Drawable drawable = editingGraph.content[layer][index];

                        if (!drawable.Selectible()) {
                            continue;
                        }

                        if (highestChildIndex <
                            hit.transform.GetSiblingIndex()) {
                            highestChildIndex = hit.transform.GetSiblingIndex();
                            highestEligibleView = v;
                            highestEligibleDrawable = drawable;
                        }
                    }

                    // Deselect.
                    SelectObject(highestEligibleDrawable, highestEligibleView, 
                        screenPos);
                } else if (count == 2) {
                    // Handle double tap action.
                }
            } else {
                // Editing, so we should relay this info to edit handler.
                if (editHandlers.ContainsKey(editingDrawable.GetType())) {
                    editHandlers[editingDrawable.GetType()]
                        .OnPaperTap(paper, pos, screenPos, count);
                }
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

        public void OnSelectionShouldEdit(SelectionHandler handler,
            Drawable selected, DrawableView selectedView) {
            EditObject(selected, selectedView);
        }

        public void OnChange(SelectionHandler handler, Command cmd) {
            historyController.Commit(cmd);
        }

        #endregion

        #region IToolHandlerDelegate implementation

        public void OnChange(ToolHandler handler, Command cmd) {
            historyController.Commit(cmd);
        }

        #endregion

        #region ISideBarViewDelegate implementation

        public void OnUndoButtonPress(SideBarView sideBarView) {
            historyController.Undo();
        }

        public void OnRedoButtonPress(SideBarView sideBarView) {
            historyController.Redo();
        }

        public void OnSnapButtonPress(SideBarView sideBarView) {
            throw new NotImplementedException();
        }

        #endregion

        #region IHistoryControllerDelegate implementation

        public void OnHistoryUndo(HistoryController controller) {
            UpdateHistoryButtonsVisibility();
        }

        public void OnHistoryRedo(HistoryController controller) {
            UpdateHistoryButtonsVisibility();
        }

        public void OnHistoryCommit(HistoryController controller) {
            UpdateHistoryButtonsVisibility();
        }

        #endregion

        #region IEditHandlerDelegate implementation

        public void OnEditShouldEnd(EditHandler handler) {
            EditObject(null, null);
        }

        public void OnChange(EditHandler handler, Command cmd) {
            historyController.Commit(cmd);
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
