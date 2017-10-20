using MaterialUI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        void OpenGraphAnimation(GraphTile clickedTile) {
            // Animating change.
            drawingView.gameObject.SetActive(true);
            drawingView.SetExpansion(true, true, clickedTile);
            appBarView.SetLeftButton(AppBarView.LeftButtonType.CloseButton,
                true);
            appBarView.SetMinimized(true, true);
            toolbarView.SetToolbarVisibility(true, true);
        }

        #region IAppBarViewDelegate implementation

        public void OnCloseButtonClick(Button clickedButton) {
            // Dismisses graph.
            GraphTile tile = (GraphTile)graphGridViewController
                .gridView.CellForItem(editingGraphIndex);
            tile.preventFromDequeue = true;
            drawingView.SetExpansion(false, true, tile, 
                () => {
                    drawingView.gameObject.SetActive(false);
                    tile.SetOverlayVisibility(true, true);
                    tile.preventFromDequeue = false;
                });
            appBarView.SetLeftButton(AppBarView.LeftButtonType.NavigationButton,
                true);
            appBarView.SetMinimized(false, true);
            toolbarView.SetToolbarVisibility(false, true);
            editingGraphIndex = -1;
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
            drawingView.gameObject.SetActive(true);
            drawingView.SetExpansion(true, true, clickedTile);
            appBarView.SetLeftButton(AppBarView.LeftButtonType.CloseButton,
                true);
            appBarView.SetMinimized(true, true);
            toolbarView.SetToolbarVisibility(true, true);
            editingGraphIndex = index;
        }

        #endregion
    }
}
