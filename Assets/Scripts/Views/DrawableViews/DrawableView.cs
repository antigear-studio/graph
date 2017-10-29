using UnityEngine;

namespace Antigear.Graph {
    public abstract class DrawableView : MonoBehaviour {
        public virtual void UpdateView(Drawable drawable, 
            Graph.Preference drawingPreferences, bool animated) {}
    }
}
