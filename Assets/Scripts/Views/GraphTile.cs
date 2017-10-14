using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Antigear.Graph {
    /// <summary>
    /// Handles tile configurations, such as setting labels, graph previews and
    /// such.
    /// </summary>
    public class GraphTile : MonoBehaviour {
        public IGraphTileDelegate graphTileDelegate;

        public void OnGraphTileClick() {
            if (graphTileDelegate != null) {
                graphTileDelegate.OnGraphTileClick(this);
            }
        }
    }

    /// <summary>
    /// Defines actions reportable by the graph tile.
    /// </summary>
    public interface IGraphTileDelegate {
        /// <summary>
        /// Raises the graph tile clicked event. Triggered when the graph tile's
        /// main action button is triggered.
        /// </summary>
        /// <param name="clickedTile">Clicked tile.</param>
        void OnGraphTileClick(GraphTile clickedTile);
    }
}