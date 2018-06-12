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
    /// 
    /// This instance behaves as follows for single/double finger drags.
    /// If a single finger started dragging, a start time is recorded. If within
    /// a grace period a second finger is detected, we enter double finger mode,
    /// otherwise we stay in single finger mode. In singer finger mode, any
    /// touch id other than the first detected will be ignored. We are in this
    /// mode until all fingers are lifted off.
    /// 
    /// Once we enter double finger mode, we send a drag cancel callback, then
    /// we start sending double drag events. Any fingers apart from the original
    /// recorded two will be ignored. End drag is sent if one of the two fingers
    /// lift off and we reset the whole state when all fingers are lifted off.
    /// </summary>
    public class Paper : MonoBehaviour, IBeginDragHandler, IDragHandler, 
    IEndDragHandler, IPointerClickHandler {
        public float animationDuration = 0.2f;
        public float doubleDragGracePeriod = 0.1f;

        // Outlets.
        public Image backgroundImage;
        public GameObject editLayerObject;
        public IPaperDelegate paperDelegate;
        public RectTransform content;

        // Private.
        int colorAnimationTweenId = -1;
        int pointer1Id = -2;
        int pointer2Id = -2;
        float timeSinceFirstTouch = -1;
        Vector2 pointer1Pos;
        Vector2 pointer2Pos;
        Vector2 pointer1ScreenPos;
        Vector2 pointer2ScreenPos;
        bool isDoubleDrag;

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

        Vector2 ScreenToLocal(Vector2 pos) {
            Vector2 pt;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(content,
                pos, Camera.main, out pt);

            return pt;
        }

        #region IBeginDragHandler implementation

        public void OnBeginDrag(PointerEventData eventData) {
            if (pointer1Id < 0 && !isDoubleDrag) {
                // Single finger drag.
                pointer1Id = eventData.pointerId;
                timeSinceFirstTouch = Time.time;

                pointer1Pos = ScreenToLocal(eventData.position);
                pointer1ScreenPos = eventData.position;

                if (paperDelegate != null) {
                    paperDelegate.OnPaperBeginDrag(this, pointer1Pos, 
                        pointer1ScreenPos);
                }
            } else if (pointer2Id < 0 && !isDoubleDrag &&
                Time.time - timeSinceFirstTouch < doubleDragGracePeriod) {
                isDoubleDrag = true;
                pointer2Id = eventData.pointerId;

                pointer2Pos = ScreenToLocal(eventData.position);
                pointer2ScreenPos = eventData.position;

                if (paperDelegate != null) {
                    paperDelegate.OnPaperCancelDrag(this);
                    paperDelegate.OnPaperBeginDoubleDrag(this, pointer1Pos,
                        pointer2Pos, pointer1ScreenPos, pointer2ScreenPos);
                }
            }
        }

        #endregion

        #region IDragHandler implementation

        public void OnDrag(PointerEventData eventData) {
            // Update whichever point is recorded.
            bool pointer1Dragged = eventData.pointerId == pointer1Id;
            bool pointer2Dragged = eventData.pointerId == pointer2Id;

            if (pointer1Dragged) {
                pointer1Pos = ScreenToLocal(eventData.position);
                pointer1ScreenPos = eventData.position;
            } else if (pointer2Dragged) {
                pointer2Pos = ScreenToLocal(eventData.position);
                pointer2ScreenPos = eventData.position;
            }

            // Decide to call whichever function:
            if (paperDelegate != null) {
                bool twoActivePointers = pointer1Id >= 0 && pointer2Id >= 0;
                if (eventData.pointerId == pointer1Id && !isDoubleDrag) {
                    paperDelegate.OnPaperDrag(this, pointer1Pos,
                        pointer1ScreenPos);
                } else if ((pointer1Dragged || pointer2Dragged) && 
                    twoActivePointers) {
                    paperDelegate.OnPaperDoubleDrag(this, pointer1Pos,
                        pointer2Pos, pointer1ScreenPos, pointer2ScreenPos);
                }
            }
        }

        #endregion

        #region IEndDragHandler implementation

        public void OnEndDrag(PointerEventData eventData) {
            bool pointer1Ended = eventData.pointerId == pointer1Id;
            bool pointer2Ended = eventData.pointerId == pointer2Id;
           
            if (pointer1Ended) {
                pointer1Id = -2;
            } else if (pointer2Ended) {
                pointer2Id = -2;
            }

            if (pointer1Ended) {
                pointer1Pos = ScreenToLocal(eventData.position);
                pointer1ScreenPos = eventData.position;
            } else if (pointer2Ended) {
                pointer2Pos = ScreenToLocal(eventData.position);
                pointer2ScreenPos = eventData.position;
            }

            if (paperDelegate != null) {
                if (pointer1Ended || pointer2Ended) {
                    if (!isDoubleDrag) {
                        paperDelegate.OnPaperEndDrag(this, pointer1Pos, 
                            pointer1ScreenPos);
                    } else if (pointer1Id >= 0 || pointer2Id >= 0) {
                        paperDelegate.OnPaperEndDoubleDrag(this, pointer1Pos, 
                            pointer1ScreenPos, pointer2Pos, pointer2ScreenPos);
                    }
                }
            }

            isDoubleDrag &= !(pointer1Id < 0 && pointer2Id < 0);
        }

        #endregion

        #region IPointerClickHandler implementation

        public void OnPointerClick(PointerEventData eventData) {
            // Filter out dragged points.
            if (pointer1Id > -2 || pointer2Id > -2) {
                return;
            }

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
        /// Called when drag was mistakenly detected and should be undone.
        /// </summary>
        /// <param name="paper">Paper.</param>
        void OnPaperCancelDrag(Paper paper);

        /// <summary>
        /// Called when paper is tapped.
        /// </summary>
        /// <param name="paper">Paper.</param>
        /// <param name="pos">Point of contact, in graph coordinates.</param>
        /// <param name="screenPos">Point of contact, in screen coordinates.
        /// </param>
        /// <param name="count">Number of taps at the same pos in a row.</param>
        void OnPaperTap(Paper paper, Vector2 pos, Vector2 screenPos, int count);

        /// <summary>
        /// Called when a swipe is detected with two fingers.
        /// </summary>
        /// <param name="paper">Paper.</param>
        /// <param name="pos1">First point of contact, in graph coordinates.
        /// </param>
        /// <param name="pos2">Second point of contact, in graph coordinates.
        /// </param>
        /// <param name="screenPos1">First point of contact, in screen 
        /// coordinates.</param>
        /// <param name="screenPos2">Second point of contact, in screen 
        /// coordinates.</param>
        void OnPaperBeginDoubleDrag(Paper paper, Vector2 pos1, Vector2 pos2,
            Vector2 screenPos1, Vector2 screenPos2);

        /// <summary>
        /// Called when a swipe with two fingers moved.
        /// </summary>
        /// <param name="paper">Paper.</param>
        /// <param name="pos1">First point of contact, in graph coordinates.
        /// </param>
        /// <param name="pos2">Second point of contact, in graph coordinates.
        /// </param>
        /// <param name="screenPos1">First point of contact, in screen 
        /// coordinates.</param>
        /// <param name="screenPos2">Second point of contact, in screen 
        /// coordinates.</param>
        void OnPaperDoubleDrag(Paper paper, Vector2 pos1, Vector2 pos2,
            Vector2 screenPos1, Vector2 screenPos2);

        /// <summary>
        /// Called when a swipe with two fingers ended (at least one finger
        /// lifted.
        /// </summary>
        /// <param name="paper">Paper.</param>
        /// <param name="pos1">First point of contact, in graph coordinates.
        /// </param>
        /// <param name="pos2">Second point of contact, in graph coordinates.
        /// </param>
        /// <param name="screenPos1">First point of contact, in screen 
        /// coordinates.</param>
        /// <param name="screenPos2">Second point of contact, in screen 
        /// coordinates.</param>
        void OnPaperEndDoubleDrag(Paper paper, Vector2 pos1, Vector2 pos2,
            Vector2 screenPos1, Vector2 screenPos2);
    }
}