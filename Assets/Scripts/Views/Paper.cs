using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Antigear.Graph {
    /// <summary>
    /// Acts as the input filter. Detects user touch inputs and translate them
    /// into useful data for delegate.
    /// </summary>
    public class Paper : MonoBehaviour, IBeginDragHandler, IDragHandler, 
    IEndDragHandler, IPointerClickHandler {
        #region IBeginDragHandler implementation

        public void OnBeginDrag(PointerEventData eventData) {
            Debug.Log("Begin drag: " + eventData.position);
        }

        #endregion

        #region IDragHandler implementation

        public void OnDrag(PointerEventData eventData) {
            Debug.Log("Drag: " + eventData.position);
        }

        #endregion

        #region IEndDragHandler implementation

        public void OnEndDrag(PointerEventData eventData) {
            Debug.Log("End drag: " + eventData.position);
        }

        #endregion

        #region IPointerClickHandler implementation

        public void OnPointerClick(PointerEventData eventData) {
            Debug.Log("Tapped: " + eventData.position + " Count: " + eventData.clickCount);
        }

        #endregion
    }

    public interface IPaperDelegate {
        /// <summary>
        /// Called when touch started dragging on the paper.
        /// </summary>
        /// <param name="paper">Paper.</param>
        /// <param name="eventData">Event data.</param>
        void OnPaperBeginDrag(Paper paper, PointerEventData eventData);

        /// <summary>
        /// Called when touch is dragging on the paper.
        /// </summary>
        /// <param name="paper">Paper.</param>
        /// <param name="eventData">Event data.</param>
        void OnPaperDrag(Paper paper, PointerEventData eventData);

        /// <summary>
        /// Called when touch stopped dragging on the paper.
        /// </summary>
        /// <param name="paper">Paper.</param>
        /// <param name="eventData">Event data.</param>
        void OnPaperEndDrag(Paper paper, PointerEventData eventData);

        /// <summary>
        /// Called when paper is tapped. Double taps are also handled in here.
        /// </summary>
        /// <param name="paper">Paper.</param>
        /// <param name="eventData">Event data.</param>
        void OnPaperClick(Paper paper, PointerEventData eventData);

    }
}