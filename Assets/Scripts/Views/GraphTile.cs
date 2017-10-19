using MaterialUI;
using System;
using TimeAgo;
using UnityEngine;

namespace Antigear.Graph {
    /// <summary>
    /// Handles tile configurations, such as setting labels, graph previews and
    /// such.
    /// </summary>
    public class GraphTile : GridViewCell {
        public IGraphTileDelegate graphTileDelegate;

        // UI links
        public RectTransform overlayRectTransform;
        public Text titleText;
        public Text subtitleText;

        // Exposed
        public bool isOverlayVisible = true;
        public float animationDuration = 0.2f;

        bool wasOverlayVisible;
        int overlayAnimationTweenId = -1;

        void Start() {
            wasOverlayVisible = isOverlayVisible;
        }

        void Update() {
            if (wasOverlayVisible != isOverlayVisible) {
                SetOverlayVisibility(isOverlayVisible, false);
            }

            if (Input.GetKeyDown(KeyCode.Space)) {
                SetOverlayVisibility(!isOverlayVisible, true);
            }
        }

        public void SetOverlayVisibility(bool visible, bool animated,
            Action callback = null) {
            wasOverlayVisible = visible;
            isOverlayVisible = visible;

            Vector2 targetPos = overlayRectTransform.anchoredPosition;
            targetPos.y = visible ? 0 : -overlayRectTransform.sizeDelta.y;
            Vector2 startPos = targetPos;
            startPos.y = !visible ? 0 : -overlayRectTransform.sizeDelta.y;

            if (overlayAnimationTweenId >= 0) {
                TweenManager.EndTween(overlayAnimationTweenId);
                overlayAnimationTweenId = -1;
            }

            if (animated) {
                overlayRectTransform.anchoredPosition = startPos;
                overlayAnimationTweenId = 
                    TweenManager.TweenVector2(
                        a => overlayRectTransform.anchoredPosition = a, 
                        overlayRectTransform.anchoredPosition, targetPos, 
                        animationDuration, 0, callback);
            } else {
                overlayRectTransform.anchoredPosition = targetPos;

                if (callback != null)
                    callback();
            }
        }

        public void OnGraphTileClick() {
            if (graphTileDelegate != null) {
                graphTileDelegate.OnGraphTileClick(this);
            }
        }

        public void UpdateCellWithGraph(Graph graph) {
            titleText.text = graph.name;
            subtitleText.text = graph.timeModified.ToLocalTime().TimeAgo();
        }
    }

    /// <summary>
    /// Defines actions reportable by the graph tile.
    /// </summary>
    public interface IGraphTileDelegate {
        /// <summary>
        /// Raises the graph tile clicked event. Triggered when the graph tile's
        /// main action button is triggered.
        /// </summary>
        /// <param name="clickedTile">Clicked tile.</param>
        void OnGraphTileClick(GraphTile clickedTile);
    }
}
