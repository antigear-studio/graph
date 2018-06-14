import "dart:async";
import "dart:io";

import "package:path_provider/path_provider.dart";
import "package:uuid/uuid.dart";

import "graph.dart";

/// Manages how graphs are saved on disk. Graphs are convertible to .svg files,
/// which is in XML format.
///
/// Storage: graphs are stored in solid files with different file names. These
/// file names may not necessarily correspond to the graph title. With server
/// side integration planned, it is better to have a UUID as the file name.
class GraphManager {
  /// The path suffix for the folder that stores graphs.
  static const graphFolder = "graphs";

  /// The graph store mapping graph to its id.
  Map<Graph, String> graphStore = {};

  /// Gets the graph storage path. Creates the folder if it doesn't exist.
  Future<Directory> get _graphDirectory async {
    final documentRoot = await getApplicationDocumentsDirectory();
    final graphRoot = new Directory("${documentRoot.path}/$graphFolder/");

    if (!await graphRoot.exists()) {
      await graphRoot.create();
    }

    return graphRoot;
  }

  /// Loads all graphs from disk. Only valid graphs will be added to the store.
  Future<List<Graph>> loadGraphs({bool overwrite = false}) async {
    var entities = (await _graphDirectory).listSync(followLinks: false);

    for (FileSystemEntity entity in entities) {
      if (entity is File) {
        String graphId = entity.path.split("/").last.split(".").first;
        String content = await entity.readAsString();
        Graph graph = Graph.deserialize(content);

        if (graph != null && (!graphStore.containsKey(graphId) || overwrite)) {
          graphStore[graph] = graphId;
        }
      }
    }

    return graphStore.keys;
  }

  /// Saves the current list of graphs to disk.
  Future<void> saveGraphs() async {
    for (var graph in graphStore.keys) {
      await saveGraph(graph);
    }
  }

  /// Saves the given graph to disk.
  Future<void> saveGraph(Graph graph) async {
    Directory directory = await _graphDirectory;
    File file = new File("${directory.path}${graphStore[graph]}.svg");
    await file.writeAsString(graph.serialize());
  }

  /// Creates a new graph. The graph is not immediately saved to disk.
  Graph createGraph() {
    var uuid = Uuid();
    var graph = new Graph();
    graphStore[graph] = uuid.v1();

    return graph;
  }
}
