using MaterialUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Antigear.Graph {
    /// <summary>
    /// Acts as the input filter. Detects user touch inputs and translate them
    /// into useful data for delegate.
    /// </summary>
    public class Paper : MonoBehaviour, IBeginDragHandler, IDragHandler, 
    IEndDragHandler, IPointerClickHandler {
        public float animationDuration = 0.2f;

        // Outlets.
        public Image backgroundImage;
        public IPaperDelegate paperDelegate;
        public RectTransform content;

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

        // Private.
        int colorAnimationTweenId = -1;

        public void SetBackgroundColor(Color color, bool animated) {
            if (colorAnimationTweenId >= 0) {
                TweenManager.EndTween(colorAnimationTweenId);
                colorAnimationTweenId = -1;
            }

            if (animated) {
                colorAnimationTweenId = TweenManager.TweenColor(
                    c => backgroundImage.color = c, backgroundImage.color, 
                    color, animationDuration);
            } else {
                backgroundImage.color = color;
            }
        }

        public RectTransform GetContentTransform() {
            return transform.GetChild(0) as RectTransform;
        }

        #region IBeginDragHandler implementation

        public void OnBeginDrag(PointerEventData eventData) {
            if (paperDelegate != null) {
                Vector2 pt;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(content, 
                    eventData.position, Camera.main, out pt);
                paperDelegate.OnPaperBeginDrag(this, pt, eventData.position);
            }
        }

        #endregion

        #region IDragHandler implementation

        public void OnDrag(PointerEventData eventData) {
            if (paperDelegate != null) {
                Vector2 pt;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(content, 
                    eventData.position, Camera.main, out pt);
                paperDelegate.OnPaperDrag(this, pt, eventData.position);
            }
        }

        #endregion

        #region IEndDragHandler implementation

        public void OnEndDrag(PointerEventData eventData) {
            if (paperDelegate != null) {
                Vector2 pt;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(content, 
                    eventData.position, Camera.main, out pt);
                paperDelegate.OnPaperEndDrag(this, pt, eventData.position);
            }
        }

        #endregion

        #region IPointerClickHandler implementation

        public void OnPointerClick(PointerEventData eventData) {
            if (paperDelegate != null) {
                Vector2 pt;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(content, 
                    eventData.position, Camera.main, out pt);
                paperDelegate.OnPaperTap(this, pt, eventData.position, 
                    eventData.clickCount);
            }
        }

        #endregion
    }

    public interface IPaperDelegate {
        /// <summary>
        /// Called when touch started dragging on the paper.
        /// </summary>
        /// <param name="paper">Paper.</param>
        /// <param name="pos">Point of contact, in graph coordinates.</param>
        /// <param name="screenPos">Point of contact, in screen coordinates.
        /// </param>
        void OnPaperBeginDrag(Paper paper, Vector2 pos, Vector2 screenPos);

        /// <summary>
        /// Called when touch is dragging on the paper.
        /// </summary>
        /// <param name="paper">Paper.</param>
        /// <param name="pos">Point of contact, in graph coordinates.</param>
        /// <param name="screenPos">Point of contact, in screen coordinates.
        /// </param>
        void OnPaperDrag(Paper paper, Vector2 pos, Vector2 screenPos);

        /// <summary>
        /// Called when touch stopped dragging on the paper.
        /// </summary>
        /// <param name="paper">Paper.</param>
        /// <param name="pos">Point of contact, in graph coordinates.</param>
        /// <param name="screenPos">Point of contact, in screen coordinates.
        /// </param>
        void OnPaperEndDrag(Paper paper, Vector2 pos, Vector2 screenPos);

        /// <summary>
        /// Called when paper is tapped.
        /// </summary>
        /// <param name="paper">Paper.</param>
        /// <param name="pos">Point of contact, in graph coordinates.</param>
        /// <param name="screenPos">Point of contact, in screen coordinates.
        /// </param>
        /// <param name="count">Number of taps at the same pos in a row.</param>
        void OnPaperTap(Paper paper, Vector2 pos, Vector2 screenPos, int count);

    }
}