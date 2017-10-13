using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Antigear.Graph {
    /// <summary>
    /// Manages the look of the scroll view in the graph selection interface.
    /// </summary>
    public class GraphScrollView : MonoBehaviour {
        public GridLayoutGroup gridLayoutGroup;

        public float cardWidthHeightRatio = 0.8f;
        public float minInchesPerColumn = 2.3f;
        public int minColumns = 2;
        public int maxColumns = 5;

        void Start() {
            int n = (int)(DeviceDiagonalSize() / minInchesPerColumn);
            gridLayoutGroup.constraintCount = 
                Mathf.Max(minColumns, Mathf.Min(maxColumns, n));
        }

        void Update() {
            RectTransform t = transform as RectTransform;
            float width = t.rect.width;
            Vector2 v = new Vector2();
            float space = (gridLayoutGroup.constraintCount - 1) * 
                gridLayoutGroup.spacing.x + gridLayoutGroup.padding.left + 
                gridLayoutGroup.padding.right;
            v.x = (width - space) / gridLayoutGroup.constraintCount;
            v.y = v.x / cardWidthHeightRatio;
            gridLayoutGroup.cellSize = v;
        }

        public static float DeviceDiagonalSize() {
            float screenWidth = Screen.width / Screen.dpi;
            float screenHeight = Screen.height / Screen.dpi;
            float diagonalInches = Mathf.Sqrt(
                Mathf.Pow(screenWidth, 2) + Mathf.Pow(screenHeight, 2));
            
            return diagonalInches;
        }
    }
}