import "dart:async";

import "package:flutter/material.dart";
import "package:collection/collection.dart";

import "graph.dart";
import "graph_editor_page.dart";
import "graph_manager.dart";

class GraphPickerPage extends StatefulWidget {
  @override
  createState() => new GraphPickerPageState();
}

enum SortOrder { lastModified, alphabetical, created }

class GraphPickerPageState extends State<GraphPickerPage> {
  /// The graph manager.
  GraphManager _graphManager = new GraphManager();

  /// The sort order for graphs. By default this sorts with last modified date.
  SortOrder _sortOrder = SortOrder.lastModified;

  /// Whether the sort is in ascending or descending order. By default this is
  /// ascending. To be more useful, dates are actually in reverse. In ascending,
  /// more recent dates are placed before older dates.
  bool _sortAscending = true;

  @override
  void initState() {
    super.initState();
    _reloadGraphs();
  }

  /// Reloads the graphs from graph store and refreshes state after loading.
  Future<void> _reloadGraphs() async {
    await _graphManager.loadGraphs();
    setState(() {});
  }

  @override
  Widget build(BuildContext context) {
    return new Scaffold(
      appBar: _buildAppBar(context),
      drawer: new Drawer(
        semanticLabel: "Side Menu",
      ),
      body: _buildGridView(context),
      floatingActionButton: new FloatingActionButton(
        tooltip: 'New Graph',
        child: new Icon(Icons.add),
        onPressed: _onNewGraphButtonPressed,
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

  /// Event handler for floating action button press.
  void _onNewGraphButtonPressed() {
    setState(() {
      _graphManager.createGraph();
      _sortGraphStore(_sortOrder, _sortAscending);
    });
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

  /// Event handler for "sort bottom sheet"'s "alphabetical sort button" press.
  void _onAlphabeticalSortButtonPressed() {
    Navigator.pop(context);

    if (_sortOrder != SortOrder.alphabetical) {
      setState(() {
        _sortOrder = SortOrder.alphabetical;
        _sortAscending = true;
        _sortGraphStore(_sortOrder, _sortAscending);
      });
    } else {
      setState(() {
        _sortAscending = !_sortAscending;
        _sortGraphStore(_sortOrder, _sortAscending);
      });
    }
  }

  /// Event handler for "sort bottom sheet"'s "creation date sort button" press.
  void _onCreationDateSortButtonPressed() {
    Navigator.pop(context);

    if (_sortOrder != SortOrder.created) {
      setState(() {
        _sortOrder = SortOrder.created;
        _sortAscending = true;
        _sortGraphStore(_sortOrder, _sortAscending);
      });
    } else {
      setState(() {
        _sortAscending = !_sortAscending;
        _sortGraphStore(_sortOrder, _sortAscending);
      });
    }
  }

  /// Event handler for "sort bottom sheet"'s "last modified sort button" press.
  void _onLastModifiedDateSortButtonPressed() {
    Navigator.pop(context);

    if (_sortOrder != SortOrder.lastModified) {
      setState(() {
        _sortOrder = SortOrder.lastModified;
        _sortAscending = true;
        _sortGraphStore(_sortOrder, _sortAscending);
      });
    } else {
      setState(() {
        _sortAscending = !_sortAscending;
        _sortGraphStore(_sortOrder, _sortAscending);
      });
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

  static const maxCrossAxisExtent = 150.0;
  static const childAspectRatio = 1.0;
  static const gridSpacing = 8.0;
  static const gridMargin = 8.0;

  final _scrollController = new ScrollController();

  /// Builds the grid view that displays all the graphs.
  Widget _buildGridView(BuildContext context) {
    var gridDelegate = new SliverGridDelegateWithMaxCrossAxisExtent(
      maxCrossAxisExtent: maxCrossAxisExtent,
      childAspectRatio: childAspectRatio,
      crossAxisSpacing: gridSpacing,
      mainAxisSpacing: gridSpacing,
    );
    int itemCount = _graphManager.graphStore.length;

    return new GridView.builder(
      itemCount: itemCount,
      gridDelegate: gridDelegate,
      controller: _scrollController,
      padding: const EdgeInsets.all(gridMargin),
      itemBuilder: _gridItemBuilder,
    );
  }

  static TextStyle _subtitleStyle = new TextStyle(fontSize: 12.0);

  /// Builds a single graph grid tile.
  Widget _gridItemBuilder(BuildContext context, int index) {
    Graph graph = _graphManager.graphStore[index];
    String title =
        graph.title == null || graph.title == "" ? "Untitled" : graph.title;
    String subtitle = _dateTimeToString(graph.updatedOn.toLocal());

    return new GridTile(
      child: new Container(
        color: Colors.white,
        child: new FlatButton(
          onPressed: () => _onGraphGridTilePressed(index),
          child: null,
        ),
      ),
      footer: new GridTileBar(
        title: new Text(title),
        subtitle: new Text(
          subtitle,
          style: _subtitleStyle,
        ),
        backgroundColor: Colors.black26,
      ),
    );
  }

  /// Converts a date time into a short string.
  String _dateTimeToString(DateTime t) {
    return "${t.hour}:${t.minute} ${t.year}-${t.month}-${t.day}";
  }

  /// Event handler for graph grid selection.
  void _onGraphGridTilePressed(int index) {
    print("Graph grid tile at index $index is pressed!");
    var graph = _graphManager.graphStore[index];
    Navigator.push(
      context,
      new MaterialPageRoute(builder: (context) => new GraphEditorPage(graph)),
    );
  }

  /// Sorts the graph store according to the given order.
  void _sortGraphStore(SortOrder order, bool isAscending) {
    _graphManager.graphStore.sort((a, b) {
      int sign = isAscending ? 1 : -1;

      switch (order) {
        case SortOrder.alphabetical:
          return compareNatural(a.title, b.title) * sign;
        case SortOrder.lastModified:
          return b.updatedOn.compareTo(a.updatedOn) * sign;
        case SortOrder.created:
          return b.createdOn.compareTo(a.createdOn) * sign;
      }
    });
  }
}
