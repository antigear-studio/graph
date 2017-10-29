using System;
using UnityEngine;
using System.Collections.Generic;

namespace Antigear.Graph {
    /// <summary>
    /// Models a simple straight line.
    /// </summary>
    [Serializable]
    public class StraightLine : Line {
        public Vector2 startPoint;
        public Vector2 endPoint;

        public override List<Vector2> GetPoints() {
            return new List<Vector2> { startPoint, endPoint };
        }

        public override void Offset(Vector2 amount) {
            base.Offset(amount);

            startPoint += amount;
            endPoint += amount;
        }
    }
}
