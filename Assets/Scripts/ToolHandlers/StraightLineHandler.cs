using System;
using UnityEngine;

namespace Antigear.Graph {
    public class StraightLineHandler : ToolHandler {
        StraightLine previewLine;
        StraightLineView previewLineView;
        StraightLineView prefab;

        public override void SetupToolHandler(Graph graph, 
            DrawingView drawingView) {
            base.SetupToolHandler(graph, drawingView);
            prefab = (StraightLineView)Resources.Load("StraightLinePrefab", 
                typeof(StraightLineView));
        }

        public override void OnPaperBeginDrag(Vector2 pos, Vector2 screenPos) {
            // Create a line for preview.
            previewLine = new StraightLine();
            previewLine.startPoint = pos * drawingView.paper.scaler.scaleFactor;
            previewLine.endPoint = pos;

            // Create a prefab and set the line as its model.
            if (previewLineView == null) {
                previewLineView = UnityEngine.Object.Instantiate(prefab);
            }

            // Add the prefab to the scene.
            Transform t = 
                drawingView.GetGraphLayerParentTransform(graph.activeLayer);
            previewLineView.transform.SetParent(t, true);
            RectTransform r = previewLineView.transform as RectTransform;
            r.anchoredPosition3D = Vector3.zero;
            r.localScale = Vector3.one;
            previewLineView.UpdateView(previewLine);
        }

        public override void OnPaperDrag(Vector2 pos, Vector2 screenPos) {
            // Update the model, thus the view as well.
            previewLine.endPoint = pos * drawingView.paper.scaler.scaleFactor;
            previewLineView.UpdateView(previewLine);
        }

        public override void OnPaperEndDrag(Vector2 pos, Vector2 screenPos) {
            // Add the line to graph. Remove preview object.
            previewLine.endPoint = pos * drawingView.paper.scaler.scaleFactor;
            previewLineView.UpdateView(previewLine);

            // TODO: add this to graph and clear instead of destroy.
            // UnityEngine.Object.Destroy(previewLineView.gameObject);
            previewLineView = null;
            graph.content[graph.activeLayer].Add(previewLine);
            previewLine = null;
        }

        public override void OnPaperCancelDrag() {
            // Clean up preview object if any.
            UnityEngine.Object.Destroy(previewLineView.gameObject);
            previewLineView = null;
        }
    }
}

