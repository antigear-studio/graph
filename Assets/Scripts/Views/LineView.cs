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
                UpdateTexture();
                vectorLine.vectorLine.Draw();
            }
        }

        public virtual void UpdateView(Line line) {
            vectorLine.vectorLine.points2 = line.GetPoints();
            vectorLine.vectorLine.lineWidth = line.width + 1;  // 0.5 AA dp/side
            vectorLine.vectorLine.SetColor(line.color);
            UpdateTexture();
            vectorLine.vectorLine.Draw();
        }

        protected void UpdateTexture() {
            int width = (int)(vectorLine.vectorLine.lineWidth * 2 * lastScale);
            var texture = new Texture2D(1, width, TextureFormat.ARGB32, false);

            // set the pixel values
            texture.SetPixel(0, 0, Color.clear);
            texture.SetPixel(0, width - 1, Color.clear);

            for (int i = 1; i < width - 1; i++) {
                texture.SetPixel(0, i, Color.white);
            }

            // Apply all SetPixel calls
            texture.Apply();
            vectorLine.vectorLine.texture = texture;
        }
    }
}
