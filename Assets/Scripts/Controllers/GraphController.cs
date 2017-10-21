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
    IGridViewDelegate, IGraphGridViewControllerDelegate, 
    IDrawingControllerDelegate {
        public GraphGridViewController graphGridViewController;
        public DrawingController drawingController;

        public GraphStore graphStore;
        public AppBarView appBarView;
        public MaterialNavDrawer navigationSideBar;
        public MaterialButton newGraphButton;
        public BottomSheet moreBottomSheet;

        // Put this to model later
        int editingGraphIndex;

        void Awake() {
            // Application specific settings go here.
            Application.targetFrameRate = 60;
        }

        void Start() {
            graphGridViewController.SetGraphStore(graphStore);
            graphGridViewController.controllerDelegate = this;
            appBarView.viewDelegate = this;
            drawingController.controllerDelegate = this;

            bool success = graphStore.LoadAllFromDisk();

            if (!success) {
                Debug.LogError("Unable to load graphs from disk!");
            }
        }

        void OpenGraphAnimation() {
            // Animating change.
            appBarView.SetLeftButton(AppBarView.LeftButtonType.CloseButton,
                true);
            appBarView.SetMinimized(true, true);
            newGraphButton.Dismiss(true, 0.4f, 0.15f);
        }

        public void OnCreateGraphPress() {
            List<int> index = new List<int> { graphStore.CreateGraph() };
            Graph graph = graphStore.GetGraphs()[index[0]];
            drawingController.OpenGraph(graph, true, null, () => 
                graphGridViewController.gridView.InsertItems(index, false));
            OpenGraphAnimation();
            editingGraphIndex = index[0];
        }

        #region IAppBarViewDelegate implementation

        public void OnCloseButtonClick(Button clickedButton) {
            // TODO: Probably should also scroll to the tile.
            // Dismisses graph.
            GraphTile tile = graphGridViewController
                .gridView.CellForItem(editingGraphIndex) as GraphTile;
            RectTransform tileTransform = null;

            if (tile != null) {
                tile.preventFromDequeue = true;
                tileTransform = tile.transform as RectTransform;
            }
            
            drawingController.CloseGraph(true, tileTransform, () => {
                if (tile != null) {
                    tile.SetOverlayVisibility(true, true);
                    tile.preventFromDequeue = false;
                }
            });

            appBarView.SetLeftButton(AppBarView.LeftButtonType.NavigationButton,
                true);
            appBarView.SetMinimized(false, true);
            editingGraphIndex = -1;
            newGraphButton.Show(true, 0.4f, 0.1f);
            // Save graphs.
            graphStore.SaveAllToDisk();
        }

        public void OnNavigationButtonClick(Button clickedButton) {
            // Opens the navigation side bar.
            navigationSideBar.Open();
        }

        public void OnMoreButtonClick(Button clickedButton) {
            moreBottomSheet.Show(true);
        }

        #endregion

        #region IGraphGridViewControllerDelegate implementation

        public void OpenGraph(int index) {
            Graph openedGraph = graphStore.GetGraphs()[index];
            RectTransform clickedTile = graphGridViewController
                .gridView.CellForItem(index).transform as RectTransform;
            drawingController.OpenGraph(openedGraph, true, clickedTile);
            OpenGraphAnimation();
            editingGraphIndex = index;
        }

        #endregion

        #region IDrawingControllerDelegate implementation

        public void OnUIColorChange(DrawingController controller, Color color) {
            appBarView.drawingViewButtonColor = color;
        }

        #endregion
    }
}
