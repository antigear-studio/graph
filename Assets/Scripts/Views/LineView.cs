using UnityEngine;
using Vectrosity;

namespace Antigear.Graph {
    public abstract class LineView : MonoBehaviour {
        public VectorObject2D vectorLine;

        float lastScale = 1;

        void Update() {
            float scale = transform.lossyScale.x;

            if (Mathf.Abs(scale - lastScale) > 0.01f) {
                lastScale = scale;
                vectorLine.vectorLine.Draw();
            }
        }

        public virtual void UpdateView(Line line) {
            vectorLine.vectorLine.points2 = line.GetPoints();
            vectorLine.vectorLine.lineWidth = line.width + 1;
            vectorLine.vectorLine.SetColor(line.color);
            vectorLine.vectorLine.Draw();
        }
    }
}
