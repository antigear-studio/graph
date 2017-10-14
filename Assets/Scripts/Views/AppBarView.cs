using MaterialUI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Antigear.Graph {
    /// <summary>
    /// Provides functionalities for the app bar view. This also includes tool
    /// handling. The app bar consists an upper bar and a lower tool bar.
    /// </summary>
    [ExecuteInEditMode]
    public class AppBarView : MonoBehaviour {
        /// <summary>
        /// Enumerates valid button types for the left button on the app bar.
        /// </summary>
        public enum LeftButtonType {
            NavigationButton,
            CloseButton
        }

        /// <summary>
        /// UI links.
        /// </summary>
        public UnityEngine.UI.Text titleText;
        public CanvasGroup toolbarCanvasGroup;
        public RectTransform appBarRectTransform;
        public MaterialShadow appBarMaterialShadow;
        public CanvasGroup closeButtonCanvasGroup;
        public CanvasGroup navigationButtonCanvasGroup;

        // Exposed properties.
        public IAppBarViewDelegate appBarViewDelegate;
        public string titleName = "";
        public bool showToolbar;
        public bool deepShadow;
        public LeftButtonType leftButton = LeftButtonType.NavigationButton;
        public float upperBarHeight = 56;
        public float toolbarHeight = 40;
        public float animationDuration = 0.5f;

        bool wasDeepShadow;
        bool didShowToolbar;
        LeftButtonType lastLeftButton;
        readonly List<int> toolbarAnimationTweenIds = new List<int>();
        readonly List<int> leftButtonAnimationTweenIds = new List<int>();

        void Start() {
            didShowToolbar = showToolbar;
            lastLeftButton = leftButton;
            wasDeepShadow = deepShadow;
        }

        void Update() {
            if (titleText != null)
                titleText.text = titleName;

            if (didShowToolbar != showToolbar) {
                SetToolbarVisibility(showToolbar, false);
            }

            if (lastLeftButton != leftButton) {
                SetLeftButton(leftButton, false);
            }

            if (wasDeepShadow != deepShadow) {
                SetShadowDepth(deepShadow, false);
            }
        }

        /// <summary>
        /// Sets the tool bar visibility.
        /// </summary>
        /// <param name="isVisible">If set to <c>true</c> tool bar will be
        /// visible.</param>
        /// <param name="animated">If set to <c>true</c> animates this
        /// transition.</param>
        public void SetToolbarVisibility(bool isVisible, bool animated) {
            if (isVisible == didShowToolbar)
                return;
            
            if (toolbarAnimationTweenIds.Count > 0) {
                // Cancel existing animations.
                foreach (int id in toolbarAnimationTweenIds) {
                    TweenManager.EndTween(id);
                }

                toolbarAnimationTweenIds.Clear();
            }

            showToolbar = isVisible;
            didShowToolbar = isVisible;
            toolbarCanvasGroup.interactable = isVisible;
            toolbarCanvasGroup.blocksRaycasts = isVisible;

            float startHeight = appBarRectTransform.sizeDelta.y;
            float targetHeight = isVisible ? upperBarHeight + toolbarHeight : 
                upperBarHeight;
            float startAlpha = toolbarCanvasGroup.alpha;
            float targetAlpha = isVisible ? 1 : 0;

            
            if (animated) {
                int t1 = TweenManager.TweenFloat(h => {
                    Vector2 size = appBarRectTransform.sizeDelta;
                    size.y = h;
                    appBarRectTransform.sizeDelta = size;
                }, startHeight, targetHeight, animationDuration);

                int t2 = TweenManager.TweenFloat(
                             a => toolbarCanvasGroup.alpha = a, startAlpha, 
                             targetAlpha, animationDuration);
                
                toolbarAnimationTweenIds.Add(t1);
                toolbarAnimationTweenIds.Add(t2);
            } else {
                Vector2 size = appBarRectTransform.sizeDelta;
                size.y = targetHeight;
                appBarRectTransform.sizeDelta = size;
                toolbarCanvasGroup.alpha = targetAlpha;
            }
        }

        /// <summary>
        /// Sets the shadow depth for the app bar. For drawing, the paper
        /// material is at a higher elevation than graph grid view, so the
        /// shadow should be shallower.
        /// </summary>
        /// <param name="isDeep">If set to <c>true</c> the shadow shows greater
        /// depth.</param>
        /// <param name="animated">If set to <c>true</c> animates this 
        /// transition.</param>
        public void SetShadowDepth(bool isDeep, bool animated) {
            if (isDeep == wasDeepShadow)
                return;

            wasDeepShadow = isDeep;
            deepShadow = isDeep;
            
            int shadowId = isDeep ? appBarMaterialShadow.shadowNormalSize
                : appBarMaterialShadow.shadowActiveSize;
            
            if (animated) {
                appBarMaterialShadow.SetShadows(shadowId);
            } else {
                appBarMaterialShadow.SetShadowsInstant(shadowId);
            }
        }

        /// <summary>
        /// Sets what the left button on the app bar should do.
        /// </summary>
        /// <param name="buttonType">Button type.</param>
        /// <param name="animated">If set to <c>true</c> animates this
        /// transition.</param>
        public void SetLeftButton(LeftButtonType buttonType, bool animated) {
            if (buttonType == lastLeftButton)
                return;

            if (leftButtonAnimationTweenIds.Count > 0) {
                // Cancel existing animations.
                foreach (int id in leftButtonAnimationTweenIds) {
                    TweenManager.EndTween(id);
                }

                leftButtonAnimationTweenIds.Clear();
            }

            bool closeButtonEnabled = buttonType == LeftButtonType.CloseButton;

            leftButton = buttonType;
            lastLeftButton = buttonType;
            closeButtonCanvasGroup.interactable = closeButtonEnabled;
            closeButtonCanvasGroup.blocksRaycasts = closeButtonEnabled;
            navigationButtonCanvasGroup.interactable = !closeButtonEnabled;
            navigationButtonCanvasGroup.blocksRaycasts = !closeButtonEnabled;

            Vector3 closeButtonTargetScale, navigationButtonTargetScale;

            if (closeButtonEnabled) {
                closeButtonTargetScale = new Vector3(1, 1, 1);
                navigationButtonTargetScale = new Vector3(0, 0, 1);
            } else {
                navigationButtonTargetScale = new Vector3(1, 1, 1);
                closeButtonTargetScale = new Vector3(0, 0, 1);
            }

            RectTransform closeButtonRectTransform = 
                closeButtonCanvasGroup.transform as RectTransform;
            RectTransform navigationButtonRectTransform = 
                navigationButtonCanvasGroup.transform as RectTransform;

            if (animated) {
                int t1 = 
                    TweenManager.TweenVector3(
                        v => closeButtonRectTransform.localScale = v, 
                        closeButtonRectTransform.localScale, 
                        closeButtonTargetScale, animationDuration);
                int t2 = 
                    TweenManager.TweenVector3(
                        v => navigationButtonRectTransform.localScale = v, 
                        navigationButtonRectTransform.localScale, 
                        navigationButtonTargetScale, animationDuration);

                leftButtonAnimationTweenIds.Add(t1);
                leftButtonAnimationTweenIds.Add(t2);
            } else {
                closeButtonRectTransform.localScale = closeButtonTargetScale;
                navigationButtonRectTransform.localScale = 
                    navigationButtonTargetScale;
            }
        }

        public void OnCloseButtonClick() {
            if (appBarViewDelegate != null) {
                appBarViewDelegate.OnCloseButtonClick(
                    closeButtonCanvasGroup.GetComponent<Button>());
            }
        }
    }

    /// <summary>
    /// Declares actions generated from the app bar that should be handled.
    /// </summary>
    public interface IAppBarViewDelegate {
        /// <summary>
        /// Raises the close button click event. Triggered when the close button
        /// located on the top left of the drawing mode screen is pressed.
        /// </summary>
        /// <param name="clickedButton">Clicked button.</param>
        void OnCloseButtonClick(Button clickedButton);
    }
}
