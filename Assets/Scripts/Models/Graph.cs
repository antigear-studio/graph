using System;
using System.Collections.Generic;

namespace Antigear.Graph.Model {
    /// <summary>
    /// Encapsulates a single graph object. This includes any graph-specific 
    /// user settings.
    /// </summary>
    [Serializable]
    public class Graph {
        // graph metadata
        public string name = "";
        public DateTime timeCreated;
        public DateTime timeModified;

        // graph content
        public List<Drawable> content;

        // graph-specific preferences

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="Antigear.Graph.Model.Graph"/> class.
        /// </summary>
        public Graph() {
            timeCreated = DateTime.Now;
        }
    }
}
