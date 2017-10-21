/**
 Readability

 Created: Oct-20-2017
 Author: Flying_Banana
 */

using UnityEngine;

namespace Readability {
    /// <summary>
    /// Readability script for colors. Coded by referring to the paper below:
    /// http://citeseerx.ist.psu.edu/viewdoc/download?doi=10.1.1.640.9220&rep=rep1&type=pdf
    /// </summary>
    [ExecuteInEditMode]
    public class Readability : MonoBehaviour {
        public Color foreground;
        public Color background;

        Color f;
        Color g;

        // Update is called once per frame
        void Update () {
            if (f != foreground || g != background) {
                f = foreground;
                g = background;

                Debug.Log("Readability: " + Evaluate(f, g) + "%");
            }
        }

        /// <summary>
        /// Returns a score in the range 0-100 of how readable the two colors
        /// are, with 0 being unreadable and 100 being most readable. Assumes
        /// that background has an alpha of 1, and any foreground transparency
        /// is eliminated by calculating the final color on background using
        /// default alpha blending.
        /// </summary>
        public static float Evaluate(Color foreground, Color background) {
            background.a = 1;
            foreground.r = 
                foreground.r * foreground.a + background.r * (1 - foreground.a);
            foreground.g = 
                foreground.g * foreground.a + background.g * (1 - foreground.a);
            foreground.b = 
                foreground.b * foreground.a + background.b * (1 - foreground.a);
            foreground.a = 1;

            float dr = (foreground.r - background.r) * 255;
            float dg = (foreground.g - background.g) * 255;
            float db = (foreground.b - background.b) * 255;

            return Mathf.Min(100, Mathf.Max(0, 
                1e-1f * Mathf.Abs(dr) + 5.7e-1f * Mathf.Abs(dg) + 
                8.6e-2f * Mathf.Abs(db) - 1.1e-2f * db - 1.1e-3f * dg * dg - 
                1.5e-4f * db * db - 3.6f));
        }
    }
}
