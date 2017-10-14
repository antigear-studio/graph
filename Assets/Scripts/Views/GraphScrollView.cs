﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Antigear.Graph {
    /// <summary>
    /// Manages the look of the scroll view in the graph selection interface.
    /// </summary>
    public class GraphScrollView : MonoBehaviour, IGraphTileDelegate {
        public GridLayoutGroup gridLayoutGroup;

        public IGraphScrollViewDelegate graphScrollViewDelegate;
        public float cardWidthHeightRatio = 0.8f;
        public float minInchesPerColumn = 2.3f;
        public int minColumns = 2;
        public int maxColumns = 5;

        void Start() {
            int n = (int)(DeviceDiagonalSize() / minInchesPerColumn);
            gridLayoutGroup.constraintCount = 
                Mathf.Max(minColumns, Mathf.Min(maxColumns, n));

            // Testing. Since we are not dynamically loading graphs, we don't
            // create any here. But we still need to link delegate methods.
            GraphTile[] tiles = 
                gridLayoutGroup.GetComponentsInChildren<GraphTile>(true);
            foreach (GraphTile tile in tiles) {
                tile.graphTileDelegate = this;
            }
        }

        void Update() {
            RectTransform t = transform as RectTransform;
            float width = t.rect.width;
            Vector2 v = new Vector2();
            float space = (gridLayoutGroup.constraintCount - 1) *
                          gridLayoutGroup.spacing.x +
                          gridLayoutGroup.padding.left +
                          gridLayoutGroup.padding.right;
            v.x = (width - space) / gridLayoutGroup.constraintCount;
            v.y = v.x / cardWidthHeightRatio;
            gridLayoutGroup.cellSize = v;
        }

        public static float DeviceDiagonalSize() {
            float screenWidth = Screen.width / Screen.dpi;
            float screenHeight = Screen.height / Screen.dpi;
            float diagonalInches = 
                Mathf.Sqrt(Mathf.Pow(screenWidth, 2) +
                Mathf.Pow(screenHeight, 2));
            
            return diagonalInches;
        }

        #region IGraphTileDelegate implementation

        public void OnGraphTileClick(GraphTile clickedTile) {
            if (graphScrollViewDelegate != null) {
                graphScrollViewDelegate.OnGraphTileClick(clickedTile);
            }
        }

        #endregion
    }

    /// <summary>
    /// Defines methods that must be implemented for scroll view to know what to
    /// display and how to handle events.
    /// </summary>
    public interface IGraphScrollViewDelegate {
        /// <summary>
        /// Raises the graph tile clicked event. Triggered when the graph tile's
        /// main action button is triggered.
        /// </summary>
        /// <param name="clickedTile">Clicked tile.</param>
        void OnGraphTileClick(GraphTile clickedTile);
    }
}
