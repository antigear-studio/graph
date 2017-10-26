using I2;
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
        public float shortAnimationDuration = 0.2f;
        public Color buttonColor = Color.black;
        public CanvasGroup toolGroup;
        public IToolbarViewDelegate viewDelegate;


        // Outlets.
        public MaterialDropdown lineDropdown;
        public MaterialDropdown brushDropdown;
        public MaterialDropdown mediaDropdown;
        public MaterialDropdown selectionDropdown;
        public MaterialDropdown canvasControlDropdown;
        public MaterialButton shapesButton;
        public MaterialButton layersButton;
        public MaterialButton propertiesButton;
        public VectorImage indicatorImage;
        public UnityEngine.UI.Text toolText;
        public UnityEngine.UI.Text coordinateText;

        MaterialDropdown activeDropdown;

        Tool currentTool = Tool.Unknown;

        readonly List<int> toolbarAnimationTweenIds = new List<int>();
        int buttonColorAnimationTweenId = -1;
        int indicatorAnimationTweenId = -1;
        bool didShowToolbar;
        Color lastButtonColor;

        // Use this for initialization
        void Start() {
            didShowToolbar = showToolbar;
            activeDropdown = lineDropdown;
            lastButtonColor = buttonColor;
        }
	
        // Update is called once per frame
        void Update() {
            if (didShowToolbar != showToolbar) {
                SetToolbarVisibility(showToolbar, false);
            }

            if (lastButtonColor != buttonColor) {
                SetButtonColor(buttonColor, false);
            }
        }

        public void SetButtonColor(Color color, bool animated) {
            if (buttonColorAnimationTweenId >= 0)
                TweenManager.EndTween(buttonColorAnimationTweenId);
            
            buttonColor = color;
            lastButtonColor = color;

            if (animated) {
                buttonColorAnimationTweenId = TweenManager.TweenColor(c => {
                    shapesButton.iconColor = c;
                    layersButton.iconColor = c;
                    propertiesButton.iconColor = c;
                    lineDropdown.buttonImageContent.color = c;
                    brushDropdown.buttonImageContent.color = c;
                    mediaDropdown.buttonImageContent.color = c;
                    selectionDropdown.buttonImageContent.color = c;
                    canvasControlDropdown.buttonImageContent.color = c;
                    toolText.color = c;
                    coordinateText.color = c;
                    indicatorImage.color = c;
                }, propertiesButton.iconColor, color, shortAnimationDuration);
            } else {
                shapesButton.iconColor = color;
                layersButton.iconColor = color;
                propertiesButton.iconColor = color;
                lineDropdown.buttonImageContent.color = color;
                brushDropdown.buttonImageContent.color = color;
                mediaDropdown.buttonImageContent.color = color;
                selectionDropdown.buttonImageContent.color = color;
                canvasControlDropdown.buttonImageContent.color = color;
                toolText.color = color;
                coordinateText.color = color;
                indicatorImage.color = color;
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
                toolGroup.alpha = targetAlpha;
            }
        }

        public void OnToolButtonPress(MaterialDropdown dropdown) {
            if (dropdown != activeDropdown) {
                activeDropdown = dropdown;
                Tool t = LookupTool(dropdown, dropdown.currentlySelected);
                ChangeTool(t, true);
            } else {
                activeDropdown.Show();
            }
        }

        public void OnDropdownValueChange(MaterialDropdown dropdown) {
            Tool t = LookupTool(dropdown, dropdown.currentlySelected);
            ChangeTool(t, true);
        }

        Tool LookupTool(MaterialDropdown dropdown, int index) {
            if (dropdown == lineDropdown) {
                return index + Tool.StraightLine;
            } else if (dropdown == brushDropdown) {
                return index + Tool.Pencil;
            }  else if (dropdown == mediaDropdown) {
                return index + Tool.Text;
            }  else if (dropdown == selectionDropdown) {
                return index + Tool.RectangleSelection;
            }  else if (dropdown == canvasControlDropdown) {
                return index + Tool.Zoom;
            }

            return Tool.Unknown;
        }

        public void ChangeTool(Tool t, bool animated) {
            if (t != currentTool) {
                currentTool = t;
                toolText.text = t.LocalizedName().ToUpper();

                // Update UI.
                if (t >= Tool.StraightLine && t <= Tool.FreeformLine) {
                    activeDropdown = lineDropdown;
                    lineDropdown.currentlySelected = t - Tool.StraightLine;
                    MoveIndicator(lineDropdown, animated);
                } else if (t >= Tool.Pencil && t <= Tool.Eraser) {
                    activeDropdown = brushDropdown;
                    brushDropdown.currentlySelected = t - Tool.Pencil;
                    MoveIndicator(brushDropdown, animated);
                } else if (t >= Tool.Text && t <= Tool.Image) {
                    activeDropdown = mediaDropdown;
                    mediaDropdown.currentlySelected = t - Tool.Text;
                    MoveIndicator(mediaDropdown, animated);
                } else if (t >= Tool.RectangleSelection && 
                    t <= Tool.LassoSelection) {
                    activeDropdown = selectionDropdown;
                    selectionDropdown.currentlySelected = 
                        t - Tool.RectangleSelection;
                    MoveIndicator(selectionDropdown, animated);
                } else if (t >= Tool.Zoom && t <= Tool.Pan) {
                    activeDropdown = canvasControlDropdown;
                    canvasControlDropdown.currentlySelected = t - Tool.Zoom;
                    MoveIndicator(canvasControlDropdown, animated);
                }

                if (viewDelegate != null)
                    viewDelegate.OnToolChanged(t);
            }
        }

        void MoveIndicator(MaterialDropdown target, bool animated) {
            if (indicatorAnimationTweenId >= 0) {
                TweenManager.EndTween(indicatorAnimationTweenId);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(
                transform as RectTransform);
            float targetValue = target.transform.position.x;

            if (animated) {
                indicatorAnimationTweenId = TweenManager.TweenFloat(v => {
                    Vector3 pos = indicatorImage.transform.position;
                    pos.x = v;
                    indicatorImage.transform.position = pos;
                }, indicatorImage.transform.position.x, targetValue, 
                    shortAnimationDuration);
            } else {
                Vector3 pos = indicatorImage.transform.position;
                pos.x = targetValue;
                indicatorImage.transform.position = pos;
            }
        }
    }

    public interface IToolbarViewDelegate {
        void OnToolChanged(Tool newTool);
    }
}