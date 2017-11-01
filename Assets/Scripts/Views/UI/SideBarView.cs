using System.Collections.Generic;
using UnityEngine;
using MaterialUI;

namespace Antigear.Graph {
    /// <summary>
    /// Provides additional functionalities for the graph.
    /// </summary>
    public class SideBarView : MonoBehaviour {
        // Variables
        public float animationDuration = 0.5f;

        // Outlets
        public RectTransform undoTransform;
        public RectTransform redoTransform;
        public RectTransform snapTransform;
        public ISideBarViewDelegate viewDelegate;

        int undoSizeAnimationTransitionId = -1;
        int redoSizeAnimationTransitionId = -1;
        int snapSizeAnimationTransitionId = -1;

        /// <summary>
        /// Sets the visibility of the toolbar icons.
        /// </summary>
        /// <param name="isUndoVisible">If set to <c>true</c> undo button is
        /// visible.</param>
        /// <param name="isRedoVisible">If set to <c>true</c> redo button is
        /// visible.</param>
        /// <param name="isSnapVisible">If set to <c>true</c> snap button is
        /// visible.</param>
        /// <param name="animated">If set to <c>true</c> animation is applied.
        /// </param>
        /// <param name="delay">Animation delay in seconds.</param>
        public void SetVisibility(bool isUndoVisible, bool isRedoVisible, 
            bool isSnapVisible, bool animated = true, float delay = 0) {
            SetUndoButtonVisibility(isUndoVisible, animated, delay);
            SetRedoButtonVisibility(isRedoVisible, animated, delay);
            SetSnapButtonVisibility(isSnapVisible, animated, delay);
        }

        /// <summary>
        /// Sets the visibility of the undo button.
        /// </summary>
        /// <param name="isVisible">If set to <c>true</c> button is visible.
        /// </param>
        /// <param name="animated">If set to <c>true</c> animation is applied.
        /// </param>
        /// <param name="delay">Animation delay in seconds.</param>
        public void SetUndoButtonVisibility(bool isVisible, 
            bool animated = true, float delay = 0) {
            if (undoSizeAnimationTransitionId >= 0) {
                TweenManager.EndTween(undoSizeAnimationTransitionId);
                undoSizeAnimationTransitionId = -1;
            }

            Vector3 target = isVisible ? Vector3.one : new Vector3(0, 0, 1);

            if (animated) {
                undoSizeAnimationTransitionId = TweenManager.TweenVector3(
                    v => undoTransform.localScale = v,
                    undoTransform.localScale, target, animationDuration, delay);
            } else {
                undoTransform.localScale = target;
            }
        }

        /// <summary>
        /// Sets the visibility of the redo button.
        /// </summary>
        /// <param name="isVisible">If set to <c>true</c> button is visible.
        /// </param>
        /// <param name="animated">If set to <c>true</c> animation is applied.
        /// </param>
        /// <param name="delay">Animation delay in seconds.</param>
        public void SetRedoButtonVisibility(bool isVisible, 
            bool animated = true, float delay = 0) {
            if (redoSizeAnimationTransitionId >= 0) {
                TweenManager.EndTween(redoSizeAnimationTransitionId);
                redoSizeAnimationTransitionId = -1;
            }

            Vector3 target = isVisible ? Vector3.one : new Vector3(0, 0, 1);

            if (animated) {
                redoSizeAnimationTransitionId = TweenManager.TweenVector3(
                    v => redoTransform.localScale = v,
                    redoTransform.localScale, target, animationDuration, delay);
            } else {
                redoTransform.localScale = target;
            }
        }

        /// <summary>
        /// Sets the visibility of the snap button.
        /// </summary>
        /// <param name="isVisible">If set to <c>true</c> button is visible.
        /// </param>
        /// <param name="animated">If set to <c>true</c> animation is applied.
        /// </param>
        /// <param name="delay">Animation delay in seconds.</param>
        public void SetSnapButtonVisibility(bool isVisible, 
            bool animated = true, float delay = 0) {
            if (snapSizeAnimationTransitionId >= 0) {
                TweenManager.EndTween(snapSizeAnimationTransitionId);
                snapSizeAnimationTransitionId = -1;
            }

            Vector3 target = isVisible ? Vector3.one : new Vector3(0, 0, 1);

            if (animated) {
                snapSizeAnimationTransitionId = TweenManager.TweenVector3(
                    v => snapTransform.localScale = v,
                    snapTransform.localScale, target, animationDuration, delay);
            } else {
                snapTransform.localScale = target;
            }
        }

        public void OnUndoButtonPress() {
            if (viewDelegate != null)
                viewDelegate.OnUndoButtonPress(this);
        }

        public void OnRedoButtonPress() {
            if (viewDelegate != null)
                viewDelegate.OnRedoButtonPress(this);
        }

        public void OnSnapButtonPress() {
            if (viewDelegate != null)
                viewDelegate.OnSnapButtonPress(this);
        }
    }

    public interface ISideBarViewDelegate {
        void OnUndoButtonPress(SideBarView sideBarView);
        void OnRedoButtonPress(SideBarView sideBarView);
        void OnSnapButtonPress(SideBarView sideBarView);
    }
}
