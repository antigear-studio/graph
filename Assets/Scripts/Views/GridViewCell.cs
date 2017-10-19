using MaterialUI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Antigear.Graph {
    /// <summary>
    /// A generic cell used in a grid view. Subclass to provide more behavior.
    /// </summary>
    public class GridViewCell : MonoBehaviour {
        // These are for bookkeeping and should not be touched apart from
        // inside implementations!
        public string reuseIdentifier;
        public int translationAnimationId = -1;
        public int resizeAnimationId = -1;
        public bool preventFromDequeue;
        public bool isDeleting;

        // Public fields.
        public float animationDuration = 0.3f;

        /// <summary>
        /// Prepares the cell for reinsertion into the grid view.
        /// </summary>
        public virtual void PrepareForReuse() {}

        /// <summary>
        /// Animates item creation. This is called when the cell should animate
        /// its creation.
        /// </summary>
        /// <param name="callback">Callback when done.</param>
        public virtual void AnimateCreate(Action callback = null) {
            // Creation should be called once.
            Vector3 start = new Vector3(0, 0, 1);
            Vector3 target = Vector3.one;

            resizeAnimationId = TweenManager.TweenVector3(
                v => transform.localScale = v, start, target, animationDuration, 
                0, callback);
        }

        /// <summary>
        /// Animates item deletion. This is called when the cell should animate
        /// its deletion.
        /// </summary>
        /// <param name="callback">Callback when done.</param>
        public virtual void AnimateDelete(Action callback = null) {
            if (isDeleting)
                return;

            // Deletion should be called once.
            isDeleting = true;

            Vector3 start = transform.localScale;
            Vector3 target = new Vector3(0, 0, 1);
            TweenManager.TweenVector3(v => transform.localScale = v, start,
                target, animationDuration, 0, callback);
        }

        /// <summary>
        /// Animates item resize. This is called when the cell should animate
        /// its resize.
        /// </summary>
        /// <param name="targetSize">Size upon animation completion.</param>
        /// <param name="callback">Callback when done.</param>
        public virtual void AnimateResize(Vector2 targetSize, 
            Action callback = null) {
            if (resizeAnimationId >= 0) {
                TweenManager.EndTween(resizeAnimationId, true);
            }

            RectTransform r = (transform as RectTransform);
            Vector2 start = r.sizeDelta;

            resizeAnimationId = TweenManager.TweenVector2(v => r.sizeDelta = v,
                start, targetSize, animationDuration, 0, callback);
        }
    }
}
