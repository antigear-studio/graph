using System;
using UnityEngine;
using System.Collections.Generic;

namespace Antigear.Graph {
    /// <summary>
    /// Models a simple straight line.
    /// </summary>
    [Serializable]
    public class StraightLine : Line {
        /// <summary>
        /// Start point of the line in layer coordinates.
        /// </summary>
        public Vector2 startPoint;

        /// <summary>
        /// End point of the line in layer coordinates.
        /// </summary>
        public Vector2 endPoint;

        public override void Offset(Vector2 amount) {
            base.Offset(amount);

            startPoint += amount;
            endPoint += amount;
        }

        public override List<Vector2> GetPoints() {
            return new List<Vector2> { startPoint, endPoint };
        }

        public override Rect GetBoundary() {
            return Rect.MinMaxRect(Mathf.Min(startPoint.x, endPoint.x), 
                Mathf.Min(startPoint.y, endPoint.y), 
                Mathf.Max(startPoint.x, endPoint.x), 
                Mathf.Max(startPoint.y, endPoint.y));
        }
    }
}
