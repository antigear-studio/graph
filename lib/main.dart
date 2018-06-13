import "package:flutter/material.dart";

import 'package:graph/graph_picker_page.dart';

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
