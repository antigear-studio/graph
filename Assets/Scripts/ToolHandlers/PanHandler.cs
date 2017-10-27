using System;
using UnityEngine;

namespace Antigear.Graph {
    public class PanHandler : ToolHandler {
        Vector2 panBeginPaperPosition;
        Vector2 panBeginScreenPosition;
        RectTransform content;

        public override void OnPaperBeginDrag(Vector2 pos, Vector2 screenPos) {
            content = drawingView.paper.content;
            panBeginPaperPosition = content.anchoredPosition;
            panBeginScreenPosition = screenPos;
        }

        public override void OnPaperDrag(Vector2 pos, Vector2 screenPos) {
            Vector2 begin, end;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(content, 
                screenPos, Camera.main, out end);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(content, 
                panBeginScreenPosition, Camera.main, out begin);

            Vector2 dl = (end - begin) * content.localScale.x;

            // Offset by that much.
            content.anchoredPosition = panBeginPaperPosition + dl;
        }
    }
}
