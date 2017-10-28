using System;
using UnityEngine;

namespace Antigear.Graph {
    public class PanHandler : ToolHandler {
        Vector2 panBeginPaperPosition;
        Vector2 panBeginScreenPosition;
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
            panBeginPaperPosition = contentTransform.anchoredPosition;
            panBeginScreenPosition = screenPos;
            UpdateValueText();
        }

        public override void OnPaperDrag(Vector2 pos, Vector2 screenPos) {
            Vector2 begin, end;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                contentTransform, screenPos, Camera.main, out end);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                contentTransform, panBeginScreenPosition, Camera.main, 
                out begin);

            Vector2 dl = (end - begin) * contentTransform.localScale.x;

            // Offset by that much.
            contentTransform.anchoredPosition = panBeginPaperPosition + dl;
            UpdateValueText();
        }

        void UpdateValueText() {
            Vector2 pos = -contentTransform.anchoredPosition / 
                contentTransform.localScale.x;

            string x = string.Format("{0:0.0}", pos.x);
            string y = string.Format("{0:0.0}", pos.y);

            if (Mathf.Abs(pos.x) >= 1000) {
                x = string.Format("{0:0.##e+0}", pos.x);
            }

            // Analysis disable once CompareOfFloatsByEqualityOperator
            if (Mathf.Abs(pos.y) >= 1000) {
                y = string.Format("{0:0.##e+0}", pos.y);
            }

            drawingView.toolbarView.valueText.text = "(" + x + ", " + y + ")";
        }
    }
}
