using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
namespace Devdayo
{
	[ExecuteInEditMode]
	public class DeviceSimulator : MonoBehaviour 
	{
		[SerializeField]
		private List<DeviceLayout> targets;
		
		[SerializeField, HideInInspector]
		private bool physicalSize = true, dpUnit = true;
		
		[SerializeField, HideInInspector]
		private int deviceIndex, orientation;

        [SerializeField, HideInInspector]
        private int currentResolutionIndex = 0, monitorResolutionIndex = 0;

        [SerializeField, HideInInspector]
        private float monitorDiagonal = 13.3f;

		private void Reset ()
		{
			// Your Desktop
			deviceIndex = 0;
			orientation = 1;
			
			//targets = new List<DeviceLayout>();
			//targets.AddRange(GameObject.FindObjectsOfType<DeviceLayout>());
		}
		
        // Update once per frame
        private void Update()
		{
            Vector3 monitorScale = Vector3.one;

            if(physicalSize)
            {
                Vector2 cr = Resolution.Get(currentResolutionIndex).size;
                Vector2 mr = Resolution.Get(monitorResolutionIndex).size;

                Vector3 resolutionScale = new Vector3(cr.x / mr.x, cr.y / mr.y, 1);
                
                if (monitorDiagonal < Mathf.Epsilon)
                    monitorDiagonal = 1f;

                /*
                float diagonalScale = (monitorResolution.magnitude / monitorDPI) / (monitorDiagonal + Mathf.Epsilon);

                Vector3 resolutionScale = new Vector3(currentResolution.x / monitorResolution.x, currentResolution.y / monitorResolution.y, 1);
                monitorScale = resolutionScale * diagonalScale;
                */

                float monitorDPI = mr.magnitude / monitorDiagonal;
                float dpiScale = monitorDPI / Screen.dpi;

                monitorScale = resolutionScale * dpiScale;
            }

			Device device = Device.Get(deviceIndex, orientation);
			foreach (DeviceLayout target in targets) 
			{
                if (target == null)
                    return;
                target.UpdateDevice(device, dpUnit, physicalSize, monitorScale);
				target.Update(); // Notify Changed
			}
        }
    }
    
}
#endif