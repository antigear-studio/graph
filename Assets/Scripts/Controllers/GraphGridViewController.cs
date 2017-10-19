using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Antigear.Graph {
    public class GraphGridViewController : MonoBehaviour, IGridViewDelegate, 
    IGridViewDataSource {
        public GraphTile graphTilePrefab;

        public GridView gridView;

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

        #region IGridViewDataSource implementation
        public int NumberOfItems(GridView gridView) {
            return graphStore.GetGraphs().Count;
        }

        public GridViewCell CellForIndex(GridView gridView, int index) {
            GraphTile t = gridView.DequeueReusableCell(REUSE_ID) as GraphTile;
            t.UpdateCellWithGraph(graphStore.GetGraphs()[index]);

            return t;
        }

        #endregion
        
}
}