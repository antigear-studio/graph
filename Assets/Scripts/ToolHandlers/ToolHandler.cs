using UnityEngine;

namespace Antigear.Graph {
    /// <summary>
    /// Root tool handler class that abstracts out some of the logic common to
    /// all tool handlers.
    /// </summary>
    public abstract class ToolHandler : IToolHandler {
        protected Graph graph;
        protected DrawingView drawingView;

        #region IToolHandler implementation

        public virtual void SetupToolHandler(Graph graph, 
            DrawingView drawingView) {
            this.graph = graph;
            this.drawingView = drawingView;
        }

        public virtual void OnPaperBeginDrag(Vector2 pos, Vector2 screenPos) {}

        public virtual void OnPaperDrag(Vector2 pos, Vector2 screenPos) {}

        public virtual void OnPaperEndDrag(Vector2 pos, Vector2 screenPos) {}

        public virtual void OnPaperCancelDrag() {}

        public virtual void OnPaperTap(Vector2 pos, Vector2 screenPos, 
            int count) {}

        #endregion
    }
}

