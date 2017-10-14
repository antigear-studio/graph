using UnityEngine;
using UnityEngine.UI;

namespace Antigear.Graph {
    /// <summary>
    /// Manages the main interface, including drawing management, syncing, and
    /// settings.
    /// </summary>
    public class GraphController : MonoBehaviour, IGraphScrollViewDelegate,
        IAppBarViewDelegate {
        public GraphScrollView graphScrollView;
        public DrawingView drawingView;
        public AppBarView appBarView;

        // Put this to model later
        GraphTile openGraphTile;

        void Awake() {
            // Application specific settings go here.
            Application.targetFrameRate = 60;
        }

        void Start() {
            graphScrollView.graphScrollViewDelegate = this;
            appBarView.appBarViewDelegate = this;
        }

        #region IGraphScrollViewDelegate implementation

        public void OnGraphTileClick(GraphTile clickedTile) {
            // TODO: Checks if we are in selection mode. If not, start editing!
            drawingView.gameObject.SetActive(true);
            drawingView.SetExpansion(true, true, clickedTile);
            appBarView.SetLeftButton(AppBarView.LeftButtonType.CloseButton,
                true);
            appBarView.SetShadowDepth(false, true);
            appBarView.SetToolbarVisibility(true, true);
            openGraphTile = clickedTile;
        }

        #endregion

        #region IAppBarViewDelegate implementation

        public void OnCloseButtonClick(Button clickedButton) {
            // Dismisses graph.
            drawingView.SetExpansion(false, true, openGraphTile, 
                () => drawingView.gameObject.SetActive(false));
            appBarView.SetLeftButton(AppBarView.LeftButtonType.NavigationButton,
                true);
            appBarView.SetShadowDepth(true, true);
            appBarView.SetToolbarVisibility(false, true);
            openGraphTile = null;
        }

        #endregion
    }
}
