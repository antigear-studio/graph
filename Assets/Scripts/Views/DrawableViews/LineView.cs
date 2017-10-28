using MaterialUI;
using UnityEngine;
using Vectrosity;

namespace Antigear.Graph {
    public abstract class LineView : DrawableView {
        public VectorObject2D vectorLine;
        public PolygonCollider2D polygonCollider;
        public EdgeCollider2D edgeCollider;

        const float COLOR_ANIM_DURATION = 0.15f;

        int colorAnimationTweenId = -1;

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

            Color targetColor = drawable.isSelected ? 
                drawingPreferences.selectionHighlightColor : line.color;
            UpdateColor(targetColor, animated);
            vectorLine.vectorLine.Draw();

            // Update polygon collider based on edge collider.
            polygonCollider.SetPath(0, edgeCollider.points);
        }

        void UpdateColor(Color target, bool animated) {
            if (colorAnimationTweenId > 0) {
                TweenManager.EndTween(colorAnimationTweenId);
                colorAnimationTweenId = -1;
            }

            if (animated) {
                colorAnimationTweenId = TweenManager.TweenColor(v => {
                    vectorLine.vectorLine.SetColor(v);
                    vectorLine.vectorLine.Draw();
                }, vectorLine.vectorLine.GetColor(0), target, COLOR_ANIM_DURATION);

            } else {
                vectorLine.vectorLine.SetColor(target);
            }
        }
    }
}
