using MaterialUI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Antigear.Graph {
    /// <summary>
    /// The main view handling drawing - aka putting stuff in the graph onto the
    /// screen. This view also handles its own entry/exit animations.
    /// </summary>
    [ExecuteInEditMode]
    public class DrawingView : MonoBehaviour {
        public MaterialShadow drawingViewMaterialShadow;

        public float expandedTopOffset = 96;
        public float animationDuration = 0.5f;

        readonly List<int> expansionAnimationTweenIds = new List<int>();
        public bool isExpanded;
        bool wasExpanded;

        void Start() {
            wasExpanded = isExpanded;
        }

        void Update() {
            if (wasExpanded != isExpanded) {
                SetExpansion(isExpanded, false);
            }
        }

        /// <summary>
        /// Enlarges or shrinks the drawing view. If this is animated, you must 
        /// also provide a tile to expand from or shrink to.
        /// </summary>
        /// <param name="shouldExpand">If set to <c>true</c> expands drawing
        /// view. </param>
        /// <param name="animated">If set to <c>true</c> animates this
        /// transition.</param>
        /// <param name="tile">Tile to begin the animatino on.</param>
        /// <param name="handler">The completion handler.</param>
        public void SetExpansion(bool shouldExpand, bool animated, 
            GraphTile tile = null, Action handler = null) {
            // This animation consists an elevation from the grid view and a
            // new size and pos change.
            if (shouldExpand == wasExpanded)
                return;

            if (expansionAnimationTweenIds.Count > 0) {
                // Cancel existing animations.
                foreach (int id in expansionAnimationTweenIds) {
                    TweenManager.EndTween(id);
                }

                expansionAnimationTweenIds.Clear();
            }

            wasExpanded = shouldExpand;
            isExpanded = shouldExpand;

            int shadowId = shouldExpand ? 
                drawingViewMaterialShadow.shadowActiveSize : 
                drawingViewMaterialShadow.shadowNormalSize;

            Vector2 expandedOffsetMin = Vector2.zero;
            Vector2 expandedOffsetMax = new Vector2(0, -expandedTopOffset);
            RectTransform rectTransform = transform as RectTransform;
            RectTransform parentRectTransform = 
                transform.parent as RectTransform;

            if (animated) {
                // Expanding from tile, so we need to calculate tile's coord on
                // the fly.
                int t1, t2;
                RectTransform tileRectTransform = 
                    tile.transform as RectTransform;
                
                Func<Vector2> offsetMinToTile = () => {
                    // Much simpler, since we will always end up shrinking to
                    // the tile.
                    Vector3[] parentCorners = new Vector3[4];
                    Vector3[] tileCorners = new Vector3[4];
                    parentRectTransform.GetWorldCorners(parentCorners);
                    tileRectTransform.GetWorldCorners(tileCorners);
                    Vector3 offset = tileCorners[0] - parentCorners[0];

                    return rectTransform.InverseTransformVector(offset);
                };

                Func<Vector2> offsetMaxToTile = () => {
                    // Much simpler, since we will always end up shrinking to
                    // the tile.
                    Vector3[] parentCorners = new Vector3[4];
                    Vector3[] tileCorners = new Vector3[4];
                    parentRectTransform.GetWorldCorners(parentCorners);
                    tileRectTransform.GetWorldCorners(tileCorners);
                    Vector3 offset = tileCorners[2] - parentCorners[2];

                    return rectTransform.InverseTransformVector(offset);
                };

                if (shouldExpand) {
                    t1 = TweenManager.TweenVector2(
                        v => rectTransform.offsetMin = v, offsetMinToTile, 
                        expandedOffsetMin, animationDuration, 0, handler);
                    t2 = TweenManager.TweenVector2(
                        v => rectTransform.offsetMax = v, offsetMaxToTile, 
                        expandedOffsetMax, animationDuration);
                } else {
                    t1 = TweenManager.TweenVector2(
                        v => rectTransform.offsetMin = v, 
                        () => rectTransform.offsetMin, offsetMinToTile, 
                        animationDuration, 0, handler);
                    t2 = TweenManager.TweenVector2(
                        v => rectTransform.offsetMax = v, 
                        () => rectTransform.offsetMax, offsetMaxToTile, 
                        animationDuration);
                }

                expansionAnimationTweenIds.Add(t1);
                expansionAnimationTweenIds.Add(t2);

                drawingViewMaterialShadow.SetShadows(shadowId);
            } else {
                Vector2 shrunkOffset = parentRectTransform.sizeDelta / 2.0f;

                if (shouldExpand) {
                    rectTransform.offsetMin = expandedOffsetMin;
                    rectTransform.offsetMax = expandedOffsetMax;
                } else {
                    rectTransform.offsetMin = shrunkOffset;
                    rectTransform.offsetMax = -shrunkOffset;
                }

                if (handler != null)
                    handler();

                drawingViewMaterialShadow.SetShadowsInstant(shadowId);
            }
        }
    }
}
