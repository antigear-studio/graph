using UnityEngine;

namespace Antigear.Graph {
    public class StraightLineToolHandler : ToolHandler {
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
                    .InstantiatePrefab(previewLine, layer)
                    .GetComponent<StraightLineView>();
            }

            previewLineView.UpdateView(previewLine, graph.preferences, true);
        }

        public override void OnPaperDrag(Vector2 pos, Vector2 screenPos) {
            // Update the model, thus the view as well.
            previewLine.endPoint = pos;
            previewLineView.UpdateView(previewLine, graph.preferences, true);
        }

        public override void OnPaperEndDrag(Vector2 pos, Vector2 screenPos) {
            // Add the line to graph. Remove preview object.
            previewLine.endPoint = pos;
            previewLine.pivot = 
                (previewLine.startPoint + previewLine.endPoint) / 2;
            previewLineView.UpdateView(previewLine, graph.preferences, true);
            previewLineView.transform.SetParent(
                drawingView.GetGraphLayerParentTransform(graph.activeLayer));
            previewLineView = null;
            graph.content[graph.activeLayer].Add(previewLine);

            if (handlerDelegate != null) {
                Command cmd = new Command();
                cmd.type = Command.Type.CreateDrawable;
                cmd.layerIndex = graph.activeLayer;
                cmd.drawableIndex = graph.content[graph.activeLayer].Count - 1;
                cmd.currentDrawable = previewLine.Copy();
                handlerDelegate.OnChange(this, cmd);
            }

            previewLine = null;
        }

        public override void OnPaperCancelDrag() {
            // Clean up preview object if any.
            if (previewLineView != null)
                Object.Destroy(previewLineView.gameObject);
            
            previewLineView = null;
        }
    }
}
