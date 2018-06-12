using MaterialUI;
using UnityEngine;

namespace Antigear.Graph {
    /// <summary>
    /// Handles straight line editing.
    /// </summary>
    public class StraightLineEditHandler : LineEditHandler, 
    IStraightLineViewDelegate {
        #region IStraightLineViewDelegate implementation

        public void OnBeginControlBeginDrag(StraightLineView view, Vector2 pos,
            Vector2 screenPos) {
            BeginModification();
        }

        public void OnBeginControlDrag(StraightLineView view, Vector2 pos,
            Vector2 screenPos) {
            StraightLine line = editing as StraightLine;

            if (line != null) {
                line.startPoint = pos;
            }

            view.UpdateView(line, graph.preferences, false);
        }

        public void OnBeginControlEndDrag(StraightLineView view, Vector2 pos,
            Vector2 screenPos) {
            EndModification();
        }

        public void OnEndControlBeginDrag(StraightLineView view, Vector2 pos,
            Vector2 screenPos) {
            BeginModification();
        }

        public void OnEndControlDrag(StraightLineView view, Vector2 pos,
            Vector2 screenPos) {
            StraightLine line = editing as StraightLine;

            if (line != null) {
                line.endPoint = pos;
            }

            view.UpdateView(line, graph.preferences, false);
        }

        public void OnEndControlEndDrag(StraightLineView view, Vector2 pos,
            Vector2 screenPos) {
            EndModification();
        }

        #endregion
    }
}
