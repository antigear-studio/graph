using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Antigear.Graph {
    public class GraphGridViewController : MonoBehaviour, IGridViewDelegate, 
    IGridViewDataSource, IGraphTileDelegate {
        public GraphTile graphTilePrefab;

        public GridView gridView;

        public IGraphGridViewControllerDelegate controllerDelegate;

        const string REUSE_ID = "graph_tile";

        // Non-owner.
        GraphStore graphStore;

        public void SetGraphStore(GraphStore store) {
            graphStore = store;
            gridView.dataSource = this;
            gridView.eventDelegate = this;
            gridView.RegisterCellWithReuseIdentifier(graphTilePrefab, REUSE_ID);
            gridView.ReloadData();
        }

        public void OnCreateGraphPress() {
            List<int> index = new List<int> {graphStore.CreateGraph()};
            gridView.InsertItems(index, true);
        }

        #region IGridViewDataSource implementation
        public int NumberOfItems(GridView gridView) {
            return graphStore.GetGraphs().Count;
        }

        public GridViewCell CellForIndex(GridView gridView, int index) {
            GraphTile t = gridView.DequeueReusableCell(REUSE_ID) as GraphTile;
            t.UpdateCellWithGraph(graphStore.GetGraphs()[index]);
            t.graphTileDelegate = this;

            return t;
        }

        #endregion

        #region IGraphTileDelegate implementation

        public void OnGraphTileClick(GraphTile clickedTile) {
            if (controllerDelegate != null) {
                int index = gridView.IndexForItem(clickedTile);
                controllerDelegate.OpenGraph(index);
            }
        }

        #endregion
    }

    public interface IGraphGridViewControllerDelegate {
        /// <summary>
        /// Graph at index has been selected.
        /// </summary>
        /// <param name="index">Index.</param>
        void OpenGraph(int index);
    }
}