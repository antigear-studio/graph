import "package:flutter/material.dart";

void main() => runApp(new MyApp());

class MyApp extends StatelessWidget {
  // This widget is the root of your application.
  @override
  Widget build(BuildContext context) {
    return new MaterialApp(
      title: "Draw a Graph!",
      theme: new ThemeData(
        primarySwatch: Colors.indigo,
      ),
      home: new GraphPickerPage(),
    );
  }
}

class GraphPickerPage extends StatefulWidget {
  @override
  createState() => new GraphPickerPageState();
}

class GraphPickerPageState extends State<GraphPickerPage> {
  @override
  Widget build(BuildContext context) {
    return new Scaffold(
      appBar: new AppBar(
        title: new Text("Draw a Graph!"),
        actions: [
          new IconButton(
            icon: new Icon(Icons.more_horiz),
            onPressed: null,
          ),
        ],
      ),
      drawer: new Drawer(
        semanticLabel: "Side Menu",
      ),
      floatingActionButton: new FloatingActionButton(
        tooltip: 'New Graph',
        child: new Icon(Icons.add),
      ),
    );
  }
}
