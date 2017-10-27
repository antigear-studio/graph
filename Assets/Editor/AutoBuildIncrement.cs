// C# example:
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

namespace Antigear.Util {
    public static class AutoBuildIncrement {
        [PostProcessBuildAttribute(0)]
        public static void OnPostprocessBuild(BuildTarget target, 
            string pathToBuiltProject) {
            switch (target) {
                case BuildTarget.iOS:
                    int n = int.Parse(PlayerSettings.iOS.buildNumber);
                    PlayerSettings.iOS.buildNumber = (n + 1).ToString();
                    break;
                case BuildTarget.Android:
                    PlayerSettings.Android.bundleVersionCode += 1;
                    break;
                case BuildTarget.WebGL:
                    n = int.Parse(PlayerSettings.macOS.buildNumber);
                    PlayerSettings.macOS.buildNumber = (n + 1).ToString();
                    break;
            }
        }
    }
}