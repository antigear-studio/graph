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
        object relayDragInfo;

        Coroutine fadeHorizontal, fadeVertical;
        Image handleHorizontal, handleVertical;

        Color targetColor = new Color(0, 0, 0, 0.25f);
        public float fadeDuration = 0.15f;

        public bool hasCurrentDrag;

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

        public void SetDragListener<T>(T value) where T : IBeginDragHandler, 
        IDragHandler, IEndDragHandler {
            relayDragInfo = value;
        }

        public override void OnBeginDrag(PointerEventData eventData) {
            hasCurrentDrag = true;

            if (relayDragInfo != null)
                ((IBeginDragHandler)relayDragInfo).OnBeginDrag(eventData);
            

            if (hasCurrentDrag)
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

        public override void OnDrag(PointerEventData eventData) {
            if (relayDragInfo != null)
                ((IDragHandler)relayDragInfo).OnDrag(eventData);

            if (hasCurrentDrag)
                base.OnDrag(eventData);
            
        }


        public override void OnEndDrag(PointerEventData eventData) {
            if (relayDragInfo != null)
                ((IEndDragHandler)relayDragInfo).OnEndDrag(eventData);
            
            if (hasCurrentDrag)
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