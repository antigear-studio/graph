using System.Collections;
using System.Collections.Generic;
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
            selected.isSelected = false;
            selected.timeLastSelected = Time.time;
            selectedView.UpdateView(selected, graph.preferences, true);
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
        }
    }
}
