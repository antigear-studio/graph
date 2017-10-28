using UnityEngine;
using Vectrosity;

namespace Antigear.Graph {
    public abstract class LineView : DrawableView {
        public VectorObject2D vectorLine;

        public override void UpdateView(Drawable drawable, 
            Graph.Preference drawingPreferences) {
            base.UpdateView(drawable, drawingPreferences);

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
        }
    }
}
