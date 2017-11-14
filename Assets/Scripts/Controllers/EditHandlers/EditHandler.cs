using MaterialUI;
using UnityEngine;

namespace Antigear.Graph {
    /// <summary>
    /// Handles drawable editing. This separates the logic of editing of each 
    /// object into different files.
    /// </summary>
    public abstract class EditHandler : IPaperDelegate, DrawableViewDelegate {
        protected Graph graph;
        protected DrawingView drawingView;
        protected Drawable editing;
        protected DrawableView editingView;

        // Used to implement undo/redo commands by copying the drawable before
        // any modification. Operation count accounts for multiple modifications
        // going on simultaneously. The command is only pushed when the counter
        // returns to 0.
        protected Drawable copy;
        protected int operationCount;

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
            this.editingView.viewDelegate = this;
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
            editingView.viewDelegate = null;
            editingView.UpdateView(editing, graph.preferences, true);
            editingView.transform.SetParent(drawingView
                .GetGraphLayerParentTransform(layer));
            editingView.transform.SetSiblingIndex(index);
            drawingView.paper.editLayerObject.SetActive(false);
        }

        /// <summary>
        /// Begins the modification of some state. This is used to indicate that
        /// a copy of the drawable should be cached, if not already. This object
        /// is then used again at EndModification to construct a Command object
        /// to support undo/redo. While multiple BeginModification can be called
        /// the number of BeginModification() and EndModification() must equal.
        /// The Command object is constructed when the modification stack is
        /// empty.
        /// </summary>
        protected void BeginModification() {
            if (operationCount++ == 0)
                copy = editing.Copy();
        }

        /// <summary>
        /// Ends the modification of some state. See BeginModification() for how
        /// this works.
        /// </summary>
        protected void EndModification() {
            operationCount = Mathf.Max(0, operationCount - 1);

            if (operationCount == 0 && copy != null) {
                if (handlerDelegate != null) {
                    Command cmd = new Command();
                    cmd.type = Command.Type.UpdateDrawable;
                    cmd.layerIndex = layer;
                    cmd.drawableIndex = index;
                    cmd.currentDrawable = editing.Copy();
                    cmd.previousDrawable = copy;
                    handlerDelegate.OnChange(this, cmd);
                }

                copy = null;
            }
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
