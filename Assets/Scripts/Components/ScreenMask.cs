using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Antigear.Graph {
    /// <summary>
    /// Moves and resizes the attached transform so it always cover the whole
    /// screen.
    /// </summary>
    public class ScreenMask : MonoBehaviour {
        RectTransform r;

        // Use this for initialization
        void Start() {
            r = transform as RectTransform;
        }

        // Update is called once per frame
        void Update() {
            r.sizeDelta = 
                new Vector2(Screen.width, Screen.height) / r.lossyScale.x;
            Vector3 pos = r.position;
            pos.x = 0;
            pos.y = 0;
            r.position = pos;
        }
    }
}