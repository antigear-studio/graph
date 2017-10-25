using MaterialUI;
using System;
using UnityEngine;
using System.Collections.Generic;


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
        bool isDragging;
        Tool dragTool;

        // Handlers for each tool.
        readonly Dictionary<Tool, IToolHandler> handlers = 
            new Dictionary<Tool, IToolHandler> {
                { Tool.Pan, new PanHandler() }
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
            drawingView.gameObject.SetActive(true);
            drawingView.toolbarView.SetToolbarVisibility(true, animated);
            drawingView.SetExpansion(true, true, tile, callback);
            drawingView.toolbarView.ChangeTool(graph.activeTool, false);
            drawingView.LoadContent(graph.content);
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

        #region IPaperDelegate implementation

        public void OnPaperBeginDrag(Paper paper, Vector2 pos, 
            Vector2 screenPos) {
            // Switch tool. Depending on which tool we initiate different
            // actions.
            isDragging = true;
            dragTool = editingGraph.activeTool;

            if (handlers.ContainsKey(dragTool)) {
                handlers[dragTool].OnPaperBeginDrag(pos, screenPos);
            }
        }

        public void OnPaperDrag(Paper paper, Vector2 pos, 
            Vector2 screenPos) {

            if (handlers.ContainsKey(dragTool)) {
                handlers[dragTool].OnPaperDrag(pos, screenPos);
            }
        }

        public void OnPaperEndDrag(Paper paper, Vector2 pos, 
            Vector2 screenPos) {
            if (handlers.ContainsKey(dragTool)) {
                handlers[dragTool].OnPaperEndDrag(pos, screenPos);
            }

            isDragging = false;
            dragTool = Tool.Unknown;
        }

        public void OnPaperTap(Paper paper, Vector2 pos, Vector2 screenPos, 
            int count) {
            if (handlers.ContainsKey(dragTool)) {
                handlers[dragTool].OnPaperTap(pos, screenPos, count);
            }
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

            foreach (IToolHandler handler in handlers.Values) {
                handler.SetupToolHandler(editingGraph, drawingView);
            }
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

    /// <summary>
    /// Implementer handles tool-related work. This separates the logic of each
    /// tool into different files.
    /// </summary>
    public interface IToolHandler {
        void SetupToolHandler(Graph graph, DrawingView drawingView);

        void OnPaperBeginDrag(Vector2 pos, Vector2 screenPos);

        void OnPaperDrag(Vector2 pos, Vector2 screenPos);

        void OnPaperEndDrag(Vector2 pos, Vector2 screenPos);

        void OnPaperCancelDrag();

        void OnPaperTap(Vector2 pos, Vector2 screenPos, int count);
    }
}
