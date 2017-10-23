using MaterialUI;
using System;
using UnityEngine;


namespace Antigear.Graph {
    /// <summary>
    /// Manages drawing to a graph.
    /// </summary>
    public class DrawingController : MonoBehaviour, IPaperDelegate, 
    IToolbarViewDelegate {
        public IDrawingControllerDelegate controllerDelegate;

        // Outlets.
        public DrawingView drawingView;
        public Paper paper;

        Graph editingGraph;

        // Bookkeeping.
        Vector2 dragStart;
        bool isDragging;
        Tool dragTool;

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
            drawingView.gameObject.SetActive(true);
            drawingView.toolbarView.SetToolbarVisibility(true, animated);
            drawingView.SetExpansion(true, true, tile, callback);
            drawingView.toolbarView.ChangeTool(graph.activeTool, false);
            SetBackgroundColor(graph.backgroundColor, false);
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

        #region Panning

        Vector3 panBeginPaperPosition;

        void OnPanBeginDrag(Vector2 pos) {
            dragStart = pos;
            panBeginPaperPosition = paper.content.position;
        }

        void OnPanDrag(Vector2 pos) {
            Vector3 dl = (pos - dragStart) / paper.scaler.scaleFactor;

            // Offset by that much.
            paper.content.position = panBeginPaperPosition + 
                paper.content.InverseTransformVector(dl);
        }

        #endregion


        #region IPaperDelegate implementation

        public void OnPaperBeginDrag(Paper paper, Vector2 pos, 
            Vector2 screenPos) {
            // Switch tool. Depending on which tool we initiate different
            // actions.
            isDragging = true;
            dragStart = pos;
            dragTool = editingGraph.activeTool;

            switch (editingGraph.activeTool) {
                case Tool.StraightLine:
                    break;
                case Tool.BezierCurve:
                    break;
                case Tool.Arc:
                    break;
                case Tool.FreeformLine:
                    break;
                case Tool.Pencil:
                    break;
                case Tool.Eraser:
                    break;
                case Tool.Text:
                    break;
                case Tool.Image:
                    break;
                case Tool.RectangleSelection:
                    break;
                case Tool.LassoSelection:
                    break;
                case Tool.Zoom:
                    break;
                case Tool.Pan:
                    OnPanBeginDrag(screenPos);
                    break;
            }
        }

        public void OnPaperDrag(Paper paper, Vector2 pos, 
            Vector2 screenPos) {
            switch (dragTool) {
                case Tool.StraightLine:
                    break;
                case Tool.BezierCurve:
                    break;
                case Tool.Arc:
                    break;
                case Tool.FreeformLine:
                    break;
                case Tool.Pencil:
                    break;
                case Tool.Eraser:
                    break;
                case Tool.Text:
                    break;
                case Tool.Image:
                    break;
                case Tool.RectangleSelection:
                    break;
                case Tool.LassoSelection:
                    break;
                case Tool.Zoom:
                    break;
                case Tool.Pan:
                    OnPanDrag(screenPos);
                    break;
            }
        }

        public void OnPaperEndDrag(Paper paper, Vector2 pos, 
            Vector2 screenPos) {
            switch (dragTool) {
                case Tool.StraightLine:
                    break;
                case Tool.BezierCurve:
                    break;
                case Tool.Arc:
                    break;
                case Tool.FreeformLine:
                    break;
                case Tool.Pencil:
                    break;
                case Tool.Eraser:
                    break;
                case Tool.Text:
                    break;
                case Tool.Image:
                    break;
                case Tool.RectangleSelection:
                    break;
                case Tool.LassoSelection:
                    break;
                case Tool.Zoom:
                    break;
                case Tool.Pan:
                    break;
            }

            isDragging = false;
            dragTool = Tool.Unknown;
        }

        public void OnPaperTap(Paper paper, Vector2 pos, Vector2 screenPos, 
            int count) {
        }

        #endregion

        #region IToolbarViewDelegate implementation

        public void OnToolChanged(Tool newTool) {
            editingGraph.activeTool = newTool;
            // TODO: need some cleaning up to do depending on the tool selected.
            // E.g. if we are creating a new object, changing a tool results in
            // object being discarded.
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
