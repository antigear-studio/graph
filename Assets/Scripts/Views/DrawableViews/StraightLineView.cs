using UnityEngine;
using Vectrosity;

namespace Antigear.Graph {
    /// <summary>
    /// Manages and displays a straight line.
    /// </summary>
    public class StraightLineView : LineView, IEditingControlDelegate {
        // Outlets
        public RectTransform beginGraphicTransform;
        public RectTransform endGraphicTransform;
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
            if (drawable.isEditing) {
                beginGraphicTransform.anchoredPosition = line.startPoint;
                endGraphicTransform.anchoredPosition = line.endPoint;
            }

            beginControl.gameObject.SetActive(drawable.isEditing);
            endControl.gameObject.SetActive(drawable.isEditing);
        }

        #region IEditingControlDelegate implementation

        public void OnEditingControlBeginDrag(EditingControl editingControl,
            Vector2 pos, Vector2 screenPos) {
            IStraightLineViewDelegate del = 
                viewDelegate as IStraightLineViewDelegate;
            
            if (del != null) {
                if (editingControl == beginControl) {
                    del.OnBeginControlBeginDrag(this, pos, screenPos);
                } else if (editingControl == endControl) {
                    del.OnEndControlBeginDrag(this, pos, screenPos);
                }
            }
        }

        public void OnEditingControlDrag(EditingControl editingControl,
            Vector2 pos, Vector2 screenPos) {
            IStraightLineViewDelegate del = 
                viewDelegate as IStraightLineViewDelegate;
            
            if (del != null) {
                if (editingControl == beginControl) {
                    del.OnBeginControlDrag(this, pos, screenPos);
                } else if (editingControl == endControl) {
                    del.OnEndControlDrag(this, pos, screenPos);
                }
            }
        }

        public void OnEditingControlEndDrag(EditingControl editingControl,
            Vector2 pos, Vector2 screenPos) {
            IStraightLineViewDelegate del = 
                viewDelegate as IStraightLineViewDelegate;
            
            if (del != null) {
                if (editingControl == beginControl) {
                    del.OnBeginControlEndDrag(this, pos, screenPos);
                } else if (editingControl == endControl) {
                    del.OnEndControlEndDrag(this, pos, screenPos);
                }
            }
        }

        #endregion

        protected void Start() {
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
