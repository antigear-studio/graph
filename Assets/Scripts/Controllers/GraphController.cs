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
    IGridViewDelegate {
        public GraphGridViewController graphGridViewController;

        public GraphStore graphStore;
        public DrawingView drawingView;
        public AppBarView appBarView;
        public MaterialNavDrawer navigationSideBar;

        // Put this to model later
        GraphTile openGraphTile;

        void Awake() {
            // Application specific settings go here.
            Application.targetFrameRate = 60;
        }

        void Start() {
            graphGridViewController.SetGraphStore(graphStore);
            appBarView.appBarViewDelegate = this;
            drawingView.gameObject.SetActive(false);

            bool success = graphStore.LoadAllFromDisk();

            if (!success) {
                Debug.LogError("Unable to load graphs from disk!");
            }
        }

        public void OnCreateGraphPress() {
            graphStore.CreateGraph();
            List<int> index = new List<int> {graphStore.GetGraphs().Count - 1};
            graphGridViewController.gridView.InsertItems(index, true);
        }

        void OpenGraphAnimation(GraphTile clickedTile) {
            // Animating change.
            drawingView.gameObject.SetActive(true);
            drawingView.SetExpansion(true, true, clickedTile);
            appBarView.SetLeftButton(AppBarView.LeftButtonType.CloseButton,
                true);
            appBarView.SetShadowDepth(false, true);
            appBarView.SetToolbarVisibility(true, true);
        }

        #region IAppBarViewDelegate implementation

        public void OnCloseButtonClick(Button clickedButton) {
            // Dismisses graph.
            GraphTile cached = openGraphTile;
            drawingView.SetExpansion(false, true, openGraphTile, 
                () => {
                    drawingView.gameObject.SetActive(false);
                    cached.SetOverlayVisibility(true, true);
                });
            appBarView.SetLeftButton(AppBarView.LeftButtonType.NavigationButton,
                true);
            appBarView.SetShadowDepth(true, true);
            appBarView.SetToolbarVisibility(false, true);
            openGraphTile = null;

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

    }
}
