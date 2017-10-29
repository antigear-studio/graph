using UnityEngine;

namespace Antigear.Graph {
    /// <summary>
    /// Root tool handler class that abstracts out some of the logic common to
    /// all tool handlers.
    /// </summary>
    public abstract class ToolHandler {
        protected Graph graph;
        protected DrawingView drawingView;

        /// <summary>
        /// Setups the tool handler with the given graph and view.
        /// </summary>
        /// <param name="graph">Graph.</param>
        /// <param name="drawingView">Drawing view.</param>
        public virtual void SetupToolHandler(Graph graph, 
            DrawingView drawingView) {
            this.graph = graph;
            this.drawingView = drawingView;
        }

        /// <summary>
        /// Raised when this tool is selected by user.
        /// </summary>
        public virtual void OnToolSelected() {}

        /// <summary>
        /// Raised when this tool is deselected by user.
        /// </summary>
        public virtual void OnToolDeselected() {
            OnPaperCancelDrag();
        }

        /// <summary>
        /// Raised when a drag is detected while this tool is selected by user.
        /// </summary>
        public virtual void OnPaperBeginDrag(Vector2 pos, Vector2 screenPos) {}

        /// <summary>
        /// Raised when a drag moved while this tool is selected by user.
        /// </summary>
        public virtual void OnPaperDrag(Vector2 pos, Vector2 screenPos) {}

        /// <summary>
        /// Raised when a drag is completed while this tool is selected by user.
        /// </summary>
        public virtual void OnPaperEndDrag(Vector2 pos, Vector2 screenPos) {}

        /// <summary>
        /// Raised when a drag is cancelled while this tool is selected by user.
        /// </summary>
        public virtual void OnPaperCancelDrag() {}
    }
}
