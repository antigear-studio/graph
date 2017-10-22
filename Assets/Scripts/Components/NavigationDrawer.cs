using MaterialUI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Antigear.Graph {
    /// <summary>
    /// An UI view behavior that allows the attached view to pop out from the
    /// edge of the screen, dragged and manipulated and dismissed through
    /// swiping. This class should be subclassed to actually manage the content
    /// displayed inside it.
    /// </summary>
    public class NavigationDrawer : MonoBehaviour, IBeginDragHandler,  
    IDragHandler, IEndDragHandler {

        /// <summary>
        /// The overlaying shadow.
        /// </summary>
        public Image backgroundImage;
        public RectTransform panelTransform;
        public CanvasGroup navDrawerCanvasGroup;
        public float animationDuration = 0.4f;

        Vector2 dragStartPos;
        bool isDraggingSheet;

        protected const float MAX_OVERDRAG = 96;
        protected const float DECAY_COEFF = 0.2f;
        protected const float MAX_MASK_STRENGTH = 0.5f;

        protected readonly List<int> navDrawerAnimationIds = new List<int>();

        /// <summary>
        /// The root MaterialUIScaler.
        /// </summary>
        MaterialUIScaler m_Scaler;
        /// <summary>
        /// The root MaterialUIScaler.
        /// If null, gets the root scaler if one exists.
        /// </summary>
        public MaterialUIScaler scaler {
            get {
                if (m_Scaler == null) {
                    m_Scaler = MaterialUIScaler.GetParentScaler(transform);
                }

                return m_Scaler;
            }
        }

        void OnRectTransformDimensionsChange() {
            float screenWidth = (transform as RectTransform).rect.width;
            float panelWidth = Mathf.Min(280, screenWidth - 56) + MAX_OVERDRAG;
            float oldWidth = panelTransform.rect.width;

            if (UIUtil.IsTablet()) {
                panelWidth = Mathf.Min(320, screenWidth - 64) + MAX_OVERDRAG;
            } 

            Vector2 size = panelTransform.sizeDelta;
            size.x = panelWidth;
            panelTransform.sizeDelta = size;

            if (panelTransform.gameObject.activeInHierarchy) {
                Vector2 pos = panelTransform.anchoredPosition;
                pos.x += panelWidth - oldWidth;
                panelTransform.anchoredPosition = pos;
            }
        }

        public virtual void Show(bool animated, Action onCompletion = null) {
            navDrawerCanvasGroup.interactable = true;
            if (navDrawerAnimationIds.Count > 0) {
                foreach (int id in navDrawerAnimationIds) {
                    TweenManager.EndTween(id);
                }

                navDrawerAnimationIds.Clear();
            }

            panelTransform.gameObject.SetActive(true);
            backgroundImage.gameObject.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(panelTransform);
            float targetWidth = panelTransform.rect.width - MAX_OVERDRAG;
            Color targetColor = new Color(0, 0, 0, MAX_MASK_STRENGTH);

            if (animated) {
                int t1 = TweenManager.TweenFloat(v => {
                    Vector2 pos = panelTransform.anchoredPosition;
                    pos.x = v;
                    panelTransform.anchoredPosition = pos;
                }, panelTransform.anchoredPosition.x, targetWidth, 
                    animationDuration, 0, onCompletion);
                int t2 = 
                    TweenManager.TweenColor(c => backgroundImage.color = c, 
                        backgroundImage.color, targetColor, animationDuration);
                navDrawerAnimationIds.Add(t1);
                navDrawerAnimationIds.Add(t2);
            } else {
                Vector2 pos = panelTransform.anchoredPosition;
                pos.x = targetWidth;
                panelTransform.anchoredPosition = pos;
                backgroundImage.color = targetColor;

                if (onCompletion != null)
                    onCompletion();
            }
        }

        public void OnBackgroundPress() {
            Dismiss(true, null, 0);
        }

        public virtual void Dismiss(bool animated, Action onCompletion = null, 
            float delay = 0) {
            navDrawerCanvasGroup.interactable = false;

            if (navDrawerAnimationIds.Count > 0) {
                foreach (int id in navDrawerAnimationIds) {
                    TweenManager.EndTween(id);
                }

                navDrawerAnimationIds.Clear();
            }

            const float targetWidth = 0;
            Color targetColor = Color.clear;

            if (animated) {
                int t1 = TweenManager.TweenFloat(v => {
                    Vector2 pos = panelTransform.anchoredPosition;
                    pos.x = v;
                    panelTransform.anchoredPosition = pos;
                }, panelTransform.anchoredPosition.x, targetWidth, 
                             animationDuration, delay, () => {
                    panelTransform.gameObject.SetActive(false);
                    backgroundImage.gameObject.SetActive(false);
                });
                int t2 = 
                    TweenManager.TweenColor(c => backgroundImage.color = c, 
                        backgroundImage.color, targetColor, animationDuration,
                        delay, onCompletion);
                navDrawerAnimationIds.Add(t1);
                navDrawerAnimationIds.Add(t2);
            } else {
                Vector2 pos = panelTransform.anchoredPosition;
                pos.x = targetWidth;
                panelTransform.anchoredPosition = pos;
                backgroundImage.color = targetColor;
                panelTransform.gameObject.SetActive(false);
                backgroundImage.gameObject.SetActive(false);

                if (onCompletion != null)
                    onCompletion();
            }
        }

        /// <summary>
        /// Called when [begin drag].
        /// </summary>
        /// <param name="data">The data.</param>
        public virtual void OnBeginDrag(PointerEventData data) {
            dragStartPos = data.position;
            Vector3[] corners = new Vector3[4];
            panelTransform.GetWorldCorners(corners);
            Rect rect = new Rect(corners[0], corners[2] - corners[0]);
            isDraggingSheet = rect.Contains(data.position);
        }

        /// <summary>
        /// Called when [drag].
        /// </summary>
        /// <param name="data">The data.</param>
        public virtual void OnDrag(PointerEventData data) {
            // Update position on panel based on the data. There's also a
            // max amount we can over drag (exp decay).
            if (isDraggingSheet) {
                // Max height is the height of the panel minus the extra slot
                // used to guard against over drag.
                float maxWidth = panelTransform.rect.width - MAX_OVERDRAG;

                Vector2 pos = panelTransform.anchoredPosition;
                float dx = data.delta.x / scaler.scaleFactor;
                float overdrag = Mathf.Max(0, pos.x - maxWidth);
                pos.x += dx * Mathf.Exp(-DECAY_COEFF * overdrag);
                panelTransform.anchoredPosition = pos;
            }
        }

        /// <summary>
        /// Called when [end drag].
        /// </summary>
        /// <param name="data">The data.</param>
        public virtual void OnEndDrag(PointerEventData data) {
            // Determine if we want to close or expand.
            if (isDraggingSheet) {
                float delta = data.position.x - dragStartPos.x;

                if (delta < 0) {
                    Dismiss(true);
                } else {
                    Show(true);
                }
            }
        }
    }
}
