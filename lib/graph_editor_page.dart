import "package:flutter/material.dart";

import "graph.dart";

class GraphEditorPage extends StatefulWidget {
  final Graph graph;

  /// Default initializer that sets up the editor with the given graph.
  GraphEditorPage(this.graph);

  @override
  createState() => new GraphEditorPageState(graph);
}

class GraphEditorPageState extends State<GraphEditorPage> {
  final Graph graph;

  /// Default initializer that sets up the editor state with the given graph.
  GraphEditorPageState(this.graph);

  @override
  Widget build(BuildContext context) {
    return new Scaffold(
      body: new Container(
        color: Colors.white,
        child: new IconButton(
          onPressed: _onCloseButtonPressed,
          icon: new Icon(Icons.close),
        ),
      ),
    );
  }

  void _onCloseButtonPressed() {
    Navigator.pop(context);
  }
}
