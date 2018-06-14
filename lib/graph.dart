import "package:xml/xml.dart" as xml;

/// This class stores information about a graph. It also knows how to serialize/
/// deserialize to an xml string. This xml string complies with the svg format.
class Graph {
  /// The graph identifier is a uuid that is used as its file name.
  String id = "";

  /// The title of this graph.
  String title = "";

  DateTime createdOn;

  DateTime updatedOn;

  /// Deserializes a graph from xml string representation.
  static Graph deserialize(String content) {
    var document;

    // Try parse xml.
    try {
      document = xml.parse(content);
    } on ArgumentError {
      print("Failed to parse graph as an xml object. Content: \n\n$content");
      return null;
    }

    // Check has svg element.
    if (document.findElements("svg").isEmpty) {
      return null;
    }

    var graph = new Graph();

    // Parse metadata.

    // Parse content elements.

    return graph;
  }

  /// Serializes the content of this graph into an xml string.
  String serialize() {
    return "<svg></svg>";
  }
}
