using System;
using UnityEngine;

namespace Antigear.Graph {
    public class ZoomHandler : ToolHandler {
        Vector2 zoomBeginTransformPosition;
        Vector2 zoomBeginPosition;
        Vector2 zoomBeginScreenPosition;
        float zoomBeginScale;
        RectTransform contentTransform;

        public override void SetupToolHandler(Graph graph, 
            DrawingView drawingView) {
            base.SetupToolHandler(graph, drawingView);
            contentTransform = 
                drawingView.paper.transform.GetChild(0) as RectTransform;
        }

        public override void OnPaperBeginDrag(Vector2 pos, Vector2 screenPos) {
            zoomBeginTransformPosition = contentTransform.anchoredPosition;
            zoomBeginPosition = pos / drawingView.paper.scaler.scaleFactor;
            zoomBeginScreenPosition = screenPos;
            zoomBeginScale = contentTransform.localScale.x;
        }

        public override void OnPaperDrag(Vector2 pos, Vector2 screenPos) {
            float dy = 
                (screenPos.y - zoomBeginScreenPosition.y) / Screen.height;

            // Up is zoom in, down is zoom out. Amount is 50% of initial.
            Vector3 scale = contentTransform.localScale;
            scale.x = zoomBeginScale + zoomBeginScale * dy;
            scale.y = zoomBeginScale + zoomBeginScale * dy;
            contentTransform.localScale = scale;

            // Shift canvas such that zoomBeginPos stays at the same position.
            contentTransform.anchoredPosition = zoomBeginTransformPosition - 
                dy * zoomBeginPosition * zoomBeginScale; 
        }
    }
}

