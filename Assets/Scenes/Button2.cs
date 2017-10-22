using System;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UnityEngine.UI
{
    // Button that's meant to work with mouse or touch-based devices.
    public class Button2 : UIBehaviour, IPointerClickHandler
    {

        // Trigger all registered callbacks.
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("Pointer clicked");
        }
    }
}