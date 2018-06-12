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
            onPressed: () => showModalBottomSheet(
                context: context,
                builder: (context) => new BottomSheet(
                    onClosing: () {},
                    builder: (context) => new Column(
                      children: <Widget>[
                        new Container(
                          height: 56.0,
                          child: new FlatButton(
                            onPressed: () => debugPrint("Search pressed!"),
                            child: new Row(
                              mainAxisSize: MainAxisSize.max,
                              children: <Widget>[
                                new Icon(Icons.search),
                                new SizedBox(width: 20.0),
                                new Expanded(
                                    child: new Text(
                                        "Search",
                                      style: new TextStyle(
                                        fontSize: 16.0,
                                      ),
                                    )
                                ),
                              ],
                            ),
                          ),
                        ),
                        new Container(
                          height: 56.0,
                          child: new FlatButton(
                            onPressed: () => debugPrint("Sort pressed!"),
                            child: new Row(
                              mainAxisSize: MainAxisSize.max,
                              children: <Widget>[
                                new Icon(Icons.sort_by_alpha),
                                new SizedBox(width: 20.0),
                                new Expanded(
                                    child: new Text(
                                      "Sort...",
                                      style: new TextStyle(
                                        fontSize: 16.0,
                                      ),
                                    )
                                ),
                              ],
                            ),
                          ),
                        ),
                      ],
                      mainAxisSize: MainAxisSize.min,
                      crossAxisAlignment: CrossAxisAlignment.stretch,
                    ),
                ),
            ),
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
