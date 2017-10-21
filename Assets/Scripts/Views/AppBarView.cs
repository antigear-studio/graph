using MaterialUI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Antigear.Graph {
    /// <summary>
    /// Provides functionalities for the app bar view.
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
        public RectTransform barBackgroundRectTransform;

        public MaterialButton closeButton;
        public MaterialButton navigationButton;
        public MaterialButton moreButton;


        // Exposed properties.
        public IAppBarViewDelegate viewDelegate;
        public string titleName = "";
        public bool isMinimized;
        public LeftButtonType leftButton = LeftButtonType.NavigationButton;
        public float animationDuration = 0.5f;
        public float minimizedBarBackgroundOffset = 60;
        public Color drawingViewButtonColor = Color.black;

        CanvasGroup closeButtonCanvasGroup;
        CanvasGroup navigationButtonCanvasGroup;
        CanvasGroup moreButtonCanvasGroup;

        bool wasMinimized;
        LeftButtonType lastLeftButton;
        readonly List<int> appBarAnimationTweenIds = new List<int>();
        readonly List<int> leftButtonAnimationTweenIds = new List<int>();

        void Start() {
            wasMinimized = isMinimized;
            lastLeftButton = leftButton;
            closeButtonCanvasGroup = closeButton.GetComponent<CanvasGroup>();
            navigationButtonCanvasGroup = 
                navigationButton.GetComponent<CanvasGroup>();
            moreButtonCanvasGroup = moreButton.GetComponent<CanvasGroup>();
        }

        void Update() {
            if (titleText != null)
                titleText.text = titleName;

            if (wasMinimized != isMinimized) {
                SetMinimized(isMinimized, false);
            }

            if (lastLeftButton != leftButton) {
                SetLeftButton(leftButton, false);
            }
        }

        /// <summary>
        /// Sets the style of the app bar. Minimized hides solid color chunk and
        /// only shows the two buttons.
        /// </summary>
        /// <param name="minimized">If set to <c>true</c> app bar will be
        /// minimized.</param>
        /// <param name="animated">If set to <c>true</c> animates this
        /// transition.</param>
        public void SetMinimized(bool minimized, bool animated) {
            if (minimized == wasMinimized)
                return;

            if (appBarAnimationTweenIds.Count > 0) {
                // Cancel existing animations.
                foreach (int id in appBarAnimationTweenIds) {
                    TweenManager.EndTween(id);
                }

                appBarAnimationTweenIds.Clear();
            }

            isMinimized = minimized;
            wasMinimized = minimized;

            float startPos = barBackgroundRectTransform.anchoredPosition.y;
            float targetPos = minimized ? minimizedBarBackgroundOffset : 0;
            Color targetColor = 
                isMinimized ? drawingViewButtonColor : Color.white;

            if (animated) {
                int t1 = TweenManager.TweenFloat(h => {
                    Vector2 pos = barBackgroundRectTransform.anchoredPosition;
                    pos.y = h;
                    barBackgroundRectTransform.anchoredPosition = pos;
                }, startPos, targetPos, animationDuration);
                int t2 = TweenManager.TweenColor(c => closeButton.iconColor = c, 
                    closeButton.iconColor, targetColor, animationDuration);
                int t3 = TweenManager.TweenColor(
                    c => navigationButton.iconColor = c, 
                    navigationButton.iconColor, targetColor, animationDuration);
                int t4 = TweenManager.TweenColor(c => moreButton.iconColor = c, 
                    moreButton.iconColor, targetColor, animationDuration);
                
                appBarAnimationTweenIds.Add(t1);
                appBarAnimationTweenIds.Add(t2);
                appBarAnimationTweenIds.Add(t3);
                appBarAnimationTweenIds.Add(t4);
            } else {
                Vector2 pos = barBackgroundRectTransform.anchoredPosition;
                pos.y = targetPos;
                barBackgroundRectTransform.anchoredPosition = pos;
                closeButton.iconColor = targetColor;
                navigationButton.iconColor = targetColor;
                moreButton.iconColor = targetColor;
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
            if (viewDelegate != null) {
                viewDelegate.OnCloseButtonClick(
                    closeButtonCanvasGroup.GetComponent<Button>());
            }
        }

        public void OnNavigationButtonClick() {
            if (viewDelegate != null) {
                viewDelegate.OnNavigationButtonClick(
                    navigationButtonCanvasGroup.GetComponent<Button>());
            }
        }

        public void OnMoreButtonClick() {
            // TODO - attach button
            if (viewDelegate != null) {
                viewDelegate.OnMoreButtonClick(null);
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

        /// <summary>
        /// Raises the navigation button click event. Triggered when the 
        /// navigation button located on the top left of the graph selection
        /// mode screen is pressed.
        /// </summary>
        /// <param name="clickedButton">Clicked button.</param>
        void OnNavigationButtonClick(Button clickedButton);

        /// <summary>
        /// Raises the more button click event. Triggered when the more button
        /// located on the top right of the screen is pressed.
        /// </summary>
        /// <param name="clickedButton">Clicked button.</param>
        void OnMoreButtonClick(Button clickedButton);
    }
}
