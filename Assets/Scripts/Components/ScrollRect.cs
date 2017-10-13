using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Antigear.Graph {
    /// <summary>
    /// Standard Unity scroll rect, except also controls scroll bar opacity.
    /// </summary>
    public class ScrollRect : UnityEngine.UI.ScrollRect {
        Coroutine fadeHorizontal, fadeVertical;
        Image handleHorizontal, handleVertical;

        Color targetColor = new Color(0, 0, 0, 0.25f);
        public float fadeDuration = 0.15f;

        protected override void Start() {
            base.Start();

            if (horizontalScrollbar != null) {
                handleHorizontal = 
                    horizontalScrollbar.handleRect.GetComponent<Image>();
                handleHorizontal.color = Color.clear;
            }

            if (verticalScrollbar != null) {
                handleVertical = 
                    verticalScrollbar.handleRect.GetComponent<Image>();
                handleVertical.color = Color.clear;
            }
        }

        public override void OnBeginDrag(PointerEventData eventData) {
            base.OnBeginDrag(eventData);

            // Cancel any coroutines.
            if (fadeHorizontal != null) {
                StopCoroutine(fadeHorizontal);
                fadeHorizontal = null;
            }

            if (fadeVertical != null) {
                StopCoroutine(fadeVertical);
                fadeHorizontal = null;
            }

            // Set color.
            if (handleHorizontal != null) {
                handleHorizontal.color = targetColor;
            }

            if (handleVertical != null) {
                handleVertical.color = targetColor;
            }
        }

        public override void OnEndDrag(PointerEventData eventData) {
            base.OnEndDrag(eventData);

            // Fade out.
            if (handleHorizontal != null)
                fadeHorizontal = StartCoroutine(FadeHandle(handleHorizontal));

            if (handleVertical != null)
                fadeVertical = StartCoroutine(FadeHandle(handleVertical));
        }

        IEnumerator FadeHandle(Image targetGraphic) {
            while (velocity.magnitude > 10f) {
                yield return null;
            }

            float t = 0;

            while (t < 1) {
                t += Time.deltaTime / fadeDuration;
                targetGraphic.color = Color.Lerp(targetColor, Color.clear, t);
                yield return null;
            }
        }
    }
}