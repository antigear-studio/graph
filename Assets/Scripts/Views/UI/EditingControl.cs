using MaterialUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Vectrosity;

namespace Antigear.Graph {
    /// <summary>
    /// Acts as the input filter for editor controls.
    /// </summary>
    public class EditingControl : MonoBehaviour, IBeginDragHandler,
    IDragHandler, IEndDragHandler {
        // Outlets.
        public IEditingControlDelegate controlDelegate;
        public ConstantScreenResizer resizer;
        public VectorImage fillImage;
        public VectorObject2D rimObject;
        public RectTransform graphicsRoot;

        // Private.
        int pointerId = -2;

        Vector2 ScreenToLocal(Vector2 pos) {
            Vector2 pt;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                transform as RectTransform, pos, Camera.main, out pt);

            return pt;
        }

        /// <summary>
        /// Updates the editing control given the preferences.
        /// </summary>
        /// <param name="drawingPreferences">Drawing preferences.</param>
        public void UpdateView(Graph.Preference drawingPreferences) {
            resizer.width = drawingPreferences.controlPointSize;
            resizer.height = drawingPreferences.controlPointSize;
            rimObject.vectorLine.MakeCircle(Vector2.zero, 
                drawingPreferences.controlPointSize / 2 - 4);
            rimObject.vectorLine.color = 
                drawingPreferences.controlPointRimColor;
            rimObject.vectorLine.Draw();
            fillImage.color = drawingPreferences.controlPointFillColor;
        }

        #region IBeginDragHandler implementation

        public void OnBeginDrag(PointerEventData eventData) {
            if (pointerId < 0) {
                // Single finger drag.
                pointerId = eventData.pointerId;

                if (controlDelegate != null) {
                    controlDelegate.OnEditingControlBeginDrag(this,
                        ScreenToLocal(eventData.position), eventData.position);
                }
            }
        }

        #endregion

        #region IDragHandler implementation

        public void OnDrag(PointerEventData eventData) {
            // Update whichever point is recorded.
            bool pointer1Dragged = eventData.pointerId == pointerId;

            if (pointer1Dragged && controlDelegate != null) {
                controlDelegate.OnEditingControlDrag(this, 
                    ScreenToLocal(eventData.position), eventData.position);
            }
        }

        #endregion

        #region IEndDragHandler implementation

        public void OnEndDrag(PointerEventData eventData) {
            if (eventData.pointerId == pointerId) {
                pointerId = -2;

                if (controlDelegate != null) {
                    controlDelegate.OnEditingControlEndDrag(this,
                        ScreenToLocal(eventData.position), eventData.position);
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