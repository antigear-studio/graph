using MaterialUI;
using UnityEngine;

namespace Antigear.Graph {
    public static class FloatingActionButtonAnimation {
        public static void Show(this MaterialButton btn, bool animated, 
            float duration = 0.5f, float delay = 0) {
            Vector3 startRotation = new Vector3(0, 0, 45);
            Vector3 startSize = new Vector3(0, 0, 1);
            Vector3 targetRotation = Vector3.zero;
            Vector3 targetSize = Vector3.one;
            btn.GetComponent<CanvasGroup>().blocksRaycasts = true;
            btn.GetComponent<CanvasGroup>().interactable = true;
            btn.transform.localEulerAngles = startRotation;

            if (animated) {
                TweenManager.TweenVector3(
                    v => btn.transform.localEulerAngles = v, startRotation,
                    targetRotation, duration / 2, duration / 2 + delay);
                TweenManager.TweenVector3(
                    v => btn.transform.localScale = v,
                    startSize, targetSize, duration, delay);
            } else {
                btn.transform.localEulerAngles = targetRotation;
                btn.transform.localScale = targetSize;
            }
        }

        public static void Dismiss(this MaterialButton btn, bool animated, 
            float duration = 0.5f, float delay = 0) {
            Vector3 startSize = Vector3.one;
            Vector3 targetSize = new Vector3(0, 0, 1);
            btn.GetComponent<CanvasGroup>().blocksRaycasts = false;
            btn.GetComponent<CanvasGroup>().interactable = false;

            if (animated) {
                TweenManager.TweenVector3(
                    v => btn.transform.localScale = v,
                    startSize, targetSize, duration, delay);
            } else {
                btn.transform.localScale = targetSize;
            }
        }
    }   
}