using UnityEngine;

namespace Antigear.Graph {
    /// <summary>
    /// Controls how a layer behaves on canvas.
    /// </summary>
    public class LayerView : MonoBehaviour {
        public CanvasGroup layerGroup;

        /// <summary>
        /// This updates the layer's display info. Does not modify the actual
        /// drawing objects.
        /// </summary>
        /// <param name="layer">Layer model to draw info on.</param>
        public void UpdateLayer(Layer layer) {
            name = layer.name;
            layerGroup.alpha = layer.alpha;
            layerGroup.interactable = layer.interactive;
            gameObject.SetActive(layer.visible);
        }
    }
}
