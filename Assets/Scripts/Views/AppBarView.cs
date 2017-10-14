using MaterialUI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Antigear.Graph {
    /// <summary>
    /// Provides functionalities for the app bar view. This also includes tool
    /// handling. The app bar consists an upper bar and a lower tool bar.
    /// </summary>
    [ExecuteInEditMode]
    public class AppBarView : MonoBehaviour {
        public UnityEngine.UI.Text titleText;
        public CanvasGroup toolbarCanvasGroup;
        public RectTransform appBarRectTransform;
        public MaterialShadow appBarMaterialShadow;

        // Exposed properties.
        public string titleName = "";
        public bool showToolbar;
        public float upperBarHeight = 56;
        public float toolbarHeight = 40;
        public float animationDuration = 0.3f;

        bool didShowToolbar;
        readonly List<int> toolbarAnimationTweenIds = new List<int>();

        void Start() {
            didShowToolbar = showToolbar;
        }

        void Update() {
            if (titleText != null)
                titleText.text = titleName;

            if (didShowToolbar != showToolbar) {
                setToolbarVisibility(showToolbar, false);
            }

            if (Input.GetKeyDown(KeyCode.Space)) {
                setToolbarVisibility(!showToolbar, true);
            }
        }

        /// <summary>
        /// Sets the tool bar visibility.
        /// </summary>
        /// <param name="isVisible">If set to <c>true</c> tool bar will be
        /// visible.</param>
        /// <param name="animated">If set to <c>true</c> animates this
        /// transition.</param>
        /// <param name="callback">Callback to call at the end of the
        /// transition.</param>
        public void setToolbarVisibility(bool isVisible, bool animated, 
                                         Action callback = null) {
            if (toolbarAnimationTweenIds.Count > 0) {
                // Cancel existing animations.
                foreach (int id in toolbarAnimationTweenIds) {
                    TweenManager.EndTween(id);
                }

                toolbarAnimationTweenIds.Clear();
            }

            showToolbar = isVisible;
            didShowToolbar = isVisible;

            float startHeight = appBarRectTransform.sizeDelta.y;
            float targetHeight = isVisible ? upperBarHeight + toolbarHeight : 
                upperBarHeight;
            float startAlpha = toolbarCanvasGroup.alpha;
            float targetAlpha = isVisible ? 1 : 0;
            int shadowId = isVisible ? appBarMaterialShadow.shadowActiveSize
                : appBarMaterialShadow.shadowNormalSize;
            
            if (animated) {
                int t1 = TweenManager.TweenFloat(h => {
                    Vector2 size = appBarRectTransform.sizeDelta;
                    size.y = h;
                    appBarRectTransform.sizeDelta = size;
                }, startHeight, targetHeight, animationDuration, 0, callback);

                int t2 = 
                    TweenManager.TweenFloat(a => toolbarCanvasGroup.alpha = a, 
                                            startAlpha, targetAlpha, 
                                            animationDuration, 0);

                appBarMaterialShadow.SetShadows(shadowId);

                toolbarAnimationTweenIds.Add(t1);
                toolbarAnimationTweenIds.Add(t2);
            } else {
                Vector2 size = appBarRectTransform.sizeDelta;
                size.y = targetHeight;
                appBarRectTransform.sizeDelta = size;
                toolbarCanvasGroup.alpha = targetAlpha;
                appBarMaterialShadow.SetShadowsInstant(shadowId);

                if (callback != null)
                    callback();
            }
        }
    }
}