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

        string path;

        JsonSerializer serializer;

        void Awake() {
            path = Application.persistentDataPath + "/Graphs/";
            serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());

            if (!Directory.Exists(path)) {
                Debug.Log("Creating graph directory.");
                Directory.CreateDirectory(path);
            }
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
            } catch {
                return false;
            }
        }

        /// <summary>
        /// Saves all graphs to disk. This only operates on dirty graphs, so
        /// this method can be called without run time concerns even on large
        /// graph collections. If a graph fails, other graphs will continue to
        /// be saved.
        /// </summary>
        /// <returns><c>true</c>, if all graphs are saved to disk successfully, 
        /// <c>false</c> otherwise.</returns>
        public bool SaveAllToDisk() {
            bool allSuccess = true;

            foreach (Graph graph in graphs) {
                if (!graph.isDirty) {
                    continue;
                }

                bool success = SaveToDisk(graph);
                graph.isDirty = !success;
                allSuccess |= success;
            }

            return allSuccess;
        }

        /// <summary>
        /// Loads all graphs from disk.
        /// </summary>
        /// <returns><c>true</c>, if all graphs were loaded, <c>false</c> 
        /// otherwise.</returns>
        public bool LoadAllFromDisk() {
            try {
                string[] filePaths = Directory.GetFiles(path);
                graphs = new List<Graph>();

                foreach (string filePath in filePaths) {
                    using (StreamReader sr = new StreamReader(filePath)) {
                        using (JsonReader reader = new JsonTextReader(sr)) {
                            Graph graph = serializer.Deserialize<Graph>(reader);
                            graph.localFileName = Path.GetFileName(path);
                            graph.isDirty = false;
                            graphs.Add(graph);
                        }
                    }
                }

                return true;
            } catch {
                return false;
            }
        }

        /// <summary>
        /// Creates a new graph and return to the user. The new graph is not
        /// immediately saved to disk, however.
        /// </summary>
        /// <returns>The graph.</returns>
        public Graph CreateGraph() {
            Graph graph = new Graph();
            graphs.Add(graph);

            return graph;
        }

        /// <summary>
        /// Returns a list of the stored graphs, sorted by order.
        /// </summary>
        public List<Graph> GetGraphs() {
            return new List<Graph>(graphs);
        }
    }
}
