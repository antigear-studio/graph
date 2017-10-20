using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MaterialUI;

namespace Antigear.Graph {
    [ExecuteInEditMode]
    public class ToolbarView : MonoBehaviour {
        public bool showToolbar;
        public float toolbarHeight = 40;
        public float animationDuration = 0.5f;
        public CanvasGroup toolGroup;

        List<int> toolbarAnimationTweenIds = new List<int>();
        bool didShowToolbar;

        // Use this for initialization
        void Start() {
            didShowToolbar = showToolbar;
        }
	
        // Update is called once per frame
        void Update() {
            if (didShowToolbar != showToolbar) {
                SetToolbarVisibility(showToolbar, false);
            }
        }

        public void SetToolbarVisibility(bool shouldShow, bool animated) {
            if (toolbarAnimationTweenIds.Count > 0) {
                foreach (int id in toolbarAnimationTweenIds) {
                    TweenManager.EndTween(id);
                }

                toolbarAnimationTweenIds.Clear();
            }

            didShowToolbar = shouldShow;
            showToolbar = shouldShow;
            RectTransform r = transform as RectTransform;

            float targetValue = shouldShow ? 0 : -toolbarHeight;
            float targetAlpha = shouldShow ? 1 : 0;

            if (animated) {
                int t1 = TweenManager.TweenFloat(h => {
                    Vector2 pos = r.anchoredPosition;
                    pos.y = h;
                    r.anchoredPosition = pos;
                }, r.anchoredPosition.y, targetValue, animationDuration);
                int t2 = TweenManager.TweenFloat(a => toolGroup.alpha = a,
                    toolGroup.alpha, targetAlpha, animationDuration);
                toolbarAnimationTweenIds.Add(t1);
                toolbarAnimationTweenIds.Add(t2);
            } else {
                Vector2 pos = r.anchoredPosition;
                pos.y = targetValue;
                r.anchoredPosition = pos;
            }
        }
    }
}