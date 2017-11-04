using System;
using UnityEngine;

namespace Antigear.Graph {
    public class ZoomToolHandler : ToolHandler {
        public const float MAX_ZOOM = 1000;
        public const float MIN_ZOOM = 0.001f;

        Vector2 zoomBeginTransformPosition;
        Vector2 zoomBeginPosition;
        Vector2 zoomBeginScreenPosition;
        float zoomBeginScale;

        public override void OnToolSelected() {
            UpdateValueText();
        }

        public override void OnToolDeselected() {
            drawingView.toolbarView.valueText.text = "";
        }

        public override void OnPaperBeginDrag(Vector2 pos, Vector2 screenPos) {
            zoomBeginTransformPosition = 
                drawingView.paper.content.anchoredPosition;
            zoomBeginPosition = pos / drawingView.paper.scaler.scaleFactor;
            zoomBeginScreenPosition = screenPos;
            zoomBeginScale = drawingView.paper.content.localScale.x;
            UpdateValueText();
        }

        public override void OnPaperDrag(Vector2 pos, Vector2 screenPos) {
            float dy = 
                (screenPos.y - zoomBeginScreenPosition.y) / Screen.height;

            // Up is zoom in, down is zoom out. Amount is 50% of initial.
            float scale = Mathf.Max(MIN_ZOOM, 
                Mathf.Min(MAX_ZOOM, (1 + dy) * zoomBeginScale));
            drawingView.paper.content.localScale = new Vector3(scale, scale, 1);

            // Shift canvas such that zoomBeginPos stays at the same position.
            drawingView.paper.content.anchoredPosition = 
                zoomBeginTransformPosition - (scale - zoomBeginScale) * 
                zoomBeginPosition;

            UpdateValueText();
            graph.lastScale = drawingView.paper.content.localScale.x;
            graph.lastVisitedPosition = 
                drawingView.paper.content.anchoredPosition;
        }

        void UpdateValueText() {
            float v = drawingView.paper.content.localScale.x * 100;

            if (v > 1) {
                drawingView.toolbarView.valueText.text = 
                    string.Format("{0:0}%", v);
            } else {
                drawingView.toolbarView.valueText.text = 
                    string.Format("{0:0.#}%", v);
            }
        }
    }
}

