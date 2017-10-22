using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Antigear.Graph {
    /// <summary>
    /// Resizes the target element at start depending on phone/tablet, using the 
    /// given RectTransforms.
    /// </summary>
    public class Resizer : MonoBehaviour {
        public RectTransform target;
        public RectTransform phoneRect;
        public RectTransform tabletRect;

        void Start() {
            Resize();
        }

        public void Resize() {
            if (target == null)
                return;

            if (UIUtil.IsTablet()) {
                CopyOver(tabletRect, target);
            } else {
                CopyOver(phoneRect, target);
            }
        }

        void CopyOver(RectTransform s, RectTransform t) {
            t.anchorMin = s.anchorMin;
            t.anchorMax = s.anchorMax;
            t.pivot = s.pivot;
            t.anchoredPosition = s.anchoredPosition;
            t.localRotation = s.localRotation;
            t.sizeDelta = s.sizeDelta;
        }
    }
}