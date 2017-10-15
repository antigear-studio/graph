using MaterialUI;
using UnityEngine;
using UnityEngine.UI;

namespace Antigear.Graph {
    /// <summary>
    /// Manages the main interface, including drawing management, syncing, and
    /// settings.
    /// </summary>
    public class GraphController : MonoBehaviour, IGraphScrollViewDelegate,
    IAppBarViewDelegate, IGraphScrollViewDataSource {
        public GraphStore graphStore;

        public GraphScrollView graphScrollView;
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
            graphScrollView.graphScrollViewDelegate = this;
            graphScrollView.graphScrollViewDataSource = this;
            appBarView.appBarViewDelegate = this;
            drawingView.gameObject.SetActive(false);

            bool success = graphStore.LoadAllFromDisk();

            if (!success) {
                Debug.LogError("Unable to load graphs from disk!");
            }

            graphScrollView.UpdateTiles();
        }

        public void OnCreateGraphPress() {
            graphStore.CreateGraph();
            graphScrollView.UpdateTiles();
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

        #region IGraphScrollViewDelegate implementation

        public void OnGraphTileClick(GraphTile clickedTile) {
            // TODO: Checks if we are in selection mode. If not, start editing!

            // Animating change.
            OpenGraphAnimation(clickedTile);

            openGraphTile = clickedTile;
            drawingView.editingGraph = graphStore.GetGraphs()[0];
        }

        #endregion

        #region IGraphScrollViewDataSource implementation

        public Graph GraphForTileAtIndex(int index) {
            return graphStore.GetGraphs()[index];
        }

        public int NumberOfTiles() {
            return graphStore.GetGraphs().Count;
        }

        #endregion

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
