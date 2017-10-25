using System;
using UnityEngine;

namespace Antigear.Graph {
    public class PanHandler : ToolHandler {
        Vector3 panBeginPaperPosition;
        Vector2 panBeginScreenPosition;

        public override void OnPaperBeginDrag(Vector2 pos, Vector2 screenPos) {
            panBeginPaperPosition = drawingView.paper.content.position;
            panBeginScreenPosition = screenPos;
        }

        public override void OnPaperDrag(Vector2 pos, Vector2 screenPos) {
            float scaleFactor = drawingView.paper.scaler.scaleFactor;
            Vector3 dl = (screenPos - panBeginScreenPosition) * scaleFactor;

            // Offset by that much.
            drawingView.paper.content.position = panBeginPaperPosition +
                drawingView.paper.content.InverseTransformVector(dl) * drawingView.paper.content.transform.lossyScale.x;
        }
    }
}

