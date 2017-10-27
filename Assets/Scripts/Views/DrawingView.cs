using MaterialUI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Antigear.Graph {
    /// <summary>
    /// The main view handling drawing - aka putting stuff in the graph onto the
    /// screen. This view also handles its own entry/exit animations.
    /// </summary>
    [ExecuteInEditMode]
    public class DrawingView : MonoBehaviour {
        // Public fields.
        public float animationDuration = 0.5f;
        public bool isExpanded;
        public List<GameObject> toolPrefabs = new List<GameObject>();

        // Private fields.
        readonly List<int> expansionAnimationTweenIds = new List<int>();
        bool wasExpanded;

        // Outlets.
        public Paper paper;
        public ToolbarView toolbarView;
        public HistoryBarView historyBarView;
        public DrawingBottomSheet drawingBottomSheet;
        public MaterialShadow drawingViewMaterialShadow;

        void Update() {
            if (wasExpanded != isExpanded) {
                SetExpansion(isExpanded, false);
            }
        }

        /// <summary>
        /// Enlarges or shrinks the drawing view. If this is animated, you must 
        /// also provide a tile to expand from or shrink to.
        /// </summary>
        /// <param name="shouldExpand">If set to <c>true</c> expands drawing
        /// view. </param>
        /// <param name="animated">If set to <c>true</c> animates this
        /// transition.</param>
        /// <param name="tile">Tile to begin the animatino on. If none, animate
        /// by expanding from bottom of the screen.</param>
        /// <param name="handler">The completion handler.</param>
        public void SetExpansion(bool shouldExpand, bool animated, 
            RectTransform tile = null, Action handler = null) {
            // This animation consists an elevation from the grid view and a
            // new size and pos change.
            if (expansionAnimationTweenIds.Count > 0) {
                // Cancel existing animations.
                foreach (int id in expansionAnimationTweenIds) {
                    TweenManager.EndTween(id);
                }

                expansionAnimationTweenIds.Clear();
            }

            wasExpanded = shouldExpand;
            isExpanded = shouldExpand;

            int shadowId = shouldExpand ? 
                drawingViewMaterialShadow.shadowActiveSize : 
                drawingViewMaterialShadow.shadowNormalSize;

            Vector2 expandedOffsetMin = Vector2.zero;
            Vector2 expandedOffsetMax = Vector2.zero;
            RectTransform rectTransform = paper.transform as RectTransform;
            RectTransform parentRectTransform = transform as RectTransform;

            if (animated) {
                if (tile != null) {
                    // Expanding from tile, so we need to calculate tile's coord
                    // on the fly.
                    int t1, t2;
                    Func<Vector2> offsetMinToTile = () => {
                        Vector3[] parentCorners = new Vector3[4];
                        Vector3[] tileCorners = new Vector3[4];
                        parentRectTransform.GetWorldCorners(parentCorners);
                        tile.GetWorldCorners(tileCorners);
                        Vector3 offset = tileCorners[0] - parentCorners[0];

                        return rectTransform.InverseTransformVector(offset);
                    };

                    Func<Vector2> offsetMaxToTile = () => {
                        Vector3[] parentCorners = new Vector3[4];
                        Vector3[] tileCorners = new Vector3[4];
                        parentRectTransform.GetWorldCorners(parentCorners);
                        tile.GetWorldCorners(tileCorners);
                        Vector3 offset = tileCorners[2] - parentCorners[2];

                        return rectTransform.InverseTransformVector(offset);
                    };

                    if (shouldExpand) {
                        t1 = TweenManager.TweenVector2(
                            v => rectTransform.offsetMin = v, offsetMinToTile, 
                            expandedOffsetMin, animationDuration, 0, handler);
                        t2 = TweenManager.TweenVector2(
                            v => rectTransform.offsetMax = v, offsetMaxToTile, 
                            expandedOffsetMax, animationDuration);
                    } else {
                        t1 = TweenManager.TweenVector2(
                            v => rectTransform.offsetMin = v, 
                            () => rectTransform.offsetMin, offsetMinToTile, 
                            animationDuration, 0, handler);
                        t2 = TweenManager.TweenVector2(
                            v => rectTransform.offsetMax = v, 
                            () => rectTransform.offsetMax, offsetMaxToTile, 
                            animationDuration);
                    }

                    expansionAnimationTweenIds.Add(t1);
                    expansionAnimationTweenIds.Add(t2);
                } else {
                    rectTransform.offsetMin = expandedOffsetMin;

                    // Height might change, so we use this function.
                    Func<Vector2> drawingViewHeight = () => 
                        new Vector2(0, -parentRectTransform.rect.height);
                    int t;

                    if (shouldExpand) {
                        t = TweenManager.TweenVector2(
                            v => rectTransform.offsetMax = v, drawingViewHeight, 
                            expandedOffsetMax, animationDuration, 0, handler);
                    } else {
                        t = TweenManager.TweenVector2(
                            v => rectTransform.offsetMax = v,
                            () => expandedOffsetMax, drawingViewHeight, 
                            animationDuration, 0, handler);
                    }

                    expansionAnimationTweenIds.Add(t);
                }

                drawingViewMaterialShadow.SetShadows(shadowId);
            } else {
                Vector2 shrunkOffset = parentRectTransform.rect.size / 2.0f;

                if (shouldExpand) {
                    rectTransform.offsetMin = expandedOffsetMin;
                    rectTransform.offsetMax = expandedOffsetMax;
                } else {
                    rectTransform.offsetMin = shrunkOffset;
                    rectTransform.offsetMax = -shrunkOffset;
                }

                if (handler != null)
                    handler();

                drawingViewMaterialShadow.SetShadowsInstant(shadowId);
            }
        }

        /// <summary>
        /// Loads the content of the graph onto the canvas.
        /// </summary>
        /// <param name="content">Content.</param>
        public void LoadContent(List<Layer> content) {
            // Clear off the current content.
            Transform canvas = paper.transform.GetChild(0);

            for (int i = 0; i < canvas.childCount; i++) {
                Destroy(canvas.GetChild(i).gameObject);
            }

            foreach (Layer layer in content) {
                LoadLayer(layer);
            }

            // Create a special layer for preview object creation.
            GameObject previewLayer = new GameObject("Preview Layer", 
                typeof(RectTransform));
            previewLayer.transform.SetParent(canvas);
            RectTransform r = previewLayer.transform as RectTransform;
            r.sizeDelta = Vector2.zero;
            r.anchoredPosition3D = Vector3.zero;
        }

        void LoadLayer(Layer layer) {
            Transform canvas = paper.transform.GetChild(0);

            GameObject layerViewObject = new GameObject(layer.name, 
                typeof(RectTransform), typeof(LayerView), typeof(CanvasGroup));
            layerViewObject.transform.SetParent(canvas, true);
            RectTransform r = layerViewObject.transform as RectTransform;
            r.sizeDelta = Vector2.zero;
            r.anchoredPosition3D = Vector3.zero;
            LayerView layerView = layerViewObject.GetComponent<LayerView>();
            CanvasGroup group = layerViewObject.GetComponent<CanvasGroup>();
            layerView.layerGroup = group;
            layerView.UpdateLayer(layer);

            // TODO: actually load layer content here.
        }

        public Transform GetGraphLayerParentTransform(int layerIndex) {
            return paper.transform.GetChild(0).GetChild(layerIndex);
        }

        public GameObject InstantiateToolPrefab(Tool tool, int layer) {
            GameObject prefab = Instantiate(toolPrefabs[(int)tool]);

            // Add the prefab to the scene.
            prefab.transform.SetParent(GetGraphLayerParentTransform(layer));
            prefab.transform.localPosition = Vector3.zero;
            prefab.transform.localScale = Vector3.one;
            prefab.transform.localEulerAngles = Vector3.zero;

            return prefab;
        }

        public int GetPreviewLayerIndex() {
            return paper.transform.GetChild(0).childCount - 1;
        }
    }
}
