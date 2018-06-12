using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Antigear.Graph {
    public class ConstantScreenResizer : MonoBehaviour {
        public float width;
        public float height;
        RectTransform r;

        void Start() {
            r = transform as RectTransform;
        }

        // Update is called once per frame
        void Update() {
            r.sizeDelta = new Vector2(width / r.lossyScale.x, 
                height / r.lossyScale.x);
        }
    }
}