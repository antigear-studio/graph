using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Devdayo
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Image))]
	[RequireComponent(typeof(LayoutElement))] 
	[RequireComponent(typeof(ContentSizeFitter))]
	[RequireComponent(typeof(HorizontalLayoutGroup))]
	[RequireComponent(typeof(Mask))]
	public class ActionBar : MonoBehaviour 
	{
		public float portrait = 56, landscape = 48, tabletOrDesktop = 64;
		
		//private Image image;
		private LayoutElement layoutElement;
		private ContentSizeFitter contentSizeFitter;
		private HorizontalLayoutGroup horizontalLayoutGroup;
		private Mask mask;
		
		private RectTransform rectTransform;
		
		private void Awake ()
		{
			//image = GetComponent<Image>();
			rectTransform = GetComponent<RectTransform>();
			
			layoutElement = GetComponent<LayoutElement>();
			layoutElement.preferredHeight = 48;
			layoutElement.flexibleWidth = 1;
			layoutElement.flexibleHeight = 0;
			
			contentSizeFitter = GetComponent<ContentSizeFitter>();
			contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
			
			horizontalLayoutGroup = GetComponent<HorizontalLayoutGroup>();
			horizontalLayoutGroup.padding = new RectOffset(16, 16, 0, 0);
			horizontalLayoutGroup.spacing = 16;
			horizontalLayoutGroup.childAlignment = TextAnchor.UpperLeft;
			horizontalLayoutGroup.childForceExpandWidth = false;
			horizontalLayoutGroup.childForceExpandHeight = true;
			
			mask = GetComponent<Mask>();
			mask.showMaskGraphic = true;
		}
		
		private void Start ()
		{
			rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
			rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, 0);
			
			rectTransform.anchoredPosition = Vector2.zero;
			rectTransform.anchorMin = new Vector2(0, 1);
			rectTransform.anchorMax = new Vector2(1, 1);
			rectTransform.pivot = new Vector2(0.5f, 1f);
		}
		
		protected virtual void Update ()
		{
			Device device = DeviceLayout.currentDevice;
			
			if(device.type < 2) 
			{
				// Desktop or Tablet
				layoutElement.preferredHeight = tabletOrDesktop;
			}
			else if(device.orientation == 0)
			{
				// Portrait
				layoutElement.preferredHeight = portrait;
			}
			else
			{
				// Landscape
				layoutElement.preferredHeight = landscape;
			}
		}
	}
	
}