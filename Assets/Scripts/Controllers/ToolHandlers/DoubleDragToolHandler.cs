using System;
using UnityEngine;

namespace Antigear.Graph {
    public class DoubleDragToolHandler : ToolHandler {
        Vector2 doubleDragBeginTransformPosition;
        Vector2 doubleDragBeginScreenPosition1;
        Vector2 doubleDragBeginScreenPosition2;
        Vector2 doubleDragBeginPosition1;
        Vector2 doubleDragBeginPosition2;
        float doubleDragBeginScale;

        public override void OnPaperBeginDoubleDrag(Paper paper, Vector2 pos1, 
            Vector2 pos2, Vector2 screenPos1, Vector2 screenPos2) {
            base.OnPaperBeginDoubleDrag(paper, pos1, pos2, screenPos1, 
                screenPos2);

            doubleDragBeginTransformPosition = 
                drawingView.paper.content.anchoredPosition;
            doubleDragBeginPosition1 = 
                pos1 / drawingView.paper.scaler.scaleFactor;
            doubleDragBeginPosition2 = 
                pos2 / drawingView.paper.scaler.scaleFactor;
            doubleDragBeginScreenPosition1 = screenPos1;
            doubleDragBeginScreenPosition2 = screenPos2;
            doubleDragBeginScale = drawingView.paper.content.localScale.x;

            UpdateValueText();
        }

        public override void OnPaperDoubleDrag(Paper paper, Vector2 pos1, 
            Vector2 pos2, Vector2 screenPos1, Vector2 screenPos2) {
            base.OnPaperDoubleDrag(paper, pos1, pos2, screenPos1, screenPos2);

            // Scale, then translate.
            float beginMagnitude = (doubleDragBeginScreenPosition1 - 
                doubleDragBeginScreenPosition2).magnitude;
            float currentMagnitude = (screenPos1 - screenPos2).magnitude;
            float scale = Mathf.Max(ZoomToolHandler.MIN_ZOOM, Mathf.Min(
                ZoomToolHandler.MAX_ZOOM, currentMagnitude / beginMagnitude * 
                    doubleDragBeginScale));
            drawingView.paper.content.localScale = new Vector3(scale, scale, 1);
            float dy = scale - doubleDragBeginScale;
            Vector2 doubleDragBeginPosition = (doubleDragBeginPosition1 + 
                doubleDragBeginPosition2) / 2;
            Vector2 offset = dy * doubleDragBeginPosition;

            Vector2 screenPos = (screenPos1 + screenPos2) / 2;
            Vector2 doubleDragBeginScreenPosition = 
                (doubleDragBeginScreenPosition1 + 
                    doubleDragBeginScreenPosition2) / 2;

            Vector2 begin, end;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                drawingView.paper.content, screenPos, Camera.main, out end);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                drawingView.paper.content, doubleDragBeginScreenPosition, 
                Camera.main, out begin);

            Vector2 dl = (end - begin) * drawingView.paper.content.localScale.x;
            Vector2 pos = doubleDragBeginTransformPosition + dl - offset;
            drawingView.paper.content.anchoredPosition = pos;

            UpdateValueText();
            graph.lastVisitedPosition = pos;
            graph.lastScale = scale;
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

            float v = drawingView.paper.content.localScale.x * 100;
            string text = string.Format("{0:0}%", v);

            if (v <= 1) {
                text = string.Format("{0:0.#}%", v);
            }

            text += "\n(" + x + ", " + y + ")";

            drawingView.toolbarView.valueText.text = text;
        }
    }
}
