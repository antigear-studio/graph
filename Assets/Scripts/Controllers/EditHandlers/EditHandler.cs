using MaterialUI;
using UnityEngine;

namespace Antigear.Graph {
    /// <summary>
    /// Handles drawable editing. This separates the logic of editing of each 
    /// object into different files.
    /// </summary>
    public abstract class EditHandler : IPaperDelegate {
        protected Graph graph;
        protected DrawingView drawingView;

        Drawable editing;
        DrawableView editingView;
        int layer;
        int index;
        protected IEditHandlerDelegate handlerDelegate;

        /// <summary>
        /// Setups the selection handler with the given graph and view.
        /// </summary>
        /// <param name="graph">Graph.</param>
        /// <param name="drawingView">Drawing view.</param>
        /// <param name="handler">Delegate for this handler.</param>
        public virtual void SetupEditHandler(Graph graph, 
            DrawingView drawingView, IEditHandlerDelegate handler) {
            this.graph = graph;
            this.drawingView = drawingView;
            handlerDelegate = handler;
        }

        /// <summary>
        /// Raised when an object is selected by user.
        /// </summary>
        /// <param name="editing">Editing drawable.</param>
        /// <param name="editingView">Editing drawable's view.</param>
        public virtual void OnDrawableEditBegin(Drawable editing,
            DrawableView editingView) {
            this.editing = editing;
            this.editingView = editingView;
            editing.isEditing = true;
            editingView.UpdateView(editing, graph.preferences, true);
            index = editingView.transform.GetSiblingIndex();
            layer = editingView.transform.parent.GetSiblingIndex();
            editingView.transform.SetParent(drawingView
                .GetGraphLayerParentTransform(drawingView.GetEditLayerIndex()));
            drawingView.paper.editLayerObject.SetActive(true);
        }

        /// <summary>
        /// Raised when an object is no longer in edit mode.
        /// </summary>
        /// <param name="editing">Editing drawable.</param>
        /// <param name="editingView">Editing drawable's view.</param>
        public virtual void OnDrawableEditEnd(Drawable editing, 
            DrawableView editingView) {
            editing.isEditing = false;
            editingView.UpdateView(editing, graph.preferences, true);
            editingView.transform.SetParent(drawingView
                .GetGraphLayerParentTransform(layer));
            editingView.transform.SetSiblingIndex(index);
            drawingView.paper.editLayerObject.SetActive(false);
        }

        #region IPaperDelegate implementation

        public void OnPaperBeginDrag(Paper paper, Vector2 pos,
            Vector2 screenPos) {}

        public void OnPaperDrag(Paper paper, Vector2 pos, Vector2 screenPos) {}

        public void OnPaperEndDrag(Paper paper, Vector2 pos,
            Vector2 screenPos) {}

        public void OnPaperCancelDrag(Paper paper) {}

        public void OnPaperTap(Paper paper, Vector2 pos, Vector2 screenPos,
            int count) {
            handlerDelegate.OnEditShouldEnd(this);
        }

        public void OnPaperBeginDoubleDrag(Paper paper, Vector2 pos1,
            Vector2 pos2, Vector2 screenPos1, Vector2 screenPos2) {}

        public void OnPaperDoubleDrag(Paper paper, Vector2 pos1, Vector2 pos2,
            Vector2 screenPos1, Vector2 screenPos2) {}

        public void OnPaperEndDoubleDrag(Paper paper, Vector2 pos1,
            Vector2 pos2, Vector2 screenPos1, Vector2 screenPos2) {}

        #endregion
    }

    public interface IEditHandlerDelegate {
        /// <summary>
        /// Raises the end editing event. This happens when the editing process
        /// should stop.
        /// </summary>
        /// <param name="handler">Handler sending this event.</param>
        void OnEditShouldEnd(EditHandler handler);

        /// <summary>
        /// Raises when the graph object changes in a way supported by History
        /// Controller for undo/redo.
        /// </summary>
        /// <param name="handler">Handler.</param>
        /// <param name="cmd">Command that records this change.</param>
        void OnChange(EditHandler handler, Command cmd);
    }
}
