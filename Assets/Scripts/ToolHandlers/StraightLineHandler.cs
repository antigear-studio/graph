using UnityEngine;

namespace Antigear.Graph {
    public class StraightLineHandler : ToolHandler {
        StraightLine previewLine;
        StraightLineView previewLineView;

        public override void OnPaperBeginDrag(Vector2 pos, Vector2 screenPos) {
            // Create a line for preview.
            previewLine = new StraightLine();
            previewLine.startPoint = pos;
            previewLine.endPoint = pos;

            // Create a prefab and set the line as its model.
            if (previewLineView == null) {
                int layer = drawingView.GetPreviewLayerIndex();
                previewLineView = drawingView
                    .InstantiateToolPrefab(Tool.StraightLine, layer)
                    .GetComponent<StraightLineView>();
            }

            previewLineView.UpdateView(previewLine);
        }

        public override void OnPaperDrag(Vector2 pos, Vector2 screenPos) {
            // Update the model, thus the view as well.
            previewLine.endPoint = pos;
            previewLineView.UpdateView(previewLine);
        }

        public override void OnPaperEndDrag(Vector2 pos, Vector2 screenPos) {
            // Add the line to graph. Remove preview object.
            previewLine.endPoint = pos;
            previewLineView.UpdateView(previewLine);
            previewLineView.transform.parent = 
                drawingView.GetGraphLayerParentTransform(graph.activeLayer);
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
