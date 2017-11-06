using UnityEngine;
using Vectrosity;

namespace Antigear.Graph {
    /// <summary>
    /// Manages and displays a straight line.
    /// </summary>
    public class StraightLineView : LineView {
        // Outlets
        public RectTransform beginTransform;
        public RectTransform endTransform;

        public override void UpdateView(Drawable drawable, 
            Graph.Preference drawingPreferences, bool animated) {
            base.UpdateView(drawable, drawingPreferences, animated);

            StraightLine line = drawable as StraightLine;

            if (line == null) {
                Debug.LogError("Cannot update view with incompatible model!");
                return;
            }

            // Update the two points.

            if (drawable.isEditing) {
                beginTransform.anchoredPosition = line.startPoint;
                endTransform.anchoredPosition = line.endPoint;
            }

            beginTransform.gameObject.SetActive(drawable.isEditing);
            endTransform.gameObject.SetActive(drawable.isEditing);
        }
    }
}