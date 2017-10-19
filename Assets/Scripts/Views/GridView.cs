using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Antigear.Graph {
    /// <summary>
    /// A generic framework with displaying items in a grid in a dynamic and
    /// responsive way.
    /// </summary>
    public class GridView : MonoBehaviour {
        public IGridViewDataSource dataSource;
        public IGridViewDelegate eventDelegate;
        public GameObject backgroundView;
        public readonly Dictionary<string, GridViewCell> cellPrefabs = 
            new Dictionary<string, GridViewCell>();

        // Private variables, but linked through inspector for simplicity.
        public Transform cellContainer;
        public Transform recycledCellContainer;
        public UnityEngine.UI.ScrollRect scrollRect;

        // Exposed stuff.
        public float minimumCellWidth = 100;
        public float cellWidthToHeightRatio = 1;
        public Vector2 spacing;
        public Vector4 padding;

        readonly Dictionary<int, GridViewCell> visibleCellForIndex = 
            new Dictionary<int, GridViewCell>();

        readonly Dictionary<string, Queue<GridViewCell>> reusableCells = 
            new Dictionary<string, Queue<GridViewCell>>();

        /// <summary>
        /// Registers the given prefab with the given reuse identifier.
        /// </summary>
        /// <param name="prefab">Prefab to link to identifier.</param>
        /// <param name="identifier">Identifier.</param>
        public void RegisterCellWithReuseIdentifier(GridViewCell prefab, 
            string identifier) {
            cellPrefabs[identifier] = prefab;
        }

        /// <summary>
        /// Dequeues a reusable cell if one is available. Otherwise creates a
        /// new cell.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        public GridViewCell DequeueReusableCell(string identifier) {
            GridViewCell cell = null;

            if (reusableCells.ContainsKey(identifier) && 
                reusableCells[identifier].Count > 0) {
                // Dequeue and call PrepareForReuse.
                cell = reusableCells[identifier].Dequeue();
                cell.PrepareForReuse();
                cell.reuseIdentifier = identifier;
            } else if (cellPrefabs.ContainsKey(identifier)) {
                // Creates a new cell.
                cell = Instantiate(cellPrefabs[identifier]);
                cell.reuseIdentifier = identifier;
            }

            return cell;
        }

        public int NumberOfItems() {
            return dataSource.NumberOfItems(this);
        }

        /// <summary>
        /// Returns a list of cells currently visible on the screen.
        /// </summary>
        /// <returns>The cells currently displayed on the screen.</returns>
        public List<GridViewCell> VisibleCells() {
            return 
                new List<GridViewCell>(GetComponentsInChildren<GridViewCell>());
        }

        /// <summary>
        /// Performs recalculation on the visible cells, be it deletion,
        /// insertion, or reordering due to content or screen bound changes.
        /// </summary>
        /// <param name="animated">If set to <c>true</c> animated.</param>
        public void UpdateVisibleCellLayout(bool animated) {
            // TODO - the main meat chunk of this class.
        }

        /// <summary>
        /// Returns the index of the item contained in the point, given at
        /// screen coordinates. If no such item exists, then returns -1.
        /// </summary>
        /// <returns>The index for item at the point.</returns>
        /// <param name="point">Point in screen coordinates.</param>
        public int IndexForItem(Vector2 point) {
            // TODO
            return -1;
        }

        /// <summary>
        /// Returns a list of indices for all visible items.
        /// </summary>
        /// <returns>The indices for visible items.</returns>
        public List<int> IndexPathsForVisibleItems() {
            return new List<int>(visibleCellForIndex.Keys);
        }

        /// <summary>
        /// Returns an index for the given cell, if the cell is part of the grid
        /// view. Otherwise returns -1. Since cells are dequeued when they move 
        /// out of view, this only applies to ones visible on the screen.
        /// </summary>
        /// <returns>The index for given item.</returns>
        /// <param name="cell">Cell.</param>
        public int IndexForItem(GridViewCell cell) {
            foreach (int index in visibleCellForIndex.Keys) {
                if (visibleCellForIndex[index] == cell) {
                    return index;
                }
            }

            return -1;
        }

        /// <summary>
        /// Returns the visible cell at the given index. If the indexed cell is
        /// not visible, returns null.
        /// </summary>
        /// <returns>The cell at given index.</returns>
        /// <param name="index">Index.</param>
        public GridViewCell CellForItem(int index) {
            if (visibleCellForIndex.ContainsKey(index)) {
                return visibleCellForIndex[index];
            }

            return null;
        }

        /// <summary>
        /// Reloads all data.
        /// </summary>
        public void ReloadData() {
            // We always load what is required. We first look at how many items
            // we can fit on screen.
            RectTransform rectTransform = transform as RectTransform;
            RectTransform contentRectTransform = 
                cellContainer.transform as RectTransform;
            Rect displayRect = rectTransform.rect;
            int columns = (int)((displayRect.width - padding.x - padding.y + 
                spacing.x) / minimumCellWidth / (1 + spacing.x / 
                    minimumCellWidth));
            
            int n = dataSource.NumberOfItems(this);
            int rows = Mathf.CeilToInt(n / (float)columns);

            // Calculate actual cell size.
            float cellWidth = (displayRect.width - padding.x - padding.y - 
                (columns - 1) * spacing.x) / columns;
            Vector2 cellSize = new Vector2(cellWidth, 
                cellWidth / cellWidthToHeightRatio);

            // Calculate visible bounds. Max is the bottom of the screen and min
            // is the top of the screen.
            float minHeight = scrollRect.content.offsetMax.y;
            float maxHeight = minHeight + scrollRect.viewport.rect.height;

            // Set up the container view.
            float height = rows * cellSize.y + (rows - 1) * spacing.y + 
                padding.z + padding.w;
            
            Vector2 sizeDelta = contentRectTransform.sizeDelta;
            sizeDelta.y = height;
            contentRectTransform.sizeDelta = sizeDelta;

            // Remove previous cells.
            DequeueAll();

            // Now calculate which cells would be on the screen at this time.
            int rowsBefore = Mathf.Min(rows, Mathf.Max(0, 
                (int)((minHeight - padding.z) / (cellSize.y + spacing.y))));
            int rowsEnd = Mathf.Min(rows, Mathf.Max(0, Mathf.CeilToInt(
                (maxHeight - padding.z) / (cellSize.y + spacing.y))));
            int startIndex = rowsBefore * columns;
            int endIndex = rowsEnd * columns;

            // Fetch these cells.
            for (int i = startIndex; i < endIndex && i < n; i++) {
                // Insert the cell into the table view hierarchy.
                int row = i / columns;
                int col = i % columns;
                GridViewCell cell = dataSource.CellForIndex(this, i);
                visibleCellForIndex[i] = cell;
                RectTransform t = cell.transform as RectTransform;
                t.SetParent(cellContainer);
                t.anchorMin = new Vector2(0, 1);
                t.anchorMax = new Vector2(0, 1);
                t.pivot = new Vector2(0, 1);
                t.anchoredPosition = 
                    new Vector2(padding.x + (cellSize.x + spacing.x) * col, 
                                -padding.y - (cellSize.y + spacing.y) * row);
                t.sizeDelta = cellSize;
            }
        }

        /// <summary>
        /// Reloads the given items located at the paths.
        /// </summary>
        /// <param name="indexPaths">Index paths.</param>
        public void ReloadItems(List<Vector2Int> indexPaths) {
            // TODO
        }

        /// <summary>
        /// Dequeues a single item with given index from visible cells.
        /// </summary>
        /// <param name="index">Index.</param>
        void DequeueItem(int index) {
            if (visibleCellForIndex.ContainsKey(index)) {
                GridViewCell cell = visibleCellForIndex[index];
                cell.transform.SetParent(recycledCellContainer);
                visibleCellForIndex.Remove(index);

                if (!reusableCells.ContainsKey(cell.reuseIdentifier)) {
                    reusableCells[cell.reuseIdentifier] = 
                        new Queue<GridViewCell>();
                }

                reusableCells[cell.reuseIdentifier].Enqueue(cell);
            }
        }

        /// <summary>
        /// Dequeues all visible cells.
        /// </summary>
        void DequeueAll() {
            foreach (GridViewCell cell in visibleCellForIndex.Values) {
                cell.transform.SetParent(recycledCellContainer);

                if (!reusableCells.ContainsKey(cell.reuseIdentifier)) {
                    reusableCells[cell.reuseIdentifier] = 
                        new Queue<GridViewCell>();
                }

                reusableCells[cell.reuseIdentifier].Enqueue(cell);
            }

            visibleCellForIndex.Clear();
        }

        void Start() {
        }

        void Update() {
            if (dataSource != null) {
                ReloadData();
            }
        }
    }

    /// <summary>
    /// Defines methods to relay interactions to a controller.
    /// </summary>
    public interface IGridViewDelegate {
        
    }

    /// <summary>
    /// Defines methods to provide data to be displayed by this view.
    /// </summary>
    public interface IGridViewDataSource {
        int NumberOfItems(GridView gridView);

        GridViewCell CellForIndex(GridView gridView, int index);
    }
}