using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Antigear.Graph {
    /// <summary>
    /// A generic cell used in a grid view. Subclass to provide more behavior.
    /// </summary>
    public class GridViewCell : MonoBehaviour {
        /// <summary>
        /// Prepares the cell for reinsertion into the grid view.
        /// </summary>
        public virtual void PrepareForReuse() {}
    }
}