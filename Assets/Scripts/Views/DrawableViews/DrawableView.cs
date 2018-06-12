using UnityEngine;

namespace Antigear.Graph {
    public abstract class DrawableView : MonoBehaviour,
    IEditingControlDelegate {
        // Outlets
        public DrawableViewDelegate viewDelegate;
        public EditingControl pivotControl;

        protected RectTransform rectTransform;

        public virtual void UpdateView(Drawable drawable, 
            Graph.Preference drawingPreferences, bool animated) {
            pivotControl.gameObject.SetActive(drawable.isEditing);

            // Update pivot, size and position.
            Rect bounds = drawable.GetBoundary();
            rectTransform.pivot = drawable.pivot;
            rectTransform.sizeDelta = bounds.size;
            rectTransform.anchoredPosition =
                bounds.position + Vector2.Scale(bounds.size, drawable.pivot);

            // Update pivot control.
            pivotControl.UpdateView(drawingPreferences);
            pivotControl.graphicsRoot.anchoredPosition = Vector2.Scale(
                bounds.size, drawable.pivot - new Vector2(0.5f, 0.5f));
        }

        #region IEditingControlDelegate implementation

        public virtual void OnEditingControlBeginDrag(
            EditingControl editingControl, Vector2 pos, Vector2 screenPos) {
            if (viewDelegate != null) {
                if (editingControl == pivotControl) {
                    viewDelegate.OnPivotControlBeginDrag(this, Local2Paper(pos), 
                        screenPos);
                }
            }
        }

        public virtual void OnEditingControlDrag(EditingControl editingControl,
            Vector2 pos, Vector2 screenPos) {
            if (viewDelegate != null) {
                if (editingControl == pivotControl) {
                    viewDelegate.OnPivotControlDrag(this, Local2Paper(pos), 
                        screenPos);
                }
            }
        }

        public virtual void OnEditingControlEndDrag(
            EditingControl editingControl, Vector2 pos, Vector2 screenPos) {
            if (viewDelegate != null) {
                if (editingControl == pivotControl) {
                    viewDelegate.OnPivotControlEndDrag(this, Local2Paper(pos), 
                        screenPos);
                }
            }
        }

        #endregion

        void Awake() {
            rectTransform = transform as RectTransform;
        }

        protected virtual void Start() {
            pivotControl.controlDelegate = this;
        }

        protected Vector2 Paper2Local(Vector2 pos) {
            // Need to handle rotation in the future.
            return pos - rectTransform.anchoredPosition + Vector2.Scale(rectTransform.sizeDelta, rectTransform.pivot - new Vector2(0.5f, 0.5f));
        }

        protected Vector2 Local2Paper(Vector2 pos) {
            // Need to handle rotation in the future.
            return pos + rectTransform.anchoredPosition - Vector2.Scale(rectTransform.sizeDelta, rectTransform.pivot - new Vector2(0.5f, 0.5f));
        }
    }

    public interface DrawableViewDelegate {
        /// <summary>
        /// Raises when the pivot point is beginning to be dragged.
        /// </summary>
        /// <param name="view">View.</param>
        /// <param name="pos">Position in paper coordinates.</param>
        /// <param name="screenPos">Screen position.</param>
        void OnPivotControlBeginDrag(DrawableView view, Vector2 pos, 
            Vector2 screenPos);

        /// <summary>
        /// Raises when the pivot point is being dragged.
        /// </summary>
        /// <param name="view">View.</param>
        /// <param name="pos">Position in paper coordinates.</param>
        /// <param name="screenPos">Screen position.</param>
        void OnPivotControlDrag(DrawableView view, Vector2 pos,
            Vector2 screenPos);

        /// <summary>
        /// Raises when the pivot point has stopped being dragged.
        /// </summary>
        /// <param name="view">View.</param>
        /// <param name="pos">Position in paper coordinates.</param>
        /// <param name="screenPos">Screen position.</param>
        void OnPivotControlEndDrag(DrawableView view, Vector2 pos,
            Vector2 screenPos);
        
    }
}
