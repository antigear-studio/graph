using MaterialUI;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace Antigear.Graph {
    /// <summary>
    /// Manages the main interface, including drawing management, syncing, and
    /// settings.
    /// </summary>
    public class GraphController : MonoBehaviour, IAppBarViewDelegate, 
    IGridViewDelegate, IGraphGridViewControllerDelegate {
        public GraphGridViewController graphGridViewController;

        public GraphStore graphStore;
        public DrawingView drawingView;
        public AppBarView appBarView;
        public ToolbarView toolbarView;
        public MaterialNavDrawer navigationSideBar;
        public MaterialButton newGraphButton;

        // Put this to model later
        int editingGraphIndex;

        void Awake() {
            // Application specific settings go here.
            Application.targetFrameRate = 60;
        }

        void Start() {
            graphGridViewController.SetGraphStore(graphStore);
            graphGridViewController.controllerDelegate = this;
            appBarView.appBarViewDelegate = this;
            drawingView.gameObject.SetActive(false);

            bool success = graphStore.LoadAllFromDisk();

            if (!success) {
                Debug.LogError("Unable to load graphs from disk!");
            }
        }

        void OpenGraphAnimation(GraphTile clickedTile, Action callback = null) {
            // Animating change.
            drawingView.gameObject.SetActive(true);
            drawingView.SetExpansion(true, true, clickedTile, callback);
            appBarView.SetLeftButton(AppBarView.LeftButtonType.CloseButton,
                true);
            appBarView.SetMinimized(true, true);
            toolbarView.SetToolbarVisibility(true, true);
            newGraphButton.Dismiss(true, 0.5f, 0.1f);
        }

        public void OnCreateGraphPress() {
            List<int> index = new List<int> {graphStore.CreateGraph()};
            OpenGraphAnimation(null, () => 
                graphGridViewController.gridView.InsertItems(index, false));
            editingGraphIndex = index[0];
        }

        #region IAppBarViewDelegate implementation

        public void OnCloseButtonClick(Button clickedButton) {
            // TODO: Probably should also scroll to the tile.
            // Dismisses graph.
            GraphTile tile = (GraphTile)graphGridViewController
                .gridView.CellForItem(editingGraphIndex);
            if (tile != null)
                tile.preventFromDequeue = true;
            drawingView.SetExpansion(false, true, tile, 
                () => {
                    drawingView.gameObject.SetActive(false);
                    if (tile != null) {
                        tile.SetOverlayVisibility(true, true);
                        tile.preventFromDequeue = false;
                    }
                });
            appBarView.SetLeftButton(AppBarView.LeftButtonType.NavigationButton,
                true);
            appBarView.SetMinimized(false, true);
            toolbarView.SetToolbarVisibility(false, true);
            editingGraphIndex = -1;
            newGraphButton.Show(true, 0.5f, 0.1f);
            // Save graphs.
            graphStore.SaveAllToDisk();
        }

        public void OnNavigationButtonClick(Button clickedButton) {
            // Opens the navigation side bar.
            navigationSideBar.Open();
        }

        public void OnMoreButtonClick(Button clickedButton) {
            // TODO
        }

        #endregion

        #region IGraphGridViewControllerDelegate implementation

        public void OpenGraph(int index) {
            GraphTile clickedTile = 
                (GraphTile)graphGridViewController.gridView.CellForItem(index);
            OpenGraphAnimation(clickedTile);
            editingGraphIndex = index;
        }

        #endregion
    }
}
