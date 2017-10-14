using UnityEngine;

namespace Antigear.Graph {
    /// <summary>
    /// Manages the main interface, including drawing management, syncing, and
    /// settings.
    /// </summary>
    public class GraphController : MonoBehaviour {
        void Awake() {
            // Application specific settings go here.
            Application.targetFrameRate = 60;
        }
    }
}
