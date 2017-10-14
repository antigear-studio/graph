using System.Collections.Generic;
using UnityEngine;

namespace Antigear.Graph {
    /// <summary>
    /// Provides functionalities for the history bar view. This is the bar with
    /// undo and redo buttons. This bar also react to user settings (displaying
    /// on left or right side of the screen). Also shrinks when one of undo/redo
    /// is unavailable. Finally, if on a new canvas without history, nothing
    /// will be displayed.
    /// </summary>
    public class HistoryBarView : MonoBehaviour {
        public bool showUndo;
        public bool showRedo;

        bool didShowUndo;
        bool didShowRedo;

        readonly List<int> horizontalShrinkAnimationTweenIds = new List<int>();
        readonly List<int> verticalPopupAnimationTweenIds = new List<int>();

        void Start() {
            didShowUndo = showUndo;
            didShowRedo = showRedo;
        }

        /// <summary>
        /// Sets the view based on either options being available.
        /// </summary>
        /// <param name="hasUndo">If set to <c>true</c> has undo.</param>
        /// <param name="hasRedo">If set to <c>true</c> has redo.</param>
        /// <param name="animated">If set to <c>true</c> animated.</param>
        public void SetShowHistory(bool hasUndo, bool hasRedo, bool animated) {
            // Animation consists of setting the right anchors, then fade the 
            // button not being used and shrink the rect at the same time.
        }
    }
}
