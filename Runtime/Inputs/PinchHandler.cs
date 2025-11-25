// using Ouroboros.Common.Consoles;
using Ouroboros.Common.Utils;
using System;
using UnityEngine;

namespace Ouroboros.Common.Inputs
{
    public class PinchHandler : MonoBehaviour
    {
        public Action<float, float> OnPinch { get; set; }

        private float initialDistance;
        private float lastPinchDelta;
        
        // private void Start()
        // {
        //     DebugConsole.AddWatch("touch count");
        //     DebugConsole.AddWatch("touch 1 phase");
        //     DebugConsole.AddWatch("touch 1 pos");
        //     DebugConsole.AddWatch("touch 2 phase");
        //     DebugConsole.AddWatch("touch 2 pos");
        // }

        private void Update()
        {
            // DebugConsole.UpdateVariable("touch count", InputWrapper.TouchCount);
            // DebugConsole.UpdateVariable("touch 1 phase", InputWrapper.TouchCount > 0 ? InputWrapper.GetTouch(0).phase.ToString() : "None");
            // DebugConsole.UpdateVariable("touch 1 pos", InputWrapper.TouchCount > 0 ? InputWrapper.GetTouch(0).position : Vector2.zero);
            // DebugConsole.UpdateVariable("touch 2 phase", InputWrapper.TouchCount > 0 ? InputWrapper.GetTouch(0).phase.ToString() : "None");
            // DebugConsole.UpdateVariable("touch 2 pos", InputWrapper.TouchCount > 0 ? InputWrapper.GetTouch(0).position : Vector2.zero);

            if (InputWrapper.TouchCount != 2) return;

            Touch touch1 = InputWrapper.GetTouch(0);
            Touch touch2 = InputWrapper.GetTouch(1);

            if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
            {
                initialDistance = Vector2.Distance(touch1.position, touch2.position);
                lastPinchDelta = 0f;
            }
            else if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
            {
                var currentDistance = Vector2.Distance(touch1.position, touch2.position);
                var initialDistanceDelta = initialDistance - currentDistance;
                var initialPinchDelta = initialDistanceDelta / ScreenUtils.ScreenSizeAvg;

                // Debug.Log($"Pinch delta: {pinchDelta}, dpi={Screen.dpi}, w={Screen.width}");
                OnPinch?.Invoke(initialPinchDelta, initialPinchDelta - lastPinchDelta);
                lastPinchDelta = initialPinchDelta;
            }
        }
    }
}