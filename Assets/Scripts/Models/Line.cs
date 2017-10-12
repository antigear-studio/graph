using System;
using UnityEngine;

namespace Antigear.Graph.Model {
    /// <summary>
    /// Maintains metadata for line objects in general, such as line color,
    /// style, etc.
    /// </summary>
    [Serializable]
    public abstract class Line : Drawable {

        /// <summary>
        /// Enumerates supported line dash styles.
        /// </summary>
        public enum DashStyle {
            Solid,
            Dash,
            Dot,
            DashDot
        }

        /// <summary>
        /// Enumerates supported line brush styles.
        /// </summary>
        public enum BrushStyle {
            Normal,
            HandDrawn
        }

        Color color;
        float width;
        DashStyle dashStyle;
        BrushStyle brushStyle;
    }
}
