using System;
using UnityEngine;

namespace Antigear.Graph {
    /// <summary>
    /// Models a simple straight line.
    /// </summary>
    [Serializable]
    public class StraightLine : Line {
        Vector2 startPoint;
        Vector2 endPoint;
    }
}
