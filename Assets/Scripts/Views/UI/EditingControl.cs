using MaterialUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Antigear.Graph {
    /// <summary>
    /// Acts as the input filter for editor controls.
    /// </summary>
    public class EditingControl : MonoBehaviour, IBeginDragHandler,
    IDragHandler, IEndDragHandler {
        // Outlets.
        public IEditingControlDelegate controlDelegate;

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
        int pointerId = -2;

        Vector2 ScreenToLocal(Vector2 pos) {
            Vector2 pt;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                transform as RectTransform, pos, Camera.main, out pt);

            return pt;
        }

        #region IBeginDragHandler implementation

        public void OnBeginDrag(PointerEventData eventData) {
            if (pointerId < 0) {
                // Single finger drag.
                pointerId = eventData.pointerId;

                if (controlDelegate != null) {
                    controlDelegate.OnEditingControlBeginDrag(this,
                        ScreenToLocal(eventData.position) * scaler.scaleFactor, 
                        eventData.position);
                }
            }
        }

        #endregion

        #region IDragHandler implementation

        public void OnDrag(PointerEventData eventData) {
            // Update whichever point is recorded.
            bool pointer1Dragged = eventData.pointerId == pointerId;

            if (pointer1Dragged) {
                controlDelegate.OnEditingControlDrag(this, 
                    ScreenToLocal(eventData.position) * scaler.scaleFactor,
                    eventData.position);
            }
        }

        #endregion

        #region IEndDragHandler implementation

        public void OnEndDrag(PointerEventData eventData) {
            if (eventData.pointerId == pointerId) {
                pointerId = -2;
                if (controlDelegate != null) {
                    controlDelegate.OnEditingControlEndDrag(this,
                        ScreenToLocal(eventData.position) * scaler.scaleFactor,
                        eventData.position);
                }
            }
        }

        #endregion
    }

    public interface IEditingControlDelegate {
        /// <summary>
        /// Called when touch started dragging on the editingControl.
        /// </summary>
        /// <param name="editingControl">EditingControl.</param>
        /// <param name="pos">Point of contact, in graph coordinates.</param>
        /// <param name="screenPos">Point of contact, in screen coordinates.
        /// </param>
        void OnEditingControlBeginDrag(EditingControl editingControl, 
            Vector2 pos, Vector2 screenPos);

        /// <summary>
        /// Called when touch is dragging on the editingControl.
        /// </summary>
        /// <param name="editingControl">EditingControl.</param>
        /// <param name="pos">Point of contact, in graph coordinates.</param>
        /// <param name="screenPos">Point of contact, in screen coordinates.
        /// </param>
        void OnEditingControlDrag(EditingControl editingControl, Vector2 pos, 
            Vector2 screenPos);

        /// <summary>
        /// Called when touch stopped dragging on the editingControl.
        /// </summary>
        /// <param name="editingControl">EditingControl.</param>
        /// <param name="pos">Point of contact, in graph coordinates.</param>
        /// <param name="screenPos">Point of contact, in screen coordinates.
        /// </param>
        void OnEditingControlEndDrag(EditingControl editingControl, Vector2 pos, 
            Vector2 screenPos);
    }
}