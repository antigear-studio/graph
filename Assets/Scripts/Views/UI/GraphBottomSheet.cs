using MaterialUI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Antigear.Graph {
    public class GraphBottomSheet : BottomSheet {
        public IGraphBottomSheetDelegate sheetDelegate;

        // Outlets.
        public GameObject clearSelectionButton;
        public GameObject selectionButton;
        public GameObject sortGroup;
        public MaterialButton sortByNameButton;
        public MaterialButton sortByDateCreatedButton;
        public MaterialButton sortByDateEditedButton;
        public RectTransform checkMarkTransform;

        bool sortVisible;
        bool sortDescending;
        MaterialButton activeSortButton;

        protected readonly List<int> sortGroupTweenAnimationIds = 
            new List<int>();

        public void OnSortPress() {
            SetSortingOptionsVisibility(!sortVisible, true);
        }

        void SetSortingOptionsVisibility(bool visible, bool animated) {
            if (sortVisible == visible) {
                return;
            }

            if (sortGroupTweenAnimationIds.Count >= 0) {
                foreach (int id in sortGroupTweenAnimationIds) {
                    TweenManager.EndTween(id);
                }

                sortGroupTweenAnimationIds.Clear();
            }

            if (bottomSheetAnimationIds.Count >= 0) {
                foreach (int id in bottomSheetAnimationIds) {
                    TweenManager.EndTween(id);
                }

                bottomSheetAnimationIds.Clear();
            }

            CanvasGroup g = sortGroup.GetComponent<CanvasGroup>();
            LayoutElement e = sortGroup.GetComponent<LayoutElement>();

            sortVisible = visible;
            g.blocksRaycasts = visible;
            g.interactable = visible;

            float targetHeight = sortVisible ? 144 : 0;
            float targetAlpha = sortVisible ? 1 : 0;
            float targetSheetHeight = panelTransform.rect.height - MAX_OVERDRAG;

            if (sortVisible)
                targetSheetHeight += 144;
            else
                targetSheetHeight -= 144;

            if (animated) {
                int t1 = TweenManager.TweenFloat(v => g.alpha = v, g.alpha, 
                             targetAlpha, animationDuration);
                int t2 = TweenManager.TweenFloat(v => {
                    e.preferredHeight = v;
                    Vector2 pos = panelTransform.anchoredPosition;
                    pos.y = panelTransform.rect.height - MAX_OVERDRAG;
                    panelTransform.anchoredPosition = pos;
                }, e.preferredHeight, targetHeight, animationDuration);
                int t3 = TweenManager.TweenFloat(v => {
                    Vector2 pos = panelTransform.anchoredPosition;
                    pos.y = v;
                    panelTransform.anchoredPosition = pos;
                }, panelTransform.anchoredPosition.y, targetSheetHeight, 
                             animationDuration, 0);
                sortGroupTweenAnimationIds.Add(t1);
                sortGroupTweenAnimationIds.Add(t2);
                bottomSheetAnimationIds.Add(t3);
            } else {
                g.alpha = targetAlpha;
                e.preferredHeight = targetHeight;
                Vector2 pos = panelTransform.anchoredPosition;
                pos.y = targetSheetHeight;
                panelTransform.anchoredPosition = pos;
            }
        }

        public void SetGraphSortOrder(GraphSortOrder order) {
            switch (order) {
                case GraphSortOrder.NaturalAscending:
                    sortDescending = false;
                    activeSortButton = sortByNameButton;
                    break;
                case GraphSortOrder.NaturalDescending:
                    sortDescending = true;
                    activeSortButton = sortByNameButton;
                    break;
                case GraphSortOrder.CreationDateAscending:
                    sortDescending = false;
                    activeSortButton = sortByDateCreatedButton;
                    break;
                case GraphSortOrder.CreationDateDescending:
                    sortDescending = true;
                    activeSortButton = sortByDateCreatedButton;
                    break;
                case GraphSortOrder.ModificationDateAscending:
                    sortDescending = false;
                    activeSortButton = sortByDateEditedButton;
                    break;
                case GraphSortOrder.ModificationDateDescending:
                    sortDescending = true;
                    activeSortButton = sortByDateEditedButton;
                    break;
            }

            MaterialButton[] buttons = 
                sortGroup.GetComponentsInChildren<MaterialButton>(true);

            foreach (MaterialButton button in buttons) {
                button.icon.enabled = button == activeSortButton;
                Vector3 angle = button.icon.rectTransform.localEulerAngles;
                angle.z = sortDescending ? 0 : 180;
                button.icon.rectTransform.localEulerAngles = angle;

                if (button == activeSortButton) {
                    checkMarkTransform.SetParent(button.contentRectTransform);
                }
            }
        }

        public void SetSelectingMode(bool isSelecting) {
            clearSelectionButton.SetActive(isSelecting);
            selectionButton.SetActive(!isSelecting);
        }

        public GraphSortOrder GetGraphSortOrder() {
            if (activeSortButton == sortByNameButton) {
                if (sortDescending)
                    return GraphSortOrder.NaturalDescending;
                else
                    return GraphSortOrder.NaturalAscending;
            } else if (activeSortButton == sortByDateCreatedButton) {
                if (sortDescending)
                    return GraphSortOrder.CreationDateDescending;
                else
                    return GraphSortOrder.CreationDateAscending;
            } else if (activeSortButton == sortByDateEditedButton) {
                if (sortDescending)
                    return GraphSortOrder.ModificationDateDescending;
                else
                    return GraphSortOrder.ModificationDateAscending;
            }

            return GraphSortOrder.Unknown;
        }

        public void OnSortingOptionButtonPress(MaterialButton pressed) {
            if (pressed == activeSortButton) {
                // Change arrow orientation.
                sortDescending = !sortDescending;
            } else {
                // Change active button.
                activeSortButton.icon.enabled = false;
                activeSortButton = pressed;
                sortDescending = true;
                activeSortButton.icon.enabled = true;
                checkMarkTransform.SetParent(pressed.contentRectTransform);
            }

            Vector3 angle = pressed.icon.rectTransform.localEulerAngles;
            angle.z = sortDescending ? 0 : 180;
            pressed.icon.rectTransform.localEulerAngles = angle;
            Dismiss(true, null, animationDuration);

            if (sheetDelegate != null)
                sheetDelegate.OnSortingOrderChange(this, GetGraphSortOrder());
        }

        public void OnSelectButtonPress() {
            Dismiss(true);

            if (sheetDelegate != null)
                sheetDelegate.OnSelect(this);
        }

        public void OnSelectAllButtonPress() {
            Dismiss(true);

            if (sheetDelegate != null)
                sheetDelegate.OnSelectAll(this);
        }

        public void OnSelectCancelButtonPress() {
            Dismiss(true);

            if (sheetDelegate != null)
                sheetDelegate.OnSelectCancel(this);
        }

        public override void Dismiss(bool animated, Action onCompletion = null, 
            float delay = 0) {

            TweenManager.TweenInt(v => {
                if (sortGroupTweenAnimationIds.Count >= 0) {
                    foreach (int id in sortGroupTweenAnimationIds) {
                        TweenManager.EndTween(id);
                    }

                    sortGroupTweenAnimationIds.Clear();
                }

                base.Dismiss(animated, () => {
                    if (onCompletion != null)
                        onCompletion();

                    SetSortingOptionsVisibility(false, false);
                }, 0);
            }, 0, 0, delay);
        }

    }

    public interface IGraphBottomSheetDelegate {
        void OnSortingOrderChange(GraphBottomSheet sheet, GraphSortOrder order);

        void OnSelectAll(GraphBottomSheet sheet);

        void OnSelect(GraphBottomSheet sheet);

        void OnSelectCancel(GraphBottomSheet sheet);
    }
}