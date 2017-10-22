using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Antigear.Graph {
    public static class UIUtil {

        /// <summary>
        /// Determines if the current device is a tablet.
        /// </summary>
        /// <returns><c>true</c> if on tablet; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsTablet() {
            float w = Screen.width;
            float h = Screen.height;
            float inches = Mathf.Sqrt(w * w + h * h) / Screen.dpi;
            return inches >= 8.0f;
        }
    }
}