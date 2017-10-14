using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace Devdayo
{
	[CustomEditor(typeof(DeviceSimulator))]
	[CanEditMultipleObjects]
	public class DeviceSimulatorEditor : Editor 
	{
		private SerializedProperty deviceIndex;
		private SerializedProperty orientation;
		private SerializedProperty physicalSize;
		// private SerializedProperty hasChanged;
		private SerializedProperty dpUnit;

        private SerializedProperty currentResolutionIndex;
        private SerializedProperty monitorResolutionIndex;
        private SerializedProperty monitorDiagonal;
        // private SerializedProperty monitorDPI;

		private void OnEnable ()
		{
			deviceIndex = serializedObject.FindProperty("deviceIndex");
			orientation = serializedObject.FindProperty("orientation");
			physicalSize = serializedObject.FindProperty("physicalSize");
			// hasChanged = serializedObject.FindProperty("hasChanged");
            dpUnit = serializedObject.FindProperty("dpUnit");

            currentResolutionIndex = serializedObject.FindProperty("currentResolutionIndex");
            monitorResolutionIndex = serializedObject.FindProperty("monitorResolutionIndex");
            monitorDiagonal = serializedObject.FindProperty("monitorDiagonal");
            // monitorDPI = serializedObject.FindProperty("monitorDPI");
		}
		
		public override void OnInspectorGUI ()
		{
			base.OnInspectorGUI ();
			serializedObject.Update();
			
			BeginGUI();
			DrawTitleGUI();
			DrawDPUnitGUI();
            DrawPhysicalSizeGUI();
            DrawDeviceAndOrientationGUI();
			EndGUI();
			
			serializedObject.ApplyModifiedProperties();
		}
		
		private void BeginGUI()
		{
			GUIStyle style = new GUIStyle("Box");
			style.margin = new RectOffset(10, 15, 10, 10);
			style.padding = new RectOffset(15, 15, 15, 15);
			
			EditorGUILayout.BeginVertical(style);
		}
		
		private void DrawTitleGUI()
		{
			GUIStyle style = new GUIStyle();
			style.fontSize = 12;
			
			EditorGUILayout.LabelField("Device Simulator (Editor Only)", style);
			EditorGUILayout.Space(); 
		}
		
		private void DrawDPUnitGUI()
		{
			// bool oldDpUnit = dpUnit.boolValue;
			
			if(deviceIndex.intValue > 0)
				dpUnit.boolValue = EditorGUILayout.Toggle("DP Unit", dpUnit.boolValue);
			
			// if(dpUnit.boolValue != oldDpUnit)
			// 	hasChanged.boolValue = true;
		}
		
		private void DrawPhysicalSizeGUI()
		{
			// bool oldPhysicalSize = physicalSize.boolValue;
			
			if(deviceIndex.intValue > 0)
				physicalSize.boolValue = EditorGUILayout.Toggle("Physical Size", physicalSize.boolValue);
				
			// if(physicalSize.boolValue != oldPhysicalSize)
			// 	hasChanged.boolValue = true;
		}
		
		private void DrawDeviceAndOrientationGUI()
		{
			EditorGUILayout.LabelField("Device & Orientation");
			
			int oldDeviceIndex = deviceIndex.intValue;
			deviceIndex.intValue = EditorGUILayout.Popup(deviceIndex.intValue, Device.names);
			
			// int oldOrientation = orientation.intValue;
			// is Desktop ?
			if(deviceIndex.intValue != oldDeviceIndex && oldDeviceIndex == 0)
				orientation.intValue = 0;
			
			// if not desktop
            if (deviceIndex.intValue > 0)
            {
                orientation.intValue = EditorGUILayout.Popup(orientation.intValue, Device.orientations);
                DrawScreenSettingGUI();
            }
            else
                orientation.intValue = 1;
				
			// if(deviceIndex.intValue != oldDeviceIndex || orientation.intValue != oldOrientation)
			// 	hasChanged.boolValue = true;
		}

        private void DrawScreenSettingGUI()
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 11;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("------");
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Monitor Setup", style);

            //currentResolution.vector2Value = EditorGUILayout.Vector2Field("Current Resolution", currentResolution.vector2Value);
            //monitorResolution.vector2Value = EditorGUILayout.Vector2Field("Monitor Resolution", monitorResolution.vector2Value);

            currentResolutionIndex.intValue = EditorGUILayout.Popup("Current Resolution", currentResolutionIndex.intValue, Resolution.names);
            monitorResolutionIndex.intValue = EditorGUILayout.Popup("Monitor Resolution", monitorResolutionIndex.intValue, Resolution.names);
            
            monitorDiagonal.floatValue = EditorGUILayout.FloatField("Monitor Diagonal (Inch)", monitorDiagonal.floatValue);
            //monitorDPI.floatValue = EditorGUILayout.FloatField("Monitor DPI (Default : "+Screen.dpi+")", monitorDPI.floatValue);
        }
		
		private void EndGUI()
		{
			EditorGUILayout.EndVertical();
		}
	}
}
#endif