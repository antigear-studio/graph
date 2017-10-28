using UnityEngine;
using Vectrosity;

namespace Antigear.Graph {
    public abstract class LineView : DrawableView {
        public VectorObject2D vectorLine;
        public PolygonCollider2D polygonCollider;
        public EdgeCollider2D edgeCollider;

        public override void UpdateView(Drawable drawable, 
            Graph.Preference drawingPreferences, bool animated) {
            base.UpdateView(drawable, drawingPreferences, animated);

            if (!(drawable is Line)) {
                Debug.LogError("Cannot update view with incompatible model!");
                return;
            }

            Line line = drawable as Line;
            vectorLine.vectorLine.points2 = line.GetPoints();
            vectorLine.vectorLine.lineWidth = line.width + 1;

            if (drawable.isSelected) {
                vectorLine.vectorLine.SetColor(
                    drawingPreferences.selectionHighlightColor);
            } else {
                vectorLine.vectorLine.SetColor(line.color);
            }

            vectorLine.vectorLine.Draw();

            // Update polygon collider based on edge collider.
            polygonCollider.SetPath(0, edgeCollider.points);
        }
    }
}
