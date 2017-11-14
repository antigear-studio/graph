using UnityEngine;

namespace Antigear.Graph {
    public abstract class DrawableView : MonoBehaviour {
        // Outlets
        public RectTransform pivotTransform;
        public DrawableViewDelegate viewDelegate;

        public virtual void UpdateView(Drawable drawable, 
            Graph.Preference drawingPreferences, bool animated) {
            pivotTransform.gameObject.SetActive(drawable.isEditing);

            if (drawable.isEditing) {
                pivotTransform.anchoredPosition = drawable.rotationPivot;
            }
        }

        protected void Start() {
        }
    }

    public interface DrawableViewDelegate {
        
    }
}
