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
        public bool preventFromDequeue;

        /// <summary>
        /// Prepares the cell for reinsertion into the grid view.
        /// </summary>
        public virtual void PrepareForReuse() {}
    }
}
