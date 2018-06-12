using MaterialUI;
using UnityEngine;

namespace Antigear.Graph {
    /// <summary>
    /// Handles drawable selection-related work. This separates the logic of 
    /// selection of each object into different files.
    /// </summary>
    public abstract class SelectionHandler {
        readonly static Vector2 OffsetAmount = new Vector2(10, -10);

        protected Graph graph;
        protected DrawingView drawingView;

        Drawable selected;
        DrawableView selectedView;
        protected ISelectionHandlerDelegate handlerDelegate;

        /// <summary>
        /// Setups the selection handler with the given graph and view.
        /// </summary>
        /// <param name="graph">Graph.</param>
        /// <param name="drawingView">Drawing view.</param>
        /// <param name="handler">Delegate for this handler.</param>
        public virtual void SetupSelectionHandler(Graph graph, 
            DrawingView drawingView, ISelectionHandlerDelegate handler) {
            this.graph = graph;
            this.drawingView = drawingView;
            this.handlerDelegate = handler;
        }

        /// <summary>
        /// Raised when an object is selected by user.
        /// </summary>
        /// <param name="selected">Selected drawable.</param>
        /// <param name="selectedView">Selected drawable's view.</param>
        /// <param name="screenPos">Screen position of the selection point.
        /// </param>
        public virtual void OnDrawableSelected(Drawable selected, DrawableView
            selectedView, Vector2 screenPos) {
            this.selected = selected;
            this.selectedView = selectedView;
            selected.isSelected = true;
            selected.timeLastSelected = Time.time;
            selectedView.UpdateView(selected, graph.preferences, true);
            UpdateMenu(GetMenuItems(), GetMenuPosition(screenPos));
            drawingView.selectionMenu.Show(false);
            drawingView.selectionMenu.onItemSelected.RemoveAllListeners();
            drawingView.selectionMenu.onItemSelected.AddListener(OnMenuSelect);
            drawingView.selectionMenuMask.SetActive(true);
        }

        /// <summary>
        /// Raised when an object is deselected by user.
        /// </summary>
        /// <param name="deselected">Deselected drawable.</param>
        /// <param name="deselectedView">Deselected drawable's view.</param>
        public virtual void OnDrawableDeselected(Drawable deselected, 
            DrawableView deselectedView) {
            deselected.isSelected = false;
            deselectedView.UpdateView(deselected, graph.preferences, true);
            drawingView.selectionMenu.Hide(false);
            drawingView.selectionMenuMask.SetActive(false);
        }

        /// <summary>
        /// Updates the menu.
        /// </summary>
        /// <param name="options">Options.</param>
        /// <param name="pos">Position.</param>
        protected virtual void UpdateMenu(OptionData[] options, Vector2 pos) {
            MaterialDropdown menu = drawingView.selectionMenu;
            menu.ClearData();
            menu.AddData(options);
            Vector3 curr = menu.transform.position;
            curr.x = pos.x;
            curr.y = pos.y;
            menu.transform.position = curr;
        }

        /// <summary>
        /// Generates a list of the menu items to display.
        /// </summary>
        /// <returns>The menu items.</returns>
        protected virtual OptionData[] GetMenuItems() {
            return null;
        }

        /// <summary>
        /// Calculates menu's position based on the screen position.
        /// </summary>
        /// <returns>The menu position.</returns>
        /// <param name="screenPos">Screen position.</param>
        protected virtual Vector2 GetMenuPosition(Vector2 screenPos) {
            return Camera.main.ScreenToWorldPoint(screenPos);
        }

        protected virtual void OnMenuSelect(int itemIndex) {
            handlerDelegate.OnSelectionShouldClear(this);
        }

        protected virtual void OnEdit() {
            handlerDelegate.OnSelectionShouldEdit(this, selected, selectedView);
        }

        protected virtual void OnCopy() {
            int layerIndex = selectedView.transform.parent.GetSiblingIndex();
            Drawable copy = selected.Copy();
            copy.Offset(OffsetAmount);
            graph.content[layerIndex].Add(copy);
            DrawableView copyView = Object.Instantiate(selectedView, 
                selectedView.transform.parent);
            copyView.UpdateView(copy, graph.preferences, false);

            if (handlerDelegate != null) {
                Command cmd = new Command();
                cmd.type = Command.Type.CreateDrawable;
                cmd.drawableIndex = graph.content[layerIndex].Count - 1;
                cmd.layerIndex = layerIndex;
                cmd.currentDrawable = copy;
                handlerDelegate.OnChange(this, cmd);
            }
        }

        protected virtual void OnDelete() {
            int drawableIndex = selectedView.transform.GetSiblingIndex();
            int layerIndex = selectedView.transform.parent.GetSiblingIndex();
            Drawable toDelete = graph.content[layerIndex][drawableIndex];
            graph.content[layerIndex].RemoveAt(drawableIndex);
            Object.Destroy(selectedView.gameObject);

            if (handlerDelegate != null) {
                Command cmd = new Command();
                cmd.type = Command.Type.DeleteDrawable;
                cmd.drawableIndex = drawableIndex;
                cmd.layerIndex = layerIndex;
                cmd.previousDrawable = toDelete;
                handlerDelegate.OnChange(this, cmd);
            }
        }

        protected virtual void OnBringToFront() {
            int drawableIndex = selectedView.transform.GetSiblingIndex();
            int layerIndex = selectedView.transform.parent.GetSiblingIndex();
            Layer copy = graph.content[layerIndex].Copy();
            graph.content[layerIndex].RemoveAt(drawableIndex);
            graph.content[layerIndex].Add(selected);
            selectedView.transform.SetAsLastSibling();

            if (handlerDelegate != null) {
                Command cmd = new Command();
                cmd.type = Command.Type.UpdateLayer;
                cmd.layerIndex = layerIndex;
                cmd.previousLayer = copy;
                cmd.currentLayer = graph.content[layerIndex];
                handlerDelegate.OnChange(this, cmd);
            }
        }

        protected virtual void OnSendToBack() {
            int drawableIndex = selectedView.transform.GetSiblingIndex();
            int layerIndex = selectedView.transform.parent.GetSiblingIndex();
            Layer copy = graph.content[layerIndex].Copy();
            graph.content[layerIndex].RemoveAt(drawableIndex);
            graph.content[layerIndex].Insert(0, selected);
            selectedView.transform.SetAsFirstSibling();

            if (handlerDelegate != null) {
                Command cmd = new Command();
                cmd.type = Command.Type.UpdateLayer;
                cmd.layerIndex = layerIndex;
                cmd.previousLayer = copy;
                cmd.currentLayer = graph.content[layerIndex];
                handlerDelegate.OnChange(this, cmd);
            }
        }
    }

    public interface ISelectionHandlerDelegate {
        /// <summary>
        /// Raises the selection should clear event. This happens when the
        /// selected object should be deselected.
        /// </summary>
        /// <param name="handler">Handler sending this event.</param>
        void OnSelectionShouldClear(SelectionHandler handler);

        /// <summary>
        /// Raises the selection should edit event. This happens when the
        /// selected object should be edited.
        /// </summary>
        /// <param name="handler">Handler.</param>
        /// <param name="selected">Selected drawable.</param>
        /// <param name="selectedView">View for selected drawable.</param>
        void OnSelectionShouldEdit(SelectionHandler handler, Drawable selected, 
            DrawableView selectedView);

        /// <summary>
        /// Raises when the graph object changes in a way supported by History
        /// Controller for undo/redo.
        /// </summary>
        /// <param name="handler">Handler.</param>
        /// <param name="cmd">Command that records this change.</param>
        void OnChange(SelectionHandler handler, Command cmd);
    }
}
