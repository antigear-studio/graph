using UnityEngine;
using System.Collections;

namespace Devdayo
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Canvas))]
	public class DeviceLayout : MonoBehaviour 
	{
		public static Device currentDevice { get; private set; }
	
		public Canvas canvas { get; private set; }
		public RectTransform rectTransform { get; private set; }
		
		private void Awake()
		{
			canvas = GetComponent<Canvas>();
			rectTransform = GetComponent<RectTransform>();
		}
		
		// See result when builded to device.
		private void OnEnable()
		{
			Device device = Device.Get(0, 1);
			canvas.scaleFactor = device.scaleFactor;
			SetDeviceChanged(device);
		}
		
		#if UNITY_EDITOR
		private Vector2 currentSizeDelta;
		private Vector3 currentScale;
		        
		// Force Update Layout instead Update by System every frame in runtime.
		private void FixedUpdate()
		{
			if(false == Application.isPlaying)
				return;
			
			UpdateRect();
		}
		
		// Update Layout for interact response in runtime.
		private void OnGUI()
		{
			if(false == Application.isPlaying)
				return;
			
			UpdateRect();
		}
		
		// Update Layout in editor & runtime
		internal void Update()
		{
			DeviceSimulator ds = GameObject.FindObjectOfType<DeviceSimulator>();
			if(null == ds)
			{
				// PX Unit
				Device device = Device.Get(0, 1);
				UpdateDevice(device, false, false, Vector3.one);
			}
			
			UpdateRect();
		}
        
		internal void UpdateDevice (Device device, bool dpUnit, bool physicalSize, Vector3 baseScale)
		{
			SetDeviceChanged(device);
			
			Vector2 screenSize = (dpUnit) ? device.dp : device.pixel;
			Vector2 actualSize = device.actual;
			
			float screenScale = actualSize.magnitude / screenSize.magnitude;
			
			float scaleWidth = 1f;
			float scaleHeight = 1f;
			float scale = 1f;
			
			float maxWidth = Screen.width;
			float maxHeight = Screen.height;
			
			canvas.scaleFactor = 1f;
			
			if(canvas.renderMode == RenderMode.ScreenSpaceCamera)
			{
				Camera camera = canvas.worldCamera;
				float cameraHeight = 0f;
				
				if(true == camera.orthographic)
				{
					// Ortho Camera
					cameraHeight = camera.orthographicSize * 2f;
				}
				else
				{
					// Perspective Camera
					float fov = camera.fieldOfView;
					float focalLength = 2f * Mathf.Tan( fov / 2f * Mathf.Deg2Rad );
					float perspectiveHeight = canvas.planeDistance * focalLength;
					
					cameraHeight = perspectiveHeight;
				}
				
				float cameraWidth = (maxWidth / maxHeight) * cameraHeight;
				
				float normalWidth = actualSize.x / maxWidth;
				float normalHeight = actualSize.y / maxHeight;
				
				float realWidth = normalWidth * cameraWidth;
				float realHeight = normalHeight * cameraHeight;
				
				if(realWidth > cameraWidth || false == physicalSize)
				{	
					realWidth = (cameraWidth / realWidth) * realWidth;
				}
				
				if(realHeight > cameraHeight || false == physicalSize)
				{
					realHeight = (cameraHeight / realHeight) * realHeight;
				}
				
				scaleWidth = realWidth / actualSize.x;
				scaleHeight = realHeight / actualSize.y;
			}
			else
			{
				if(actualSize.x > maxWidth || false == physicalSize)
				{	
					scaleWidth = maxWidth / actualSize.x;
				}
				
				if(actualSize.y > maxHeight || false == physicalSize)
				{
					scaleHeight = maxHeight / actualSize.y;
				}
			}
			
			// Uniform Scale
			scale = Mathf.Min(scaleWidth, scaleHeight) * screenScale;
			
			// Update Data
			currentSizeDelta = screenSize;
            currentScale = baseScale * scale;//Vector3.one * scale;
		}
		
		private void UpdateRect()
		{
			rectTransform.sizeDelta = currentSizeDelta;
			rectTransform.localScale = currentScale;
		}
		#endif
				
		public void SetDeviceChanged(Device device)
		{
			DeviceLayout.currentDevice = device;
		}
		
	}
	
}