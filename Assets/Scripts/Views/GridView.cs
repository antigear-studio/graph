using MaterialUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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
        public float animationDuration = 0.3f;
        public float minimumCellWidth = 100;
        public float cellWidthToHeightRatio = 1;
        public Vector2 spacing;
        public Vector4 padding;

        Dictionary<int, GridViewCell> visibleCellForIndex = 
            new Dictionary<int, GridViewCell>();

        readonly Dictionary<string, Queue<GridViewCell>> reusableCells = 
            new Dictionary<string, Queue<GridViewCell>>();

        // Kept for scrolling calculations.
        int columns;
        int firstVisibleIndex;
        int visibleRows;
        Vector2 cellSize;

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
            CalculateColumn();

            // Calculate actual cell size.
            CalculateCellSize();

            // Set up the container view.
            UpdateContentHeight();

            // Remove previous cells.
            DequeueAll();

            // Now calculate which cells would be on the screen at this time.
            CalculateVisibleIndices();

            // Fetch these cells.
            int n = dataSource.NumberOfItems(this);

            for (int i = firstVisibleIndex; i < visibleRows * columns && i < n; 
                i++) {
                EnqueueItem(i);
            }
        }

        /// <summary>
        /// Reloads the given items located at the paths.
        /// </summary>
        /// <param name="indices">Index paths.</param>
        public void ReloadItems(List<int> indices) {
            // TODO
        }

        /// <summary>
        /// Looks for new items at the given indices and insert them into the
        /// view. If any of the items end up visible on screen, they will be
        /// added to the view.
        /// </summary>
        /// <param name="indices">Indices of the new items as part of the
        /// updated data source. E.g. if data store contains [3, 7] and we want
        /// to insert such that the end result is [3, 6, 7, 8, 9], then the list
        /// is [1, 3, 4], the list of indices of the inserted elements. </param>
        /// <param name="animated">If set to <c>true</c> animated.</param>
        public void InsertItems(List<int> indices, bool animated) {
            DeleteThenInsertItems(null, indices, animated);
        }

        /// <summary>
        /// Mark the items at the given indices for deletion.
        /// </summary>
        /// <param name="indices">Indices of the items for deletion as part of 
        /// the data source before the deletion took place. E.g. if data store
        /// contains items [3, 6, 7, 8, 9] and we want to delete [6, 8, 9], then
        /// the list is [1, 3, 4], while the new data store will be [3, 7].
        /// </param>
        /// <param name="animated">If set to <c>true</c> animated.</param>
        public void DeleteItems(List<int> indices, bool animated) {
            DeleteThenInsertItems(indices, null, animated);
        }

        /// <summary>
        /// Deletes the then inserts items. Use this method instead of separate
        /// insertions/deletions to avoid index inconsistencies.
        /// </summary>
        /// <param name="deletions">Indices of the items for deletion as part of 
        /// the data source before the deletion took place. E.g. if data store
        /// contains items [3, 6, 7, 8, 9] and we want to delete [6, 8, 9], then
        /// the list is [1, 3, 4], while the new data store will be [3, 7].
        /// </param>
        /// <param name="insertions">Indices of the new items as part of the
        /// updated data source. E.g. if data store contains [3, 7] and we want
        /// to insert such that the end result is [3, 6, 7, 8, 9], then the list
        /// is [1, 3, 4], the list of indices of the inserted elements. </param>
        /// <param name="animated">If set to <c>true</c> animated.</param>
        public void DeleteThenInsertItems(List<int> deletions, 
            List<int> insertions, bool animated) {
            // The list of new items may have several impacts on the current
            // view:
            // Anything after the visible index will change only the height.
            // Anything inside the visible range might pull cells up from the
            // bottom or push existing cells out of the bottom.
            // Anything before the visible index might pull cells into the first
            // row and change the height.

            // For each deletion and insertion, we need to modify the head and
            // tail. Ultimately, head, tail and marks on visible cells are the 
            // ones that will animate.
            int head = firstVisibleIndex;
            int headOffset = 0;
            int tailOffset = 0;

            if (deletions != null) {
                foreach (int i in deletions) {
                    if (i < head) {
                        // This cell will affect head & tail.
                        headOffset -= 1;
                    } else if (i < head + visibleRows * columns) {
                        // Dequeue this with optional animation.
                        tailOffset -= 1;
                        DequeueItem(i, animated, true);
                    }
                }
            }

            // Now we perform insertion.
            if (insertions != null) {
                foreach (int i in insertions) {
                    if (i < head) {
                        // This cell will affect head & tail.
                        headOffset += 1;
                    }
                }
            }

            // Now update visible cell indices.
            Dictionary<int, GridViewCell> offsetVisibleCellForIndex = 
                new Dictionary<int, GridViewCell>();

            foreach (int key in visibleCellForIndex.Keys) {
                offsetVisibleCellForIndex[key + headOffset] = 
                    visibleCellForIndex[key];
            }

            visibleCellForIndex = offsetVisibleCellForIndex;

            // Enqueue with animation.
            if (insertions != null) {
                foreach (int i in insertions) {
                    if (i >= head && i < head + visibleRows * columns) {
                        // Enqueue this with optional animation.
                        EnqueueItem(i, animated);
                        tailOffset += 1;
                    }
                }
            }

            int newHead = (head + headOffset) / columns * columns;

            // Adjust height.
            int rowOffset = (newHead - head) / columns;
            Vector2 pos = scrollRect.content.anchoredPosition;
            pos.y += rowOffset * (cellSize.y + spacing.y);
            UpdateContentHeight();

            int moddedHeadOffset = head + headOffset - newHead;
            tailOffset += moddedHeadOffset;

            // Update and animate change in positions for all visible cells
            // (followed by dequeue if result is offscreen).
            foreach (int key in visibleCellForIndex.Keys) {
                GridViewCell cell = visibleCellForIndex[key];
                RectTransform r = cell.transform as RectTransform;

                if (cell.translationAnimationId >= 0) {
                    TweenManager.EndTween(cell.translationAnimationId);
                    cell.translationAnimationId = -1;
                }

                Vector2 target = PositionForItem(key);

                if (animated) {
                    cell.preventFromDequeue = true;
                    cell.translationAnimationId = TweenManager.TweenVector2(
                        v => r.anchoredPosition = v, r.anchoredPosition, target,
                        animationDuration, 0, 
                        () => cell.preventFromDequeue = false);
                } else {
                    r.anchoredPosition = target;
                }
            }

            // Now generate cells from before and animate into view.
            // We actually want to enqueue the item a bit different than usual -
            // we first initialize the item at its OLD location, then ANIMATE it
            // to its new location. Of course, if animation is off, this is a
            // simple enqueue.
            for (int i = newHead; i < head + headOffset; i++) {
                EnqueueItem(i);

                if (animated) {
                    GridViewCell cell = visibleCellForIndex[i];
                    RectTransform r = cell.transform as RectTransform;
                    Vector2 start = PositionForItem(i - moddedHeadOffset);
                    Vector2 target = r.anchoredPosition;
                    cell.translationAnimationId = TweenManager.TweenVector2(
                        v => r.anchoredPosition = v, start, target, 
                        animationDuration);
                }
            }

            // Generate cells from after and animate into view.
            for (int i = -1; i >= tailOffset; i--) {
                int index = newHead + columns * visibleRows + i;
                EnqueueItem(index);

                if (animated) {
                    GridViewCell cell = visibleCellForIndex[index];
                    RectTransform r = cell.transform as RectTransform;
                    Vector2 start = PositionForItem(index - tailOffset);
                    Vector2 target = r.anchoredPosition;
                    cell.translationAnimationId = TweenManager.TweenVector2(
                        v => r.anchoredPosition = v, start, target, 
                        animationDuration);
                }
            }
        }

        void DequeueItem(int index, bool animated = false, bool force = false) {
            if (visibleCellForIndex.ContainsKey(index)) {
                GridViewCell cell = visibleCellForIndex[index];

                if (!force && cell.preventFromDequeue) {
                    return;
                }

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

        Vector2 PositionForItem(int index) {
            int row = index / columns;
            int col = index % columns;

            return new Vector2(padding.x + (cellSize.x + spacing.x) * col, 
                -padding.y - (cellSize.y + spacing.y) * row);
        }

        void EnqueueItem(int index, bool animated = false) {
            // Insert the cell into the table view hierarchy.
            GridViewCell cell = dataSource.CellForIndex(this, index);
            visibleCellForIndex[index] = cell;
            RectTransform t = cell.transform as RectTransform;
            t.SetParent(cellContainer, false);
            t.localScale = Vector3.one;
            t.anchorMin = new Vector2(0, 1);
            t.anchorMax = new Vector2(0, 1);
            t.pivot = new Vector2(0, 1);
            t.anchoredPosition = PositionForItem(index);
            t.sizeDelta = cellSize;
        }

        void CalculateColumn() {
            float width = scrollRect.viewport.rect.width;
            columns = Mathf.Max(1, (int)((width - padding.x - padding.y
                + spacing.x) / (minimumCellWidth + spacing.x)));
        }

        void CalculateCellSize() {
            float width = scrollRect.viewport.rect.width;
            float cellWidth = (width - padding.x - padding.y - (columns - 1) * 
                spacing.x) / columns;
            cellSize.x = cellWidth;
            cellSize.y = cellWidth / cellWidthToHeightRatio;
        }

        void CalculateVisibleIndices() {
            int n = dataSource.NumberOfItems(this);
            int rows = Mathf.CeilToInt(n / (float)columns);
            float minHeight = scrollRect.content.offsetMax.y;
            float maxHeight = minHeight + scrollRect.viewport.rect.height;
            int rowsBefore = Mathf.Min(rows, Mathf.Max(0, 
                (int)((minHeight - padding.z) / (cellSize.y + spacing.y))));
            int rowsEnd = Mathf.Max(0, Mathf.CeilToInt((maxHeight - padding.z) / 
                (cellSize.y + spacing.y)));
            firstVisibleIndex = 
                Mathf.Max(0, Mathf.Min(n - 1, rowsBefore * columns));
            visibleRows = rowsEnd - rowsBefore + 1;
        }

        void UpdateContentHeight() {
            int n = dataSource.NumberOfItems(this);
            int rows = Mathf.CeilToInt(n / (float)columns);
            float height = rows * cellSize.y + (rows - 1) * spacing.y + 
                padding.z + padding.w;
            scrollRect.content.sizeDelta = new Vector2(0, height);
        }

        void OnRectTransformDimensionsChange() {
            if (dataSource != null)
                // TODO: this should be animated according to a flag.
                ReloadData();
        }

        void Start() {
        }

        void Update() {
            if (dataSource != null) {
                // We recalculate some stuff here to see if stuff needs to be
                // changed.

                // Most of the time, we expect constant column and cell size.
                // This is true as long as there is no resizing on the view.
                // We expose a resizing with animation method. So we check
                // displaying indices each frame to enable/disable views. These
                // are done without animation.
                int oldFirstVisibleIndex = firstVisibleIndex;
                int oldVisibleRows = visibleRows;
                CalculateVisibleIndices();
                int itemCount = dataSource.NumberOfItems(this);

                for (int i = oldFirstVisibleIndex; i < oldFirstVisibleIndex + 
                    oldVisibleRows * columns && i < itemCount; i++) {
                    if (i < firstVisibleIndex || 
                        firstVisibleIndex + visibleRows * columns <= i) {
                        // If not in new range, dequeue.
                        DequeueItem(i);
                    }
                }

                for (int i = firstVisibleIndex; i < firstVisibleIndex + 
                    visibleRows * columns && i < itemCount; i++) {
                    if (i < oldFirstVisibleIndex ||
                        oldFirstVisibleIndex + oldVisibleRows * columns <= i) {
                        // If not in old range, enqueue.
                        EnqueueItem(i);
                    }
                }
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