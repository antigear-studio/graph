using System;
using UnityEngine;

namespace Antigear.Graph.Model {
    /// <summary>
    /// Maintains metadata for each component of the graph and provides common
    /// methods such as translation and rotation.
    /// </summary>
    [Serializable]
    public abstract class Drawable {
        public string name;

        public DateTime timeLastSelected;

        public Vector2 rotationPivot;
    }
}
