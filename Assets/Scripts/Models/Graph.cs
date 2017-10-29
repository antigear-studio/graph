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
        /// <summary>
        /// Encapsulates user preferences for a specific graph.
        /// </summary>
        [Serializable]
        public class Preference {
            public string name = "";
            public Color backgroundColor = Color.white;
            public Color selectionHighlightColor = Color.green;
        }

        // graph metadata
        public DateTime timeCreated;
        public DateTime timeModified;
        public int activeLayer;
        public Preference preferences = new Preference();

        [JsonIgnore]
        public string localFileName;
        [JsonIgnore]
        public bool isDirty;
        [JsonIgnore]
        public Tool activeTool = Tool.StraightLine;

        // graph content
        public List<Layer> content = new List<Layer>();

        // graph-specific preferences

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="Antigear.Graph.Graph"/> class.
        /// </summary>
        public Graph() {
            timeCreated = DateTime.UtcNow;
            timeModified = DateTime.UtcNow;
            content.Add(new Layer());
        }
    }
}
