using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Antigear.Graph {
    /// <summary>
    /// Manages graph objects. This include (de)serialization, cloud syncing,
    /// and updating.
    /// </summary>
    public class GraphStore : MonoBehaviour {
        /// <summary>
        /// The current maintained list of graphs.
        /// </summary>
        List<Graph> graphs = new List<Graph>();

        readonly string path = Application.persistentDataPath + "/Graphs/";

        JsonSerializer serializer;

        void Start() {
            serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
        }

        /// <summary>
        /// Saves the given graph to disk. If graph is new, this also assigns a
        /// name to the graph.
        /// </summary>
        /// <returns>Whether operation was successful.</returns>
        /// <param name="graph">Graph to save.</param>
        bool SaveToDisk(Graph graph) {
            // Determine file name if one is not set. This is a random name
            // unique to disk.
            try {
                if (string.IsNullOrEmpty(graph.localFileName)) {
                    string fileName = Guid.NewGuid().ToString();

                    while (File.Exists(path + fileName)) {
                        fileName = Guid.NewGuid().ToString();
                    }

                    // Found a unique name.
                    graph.localFileName = fileName;
                }

                string fullPath = path + graph.localFileName;

                using (StreamWriter sw = new StreamWriter(fullPath)) {
                    using (JsonWriter writer = new JsonTextWriter(sw)) {
                        serializer.Serialize(writer, graph);
                    }
                }

                return true;
            } catch (Exception any) {
                return false;
            }
        }

        /// <summary>
        /// Saves all graphs to disk.
        /// </summary>
        /// <returns><c>true</c>, if all graphs are saved to disk successfully, 
        /// <c>false</c> otherwise.</returns>
        public bool SaveAllToDisk() {
            bool success = true;

            foreach (Graph graph in graphs) {
                success |= SaveToDisk(graph);
            }

            return success;
        }

        /// <summary>
        /// Loads all graphs from disk. If there is an error, this returns a
        /// null object.
        /// </summary>
        /// <returns>The all from disk.</returns>
        public List<Graph> LoadAllFromDisk() {
            try {
                string[] filePaths = Directory.GetFiles(path);
                List<Graph> graphs = new List<Graph>();

                foreach (string filePath in filePaths) {
                    using (StreamReader sr = new StreamReader(filePath)) {
                        using (JsonReader reader = new JsonTextReader(sr)) {
                            Graph graph = serializer.Deserialize<Graph>(reader);
                            graph.localFileName = Path.GetFileName(path);
                            graphs.Add(graph);
                        }
                    }
                }

                return graphs;
            } catch (Exception any) {
                return null;
            }
        }
    }
}
