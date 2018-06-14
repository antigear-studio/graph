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

  /// The graph store mapping graph to its id. This list can be sorted in
  /// arbitrary ways.
  List<Graph> graphStore = [];

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
  /// Existing graphs may be overwritten if overwrite is true.
  Future<void> loadGraphs({bool overwrite = false}) async {
    var entities = (await _graphDirectory).listSync(followLinks: false);

    outer: for (FileSystemEntity entity in entities) {
      if (entity is File) {
        String graphId = entity.path.split("/").last.split(".").first;
        String content = await entity.readAsString();
        Graph graph = Graph.deserialize(content);

        if (graph != null) {
          for (int i = 0; i < graphStore.length; i++) {
            if (graphStore[i].id == graphId && overwrite) {
              graphStore[i] = graph;
              continue outer;
            }
          }

          graphStore.add(graph);
        }
      }
    }
  }

  /// Saves the current list of graphs to disk.
  Future<void> saveGraphs() async {
    for (var graph in graphStore) {
      await saveGraph(graph);
    }
  }

  /// Saves the given graph to disk.
  Future<void> saveGraph(Graph graph) async {
    Directory directory = await _graphDirectory;
    File file = new File("${directory.path}${graph.id}.svg");
    await file.writeAsString(graph.serialize());
  }

  /// Creates a new graph. The graph is not immediately saved to disk.
  Graph createGraph() {
    var uuid = Uuid();
    var graph = new Graph();
    graph.id = uuid.v1();
    graph.createdOn = new DateTime.now().toUtc();
    graph.updatedOn = graph.createdOn;
    graphStore.add(graph);

    return graph;
  }
}
