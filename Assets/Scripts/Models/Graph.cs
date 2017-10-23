using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Antigear.Graph {
    /// <summary>
    /// Encapsulates a single graph object. This includes any graph-specific 
    /// user settings.
    /// </summary>
    [Serializable]
    public class Graph {
        // graph metadata
        public string name;
        public DateTime timeCreated;
        public DateTime timeModified;
        public Color backgroundColor = Color.white;

        [JsonIgnore]
        public string localFileName;
        [JsonIgnore]
        public bool isDirty;
        [JsonIgnore]
        public Tool activeTool = Tool.StraightLine;


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
