import "package:flutter/material.dart";

class GraphPickerPage extends StatefulWidget {
  @override
  createState() => new GraphPickerPageState();
}

enum SortOrder { lastModified, alphabetical, created }

class GraphPickerPageState extends State<GraphPickerPage> {
  /// The sort order for graphs. By default this sorts with last modified date.
  SortOrder _sortOrder = SortOrder.lastModified;

  /// Whether the sort is in ascending or descending order. By default this is
  /// ascending. To enforce consistency, we treat dates in lastModified in
  /// reverse. That is, we sort by using the difference of time between now and
  /// graph's last edit timestamp, instead of its last edit timestamp directly.
  bool _sortAscending = true;

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
    Navigator.pop(context);

    if (_sortOrder != SortOrder.alphabetical) {
      setState(() {
        _sortOrder = SortOrder.alphabetical;
        _sortAscending = true;
      });
    } else {
      setState(() => _sortAscending = !_sortAscending);
    }
  }

  void _onCreationDateSortButtonPressed() {
    Navigator.pop(context);

    if (_sortOrder != SortOrder.created) {
      setState(() {
        _sortOrder = SortOrder.created;
        _sortAscending = true;
      });
    } else {
      setState(() => _sortAscending = !_sortAscending);
    }
  }

  void _onLastModifiedDateSortButtonPressed() {
    Navigator.pop(context);
    
    if (_sortOrder != SortOrder.lastModified) {
      setState(() {
        _sortOrder = SortOrder.lastModified;
        _sortAscending = true;
      });
    } else {
      setState(() => _sortAscending = !_sortAscending);
    }
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
    Icon alphabeticalIcon = new Icon(null);
    Icon lastModifiedIcon = new Icon(null);
    Icon createdIcon = new Icon(null);
    IconData arrow = _sortAscending ? Icons.arrow_upward : Icons.arrow_downward;

    switch (_sortOrder) {
      case SortOrder.alphabetical:
        alphabeticalIcon = new Icon(arrow);
        break;
      case SortOrder.lastModified:
        lastModifiedIcon = new Icon(arrow);
        break;
      case SortOrder.created:
        createdIcon = new Icon(arrow);
        break;
    }

    return new BottomSheet(
      onClosing: () {},
      builder: (context) => new Column(
            children: <Widget>[
              new ListTile(
                title: new Text("Sort by..."),
              ),
              new ListTile(
                leading: alphabeticalIcon,
                title: new Text("Alphabetical order"),
                onTap: _onAlphabeticalSortButtonPressed,
              ),
              new ListTile(
                leading: lastModifiedIcon,
                title: new Text("Last modified date"),
                onTap: _onLastModifiedDateSortButtonPressed,
              ),
              new ListTile(
                leading: createdIcon,
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
