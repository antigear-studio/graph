using MaterialUI;
using UnityEngine;

namespace Antigear.Graph {
    /// <summary>
    /// Handles drawable selection-related work. This separates the logic of 
    /// selection of each object into different files.
    /// </summary>
    public abstract class SelectionHandler {
        protected Graph graph;
        protected DrawingView drawingView;

        /// <summary>
        /// Setups the selection handler with the given graph and view.
        /// </summary>
        /// <param name="graph">Graph.</param>
        /// <param name="drawingView">Drawing view.</param>
        public virtual void SetupSelectionHandler(Graph graph, 
            DrawingView drawingView) {
            this.graph = graph;
            this.drawingView = drawingView;
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
            selected.isSelected = true;
            selected.timeLastSelected = Time.time;
            selectedView.UpdateView(selected, graph.preferences, true);
            UpdateMenu(GetMenuItems(), GetMenuPosition(screenPos));
            drawingView.selectionMenu.Show(false);
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
    }
}
