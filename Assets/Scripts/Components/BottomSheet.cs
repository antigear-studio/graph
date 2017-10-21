using MaterialUI;
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
    public class BottomSheet : MonoBehaviour, IBeginDragHandler, IDragHandler, 
    IEndDragHandler {

        /// <summary>
        /// The overlaying shadow.
        /// </summary>
        public Image backgroundImage;
        public RectTransform panelTransform;
        public Image shadow;
        public float animationDuration = 0.4f;

        Vector2 dragStartPos;
        bool isDraggingSheet;

        const float MAX_OVERDRAG = 96;
        const float DECAY_COEFF = 0.2f;
        const float MAX_MASK_STRENGTH = 0.25f;

        readonly List<int> bottomSheetAnimationIds = new List<int>();

        public void Show(bool animated) {
            if (bottomSheetAnimationIds.Count > 0) {
                foreach (int id in bottomSheetAnimationIds) {
                    TweenManager.EndTween(id);
                }

                bottomSheetAnimationIds.Clear();
            }

            panelTransform.gameObject.SetActive(true);
            backgroundImage.gameObject.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(panelTransform);
            float targetHeight = panelTransform.rect.height - MAX_OVERDRAG;
            Color targetColor = new Color(0, 0, 0, MAX_MASK_STRENGTH);

            if (animated) {
                int t1 = TweenManager.TweenFloat(v => {
                    Vector2 pos = panelTransform.anchoredPosition;
                    pos.y = v;
                    panelTransform.anchoredPosition = pos;
                }, panelTransform.anchoredPosition.y, targetHeight, 
                             animationDuration);
                int t2 = 
                    TweenManager.TweenColor(c => backgroundImage.color = c, 
                        backgroundImage.color, targetColor, animationDuration);
                bottomSheetAnimationIds.Add(t1);
                bottomSheetAnimationIds.Add(t2);
            } else {
                Vector2 pos = panelTransform.anchoredPosition;
                pos.y = targetHeight;
                panelTransform.anchoredPosition = pos;
                backgroundImage.color = targetColor;
            }
        }

        public void Dismiss(bool animated) {
            if (bottomSheetAnimationIds.Count > 0) {
                foreach (int id in bottomSheetAnimationIds) {
                    TweenManager.EndTween(id);
                }

                bottomSheetAnimationIds.Clear();
            }

            const float targetHeight = 0;
            Color targetColor = Color.clear;

            if (animated) {
                int t1 = TweenManager.TweenFloat(v => {
                    Vector2 pos = panelTransform.anchoredPosition;
                    pos.y = v;
                    panelTransform.anchoredPosition = pos;
                }, panelTransform.anchoredPosition.y, targetHeight, 
                             animationDuration, 0, () => {
                    panelTransform.gameObject.SetActive(false);
                    backgroundImage.gameObject.SetActive(false);
                });
                int t2 = 
                    TweenManager.TweenColor(c => backgroundImage.color = c, 
                        backgroundImage.color, targetColor, animationDuration);
                bottomSheetAnimationIds.Add(t1);
                bottomSheetAnimationIds.Add(t2);
            } else {
                Vector2 pos = panelTransform.anchoredPosition;
                pos.y = targetHeight;
                panelTransform.anchoredPosition = pos;
                backgroundImage.color = targetColor;
                panelTransform.gameObject.SetActive(false);
                backgroundImage.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Called when [begin drag].
        /// </summary>
        /// <param name="data">The data.</param>
        public void OnBeginDrag(PointerEventData data) {
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
        public void OnDrag(PointerEventData data) {
            // Update position on panel based on the data. There's also a
            // max amount we can over drag (exp decay).
            if (isDraggingSheet) {
                // Max height is the height of the panel minus the extra slot
                // used to guard against over drag.
                float maxHeight = panelTransform.rect.height - MAX_OVERDRAG;

                Vector2 pos = panelTransform.anchoredPosition;
                float overdrag = Mathf.Max(0, pos.y - maxHeight);
                pos.y += data.delta.y * Mathf.Exp(-DECAY_COEFF * overdrag);
                panelTransform.anchoredPosition = pos;
            }
        }

        /// <summary>
        /// Called when [end drag].
        /// </summary>
        /// <param name="data">The data.</param>
        public void OnEndDrag(PointerEventData data) {
            // Determine if we want to close or expand.
            if (isDraggingSheet) {
                float delta = data.position.y - dragStartPos.y;

                if (delta < 0) {
                    Dismiss(true);
                } else {
                    Show(true);
                }
            }
        }
    }
}
