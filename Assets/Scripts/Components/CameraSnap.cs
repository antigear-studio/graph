using UnityEngine;

namespace Antigear.Graph {
    [ExecuteInEditMode]
    public class CameraSnap : MonoBehaviour {
    
        // Update is called once per frame
        void Update() {
            // Make sure that canvas is at scale of 1 and position camera such
            // that it is at the same position as overlay.
            transform.position = new Vector2(Screen.width, Screen.height) / 2f;
            Camera.main.orthographicSize = Screen.height / 2f;
        }
    }
}