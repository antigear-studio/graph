using UnityEngine;
using Vectrosity;

namespace Antigear.Graph {
    /// <summary>
    /// Manages and displays a straight line.
    /// </summary>
    public class StraightLineView : LineView, IEditingControlDelegate {
        // Outlets
        public EditingControl beginControl;
        public EditingControl endControl;

        public override void UpdateView(Drawable drawable, 
            Graph.Preference drawingPreferences, bool animated) {
            base.UpdateView(drawable, drawingPreferences, animated);

            StraightLine line = drawable as StraightLine;

            if (line == null) {
                Debug.LogError("Cannot update view with incompatible model!");
                return;
            }

            // Update the two points.
            beginControl.UpdateView(drawingPreferences);
            endControl.UpdateView(drawingPreferences);

            if (drawable.isEditing) {
                beginControl.graphicsRoot.anchoredPosition = 
                    Paper2Local(line.startPoint);
                endControl.graphicsRoot.anchoredPosition = 
                    Paper2Local(line.endPoint);
            }

            beginControl.gameObject.SetActive(drawable.isEditing);
            endControl.gameObject.SetActive(drawable.isEditing);
        }

        #region IEditingControlDelegate implementation

        public override void OnEditingControlBeginDrag(
            EditingControl editingControl, Vector2 pos, Vector2 screenPos) {
            base.OnEditingControlBeginDrag(editingControl, pos, screenPos);

            IStraightLineViewDelegate del = 
                viewDelegate as IStraightLineViewDelegate;
            
            if (del != null) {
                if (editingControl == beginControl) {
                    del.OnBeginControlBeginDrag(this, Local2Paper(pos), 
                        screenPos);
                } else if (editingControl == endControl) {
                    del.OnEndControlBeginDrag(this, Local2Paper(pos), 
                        screenPos);
                }
            }
        }

        public override void OnEditingControlDrag(EditingControl editingControl,
            Vector2 pos, Vector2 screenPos) {
            base.OnEditingControlDrag(editingControl, pos, screenPos);

            IStraightLineViewDelegate del = 
                viewDelegate as IStraightLineViewDelegate;
            
            if (del != null) {
                if (editingControl == beginControl) {
                    del.OnBeginControlDrag(this, Local2Paper(pos), screenPos);
                } else if (editingControl == endControl) {
                    del.OnEndControlDrag(this, Local2Paper(pos), screenPos);
                }
            }
        }

        public override void OnEditingControlEndDrag(
            EditingControl editingControl, Vector2 pos, Vector2 screenPos) {
            base.OnEditingControlEndDrag(editingControl, pos, screenPos);

            IStraightLineViewDelegate del = 
                viewDelegate as IStraightLineViewDelegate;
            
            if (del != null) {
                if (editingControl == beginControl) {
                    del.OnBeginControlEndDrag(this, Local2Paper(pos),
                        screenPos);
                } else if (editingControl == endControl) {
                    del.OnEndControlEndDrag(this, Local2Paper(pos), screenPos);
                }
            }
        }

        #endregion

        protected override void Start() {
            base.Start();
            beginControl.controlDelegate = this;
            endControl.controlDelegate = this;
        }
    }

    public interface IStraightLineViewDelegate : LineViewDelegate {
        /// <summary>
        /// Raises when the line begin point is beginning to be dragged.
        /// </summary>
        /// <param name="view">View.</param>
        /// <param name="pos">Position.</param>
        /// <param name="screenPos">Screen position.</param>
        void OnBeginControlBeginDrag(StraightLineView view, Vector2 pos, 
            Vector2 screenPos);

        /// <summary>
        /// Raises when the line begin point is being dragged.
        /// </summary>
        /// <param name="view">View.</param>
        /// <param name="pos">Position.</param>
        /// <param name="screenPos">Screen position.</param>
        void OnBeginControlDrag(StraightLineView view, Vector2 pos,
            Vector2 screenPos);

        /// <summary>
        /// Raises when the line begin point has stopped being dragged.
        /// </summary>
        /// <param name="view">View.</param>
        /// <param name="pos">Position.</param>
        /// <param name="screenPos">Screen position.</param>
        void OnBeginControlEndDrag(StraightLineView view, Vector2 pos,
            Vector2 screenPos);
        
        /// <summary>
        /// Raises when the line end point is beginning to be dragged.
        /// </summary>
        /// <param name="view">View.</param>
        /// <param name="pos">Position.</param>
        /// <param name="screenPos">Screen position.</param>
        void OnEndControlBeginDrag(StraightLineView view, Vector2 pos, 
            Vector2 screenPos);

        /// <summary>
        /// Raises when the line end point is being dragged.
        /// </summary>
        /// <param name="view">View.</param>
        /// <param name="pos">Position.</param>
        /// <param name="screenPos">Screen position.</param>
        void OnEndControlDrag(StraightLineView view, Vector2 pos,
            Vector2 screenPos);

        /// <summary>
        /// Raises when the line end point has stopped being dragged.
        /// </summary>
        /// <param name="view">View.</param>
        /// <param name="pos">Position.</param>
        /// <param name="screenPos">Screen position.</param>
        void OnEndControlEndDrag(StraightLineView view, Vector2 pos,
            Vector2 screenPos);
    }
}
