import "package:flutter/material.dart";

class GraphPickerPage extends StatefulWidget {
  @override
  createState() => new GraphPickerPageState();
}

class GraphPickerPageState extends State<GraphPickerPage> {
  @override
  Widget build(BuildContext context) {
    return new Scaffold(
      appBar: _buildAppBar(context),
      drawer: new Drawer(
        semanticLabel: "Side Menu",
      ),
      floatingActionButton: new FloatingActionButton(
        tooltip: 'New Graph',
        child: new Icon(Icons.add),
        onPressed: () => debugPrint("Add new drawing!"),
      ),
    );
  }

  /// Builds and returns the app bar of the graph picker page.
  AppBar _buildAppBar(BuildContext context) {
    return new AppBar(
      title: new Text("Draw a Graph!"),
      actions: [
        new IconButton(
          icon: new Icon(Icons.more_horiz),
          onPressed: _onMoreButtonPressed,
        ),
      ],
    );
  }

  /// Event handler for app bar "more button" press.
  void _onMoreButtonPressed() {
    showModalBottomSheet(context: context, builder: _buildMoreBottomSheet);
  }

  /// Event handler for "more bottom sheet"'s "search button" press.
  void _onSearchButtonPressed() {
    Navigator.pop(context);
    print("Search button is pressed!");
  }

  /// Event handler for "more bottom sheet"'s "sort button" press.
  void _onSortButtonPressed() {
    Navigator.pop(context);
    showModalBottomSheet(context: context, builder: _buildSortBottomSheet);
  }

  /// Event handler for "more bottom sheet"'s "select button" press.
  void _onSelectButtonPressed() {
    Navigator.pop(context);
    print("Select button is pressed!");
  }

  void _onAlphabeticalSortButtonPressed() {

  }

  void _onCreationDateSortButtonPressed() {

  }

  void _onLastModifiedDateSortButtonPressed() {

  }

  /// Builds the bottom sheet that is shown after user taps the "more button" on
  /// the app bar.
  BottomSheet _buildMoreBottomSheet(BuildContext context) {
    return new BottomSheet(
      onClosing: () {},
      builder: (context) => new Column(
            children: <Widget>[
              new ListTile(
                leading: new Icon(Icons.search),
                title: new Text("Search"),
                onTap: _onSearchButtonPressed,
              ),
              new ListTile(
                leading: new Icon(Icons.center_focus_strong),
                title: new Text("Select..."),
                onTap: _onSelectButtonPressed,
              ),
              new ListTile(
                leading: new Icon(Icons.sort_by_alpha),
                title: new Text("Sort..."),
                onTap: _onSortButtonPressed,
              ),
            ],
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.stretch,
          ),
    );
  }

  /// Builds the bottom sheet that is shown after user taps the "sort button" on
  /// the "more bottom sheet".
  BottomSheet _buildSortBottomSheet(BuildContext context) {
    return new BottomSheet(
      onClosing: () {},
      builder: (context) => new Column(
            children: <Widget>[
              new ListTile(
                title: new Text("Sort by..."),
              ),
              new ListTile(
                leading: new Icon(null),
                title: new Text("Alphabetical order"),
                onTap: _onAlphabeticalSortButtonPressed,
              ),
              new ListTile(
                leading: new Icon(Icons.arrow_upward),
                title: new Text("Last modified date"),
                onTap: _onLastModifiedDateSortButtonPressed,
              ),
              new ListTile(
                leading: new Icon(Icons.arrow_downward),
                title: new Text("Creation date"),
                onTap: _onCreationDateSortButtonPressed,
              ),
            ],
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.stretch,
          ),
    );
  }
}
