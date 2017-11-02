using System;
using UnityEngine;

namespace Antigear.Graph {
    public class PanToolHandler : ToolHandler {
        Vector2 panBeginPaperPosition;
        Vector2 panBeginScreenPosition;

        public override void OnToolSelected() {
            UpdateValueText();
        }

        public override void OnToolDeselected() {
            drawingView.toolbarView.valueText.text = "";
        }

        public override void OnPaperBeginDrag(Vector2 pos, Vector2 screenPos) {
            panBeginPaperPosition = drawingView.paper.content.anchoredPosition;
            panBeginScreenPosition = screenPos;
            UpdateValueText();
        }

        public override void OnPaperDrag(Vector2 pos, Vector2 screenPos) {
            Vector2 begin, end;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                drawingView.paper.content, screenPos, Camera.main, out end);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                drawingView.paper.content, panBeginScreenPosition, Camera.main, 
                out begin);

            Vector2 dl = (end - begin) * drawingView.paper.content.localScale.x;

            // Offset by that much.
            drawingView.paper.content.anchoredPosition = 
                panBeginPaperPosition + dl;
            
            UpdateValueText();
            graph.lastVisitedPosition = 
                drawingView.paper.content.anchoredPosition;
        }

        void UpdateValueText() {
            Vector2 pos = -drawingView.paper.content.anchoredPosition / 
                drawingView.paper.content.localScale.x;
            
            string x = string.Format("{0:0.0}", pos.x);
            string y = string.Format("{0:0.0}", pos.y);

            if (Mathf.Abs(pos.x) >= 1000) {
                x = string.Format("{0:0.##e+0}", pos.x);
            }

            if (Mathf.Abs(pos.y) >= 1000) {
                y = string.Format("{0:0.##e+0}", pos.y);
            }

            drawingView.toolbarView.valueText.text = "(" + x + ", " + y + ")";
        }
    }
}
