using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Antigear.Graph {
    /// <summary>
    /// Grabs every pointer event on screen, relay to target if necessary.
    /// </summary>
    public class MenuMaskView : MonoBehaviour, IBeginDragHandler,
    IPointerClickHandler {
        public Paper target;

        #region IBeginDragHandler implementation

        public void OnBeginDrag(PointerEventData eventData) {
            target.OnPointerClick(eventData);
        }

        #endregion

        #region IPointerClickHandler implementation

        public void OnPointerClick(PointerEventData eventData) {
            target.OnPointerClick(eventData);
        }

        #endregion
        
    }
}