﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Antigear.Graph {
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
        [JsonIgnore]
        public string localFileName;
        [JsonIgnore]
        public bool isDirty;

        // graph content
        public List<Drawable> content;

        // graph-specific preferences

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="Antigear.Graph.Graph"/> class.
        /// </summary>
        public Graph() {
            timeCreated = DateTime.UtcNow;
            timeModified = DateTime.UtcNow;
            name = "Untitled";
        }
    }
}
