using System;
using System.Collections.Generic;
using UnityEngine;

namespace Antigear.Graph {
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

        /// <summary>
        /// Returns a list of points that should render this line.
        /// </summary>
        /// <returns>The points.</returns>
        public virtual List<Vector2> GetPoints() {
            return new List<Vector2>();
        }
    }
}
