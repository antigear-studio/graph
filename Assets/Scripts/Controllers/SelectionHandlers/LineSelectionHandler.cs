using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Antigear.Graph {
    /// <summary>
    /// Line selection handler.
    /// </summary>
    public abstract class LineSelectionHandler : SelectionHandler {
        protected Line selectedLine;
        protected LineView selectedLineView;

        public override void OnDrawableSelected(Drawable selected,
            DrawableView selectedView, Vector2 screenPos) {
            selectedLineView = selectedView as LineView;
            selectedLine = selected as Line;
            base.OnDrawableSelected(selected, selectedView, screenPos);
        }
        /// <summary>
        /// Calculates menu's position based on the screen position.
        /// </summary>
        /// <returns>The menu position.</returns>
        /// <param name="screenPos">Screen position.</param>
        protected override Vector2 GetMenuPosition(Vector2 screenPos) {
            Vector2 worldPos = base.GetMenuPosition(screenPos);

            // Do a binary search on closest distance.
            selectedLineView.vectorLine.vectorLine.SetDistances();

            return worldPos;
        }
    }
}