using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Antigear.Graph {
    public class Text : UnityEngine.UI.Text {
        public string fullText;

        void Update() {
            Vector2 extents = rectTransform.rect.size;
            float scale = extents.x / preferredWidth;

            if (scale < 1) {
                text += "...";

                while (scale < 1 && text.Length > 3) {
                    text = text.Substring(0, text.Length - 4) + "...";
                    scale = extents.x / preferredWidth;
                }
            }
        }
    }
}
