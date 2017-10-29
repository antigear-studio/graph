using System;
using UnityEngine;

namespace Antigear.Graph {
    public class ZoomToolHandler : ToolHandler {
        const float MAX_ZOOM = 1000;
        const float MIN_ZOOM = 0.001f;

        Vector2 zoomBeginTransformPosition;
        Vector2 zoomBeginPosition;
        Vector2 zoomBeginScreenPosition;
        float zoomBeginScale;
        RectTransform contentTransform;

        public override void SetupToolHandler(Graph graph, 
            DrawingView drawingView) {
            base.SetupToolHandler(graph, drawingView);
            contentTransform = drawingView.paper.content;
        }

        public override void OnToolSelected() {
            UpdateValueText();
        }

        public override void OnToolDeselected() {
            drawingView.toolbarView.valueText.text = "";
        }

        public override void OnPaperBeginDrag(Vector2 pos, Vector2 screenPos) {
            zoomBeginTransformPosition = contentTransform.anchoredPosition;
            zoomBeginPosition = pos / drawingView.paper.scaler.scaleFactor;
            zoomBeginScreenPosition = screenPos;
            zoomBeginScale = contentTransform.localScale.x;
            UpdateValueText();
        }

        public override void OnPaperDrag(Vector2 pos, Vector2 screenPos) {
            float dy = 
                (screenPos.y - zoomBeginScreenPosition.y) / Screen.height;

            // Up is zoom in, down is zoom out. Amount is 50% of initial.
            float v = Mathf.Max(MIN_ZOOM, 
                Mathf.Min(MAX_ZOOM, (1 + dy) * zoomBeginScale));
            contentTransform.localScale = new Vector3(v, v, 1);

            // Shift canvas such that zoomBeginPos stays at the same position.
            contentTransform.anchoredPosition = zoomBeginTransformPosition - 
                dy * zoomBeginPosition * zoomBeginScale;

            UpdateValueText();
        }

        void UpdateValueText() {
            float v = 100 * contentTransform.localScale.x;

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

