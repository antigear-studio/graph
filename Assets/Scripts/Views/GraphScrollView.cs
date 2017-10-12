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

        public float cardWidthHeightRatio = 0.6f;

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
    }
}