using MaterialUI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Antigear.Graph {
    [ExecuteInEditMode]
    public class ToolbarView : MonoBehaviour {
        public bool showToolbar;
        public float toolbarHeight = 40;
        public float animationDuration = 0.5f;
        public CanvasGroup toolGroup;
        public IToolbarViewDelegate viewDelegate;


        // Outlets.
        public MaterialDropdown lineDropdown;
        public MaterialDropdown brushDropdown;
        public MaterialDropdown mediaDropdown;
        public MaterialDropdown selectionDropdown;
        public MaterialDropdown canvasControlDropdown;

        MaterialDropdown activeDropdown;

        Tool currentTool = Tool.StraightLine;

        readonly List<int> toolbarAnimationTweenIds = new List<int>();
        bool didShowToolbar;

        // Use this for initialization
        void Start() {
            didShowToolbar = showToolbar;
            activeDropdown = lineDropdown;
        }
	
        // Update is called once per frame
        void Update() {
            if (didShowToolbar != showToolbar) {
                SetToolbarVisibility(showToolbar, false);
            }
        }

        public void SetToolbarVisibility(bool shouldShow, bool animated) {
            if (toolbarAnimationTweenIds.Count > 0) {
                foreach (int id in toolbarAnimationTweenIds) {
                    TweenManager.EndTween(id);
                }

                toolbarAnimationTweenIds.Clear();
            }

            didShowToolbar = shouldShow;
            showToolbar = shouldShow;
            RectTransform r = transform as RectTransform;

            float targetValue = shouldShow ? 0 : -toolbarHeight;
            float targetAlpha = shouldShow ? 1 : 0;

            if (animated) {
                int t1 = TweenManager.TweenFloat(h => {
                    Vector2 pos = r.anchoredPosition;
                    pos.y = h;
                    r.anchoredPosition = pos;
                }, r.anchoredPosition.y, targetValue, animationDuration);
                int t2 = TweenManager.TweenFloat(a => toolGroup.alpha = a,
                    toolGroup.alpha, targetAlpha, animationDuration);
                toolbarAnimationTweenIds.Add(t1);
                toolbarAnimationTweenIds.Add(t2);
            } else {
                Vector2 pos = r.anchoredPosition;
                pos.y = targetValue;
                r.anchoredPosition = pos;
            }
        }

        public void OnToolButtonPress(MaterialDropdown dropdown) {
            if (dropdown != activeDropdown) {
                activeDropdown = dropdown;
                Tool t = LookupTool(dropdown, dropdown.currentlySelected);
                ChangeTool(t);
            } else {
                activeDropdown.Show();
            }
        }

        public void OnDropdownValueChange(MaterialDropdown dropdown) {
            Tool t = LookupTool(dropdown, dropdown.currentlySelected);
            ChangeTool(t);
        }

        Tool LookupTool(MaterialDropdown dropdown, int index) {
            if (dropdown == lineDropdown) {
                if (index == 0)
                    return Tool.StraightLine;
                if (index == 1)
                    return Tool.BezierCurve;
                if (index == 2)
                    return Tool.Arc;
                if (index == 3)
                    return Tool.FreeformLine;
            } else if (dropdown == brushDropdown) {
                if (index == 0)
                    return Tool.Pencil;
                if (index == 1)
                    return Tool.Eraser;
            }  else if (dropdown == mediaDropdown) {
                if (index == 0)
                    return Tool.Text;
                if (index == 1)
                    return Tool.Image;
            }  else if (dropdown == selectionDropdown) {
                if (index == 0)
                    return Tool.RectangleSelection;
                if (index == 1)
                    return Tool.LassoSelection;
            }  else if (dropdown == canvasControlDropdown) {
                if (index == 0)
                    return Tool.Zoom;
                if (index == 1)
                    return Tool.Pan;
            }

            return Tool.Unknown;
        }

        void ChangeTool(Tool t) {
            if (t != currentTool) {
                currentTool = t;

                if (viewDelegate != null)
                    viewDelegate.OnToolChanged(t);
            }
        }
    }

    public interface IToolbarViewDelegate {
        void OnToolChanged(Tool newTool);
    }
}