using System;
using UnityEngine;

namespace Antigear.Graph {
    public class StraightLineHandler : ToolHandler {
        Vector2 straightLineBeginPosition;

        StraightLine previewLine;
        StraightLineView prefab;

        public override void SetupToolHandler(Graph graph, 
            DrawingView drawingView) {
            base.SetupToolHandler(graph, drawingView);
            prefab = (StraightLineView)Resources.Load("StraightLinePrefab", 
                typeof(StraightLineView));
        }

        public override void OnPaperBeginDrag(Vector2 pos, Vector2 screenPos) {
            straightLineBeginPosition = pos;

            // Create a line for preview.
            previewLine = new StraightLine();

            // Create a prefab and set the line as its model.
            StraightLineView view = UnityEngine.Object.Instantiate(prefab);

            // Add the prefab to the scene.
        }

        public override void OnPaperDrag(Vector2 pos, Vector2 screenPos) {
            // Update the model, thus the view as well.

        }

        public override void OnPaperEndDrag(Vector2 pos, Vector2 screenPos) {
            // Add the line to graph. Remove preview object.
        }

        public override void OnPaperCancelDrag() {
            // Clean up preview object if any.
        }
    }
}

