using UnityEngine;
using TouchPhase = UnityEngine.TouchPhase;
using Touch = UnityEngine.Touch;

#if ENABLE_INPUT_SYSTEM

using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.EnhancedTouch;
using NewTouch = UnityEngine.InputSystem.EnhancedTouch.Touch;

#endif

namespace Ouroboros.Common.Inputs
{
    public static class InputWrapper
    {
#if ENABLE_INPUT_SYSTEM

        static InputWrapper()
        {

            CreateKeyCodes();

        }

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {

            EnhancedTouchSupport.Enable();
            
        }

#endif

        public static int TouchCount 
        { 
            get
            {
#if ENABLE_INPUT_SYSTEM

                return NewTouch.activeTouches.Count;
                // return Touchscreen.current?.touches.Count ?? 0;

#else

                return Input.touchCount;

#endif
            }
        }

        public static bool GetKey(KeyCode key)
        {
#if ENABLE_INPUT_SYSTEM

            if (!TryConvertKeyCode(key, out var newKey)) return false;

            return Keyboard.current != null && Keyboard.current[newKey].isPressed;
#else

            return Input.GetKey(key);

#endif
        }

        public static bool GetKeyDown(KeyCode key)
        {
#if ENABLE_INPUT_SYSTEM

            if (!TryConvertKeyCode(key, out var newKey)) return false;

            return Keyboard.current != null && Keyboard.current[newKey].wasPressedThisFrame;

#else

            return Input.GetKeyDown(key);

#endif
        }

        public static bool GetKeyUp(KeyCode key)
        {
#if ENABLE_INPUT_SYSTEM

            if (!TryConvertKeyCode(key, out var newKey)) return false;

            return Keyboard.current != null && Keyboard.current[newKey].wasReleasedThisFrame;
#else

            return Input.GetKeyUp(key);

#endif
        }

        public static bool GetMouseButtonDown(int button)
        {
#if ENABLE_INPUT_SYSTEM

            if (Mouse.current == null) return false;

            return 
                button == 0 ? Mouse.current.leftButton.wasPressedThisFrame 
                : button == 1 ? Mouse.current.rightButton.wasPressedThisFrame 
                : button == 2 && Mouse.current.middleButton.wasPressedThisFrame;

#else

            return Input.GetMouseButtonDown(button);

#endif
        }

        public static bool GetMouseButton(int button)
        {
#if ENABLE_INPUT_SYSTEM

            if (Mouse.current == null) return false;

            return 
                button == 0 ? Mouse.current.leftButton.isPressed 
                : button == 1 ? Mouse.current.rightButton.isPressed 
                : button == 2 && Mouse.current.middleButton.isPressed;

#else

            return Input.GetMouseButton(button);

#endif
        }
        
        public static bool GetMouseButtonUp(int button)
        {
#if ENABLE_INPUT_SYSTEM

            if (Mouse.current == null) return false;

            return 
                button == 0 ? Mouse.current.leftButton.wasReleasedThisFrame 
                : button == 1 ? Mouse.current.rightButton.wasReleasedThisFrame 
                : button == 2 && Mouse.current.middleButton.wasReleasedThisFrame;

#else

            return Input.GetMouseButtonUp(button);

#endif
        }

        public static bool GetMouseOrTouchDown(int button)
        {
            return GetMouseButtonDown(button) || GetTouchDown();
        }

        public static bool GetMouseOrTouchUp(int button)
        {
            return GetMouseButtonUp(button) || GetTouchUp();
        }

        private static bool GetTouchDown()
        {
#if ENABLE_INPUT_SYSTEM

            return Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame;
#else
            
            return Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began;

#endif
        }
        
        private static bool GetTouchUp()
        {
#if ENABLE_INPUT_SYSTEM

            return Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasReleasedThisFrame;
#else
            
            return Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Ended;

#endif
        }

        public static Vector2 GetMousePosition()
        {
#if ENABLE_INPUT_SYSTEM

            return Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;

#else

            return Input.mousePosition;

#endif
        }

        public static Vector2 GetTouchPosition()
        {
#if ENABLE_INPUT_SYSTEM

            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            {
                return Touchscreen.current.primaryTouch.position.ReadValue();
            }

#else

            if (Input.touchCount > 0)
            {
                return Input.GetTouch(0).position;
            }

#endif
            return Vector2.zero;
        }

        public static bool IsTouching()
        {
#if ENABLE_INPUT_SYSTEM

            if (Touchscreen.current != null)
            {
                return Touchscreen.current.primaryTouch.press.isPressed;
            }

            return false;
#else

            return (Input.touchCount > 0);

#endif
        }

        public static Vector2 GetMouseOrTouchPosition()
        {
#if ENABLE_INPUT_SYSTEM

            if (Touchscreen.current != null
                && Touchscreen.current.touches.Count > 0)
            {
                return Touchscreen.current.primaryTouch.position.ReadValue();
            }

            if (Mouse.current != null)
            {
                return Mouse.current.position.ReadValue();
            }
                
            return Vector2.zero;
#else
            
            return Input.mousePosition;

#endif

        }

        public static float GetMouseScrollDelta()
        {
#if ENABLE_INPUT_SYSTEM

            if (Mouse.current != null)
            {
                return Mouse.current.scroll.ReadValue().y;
            }

            return 0f;

#else
            
            return Input.mouseScrollDelta.y;

#endif
        }

        public static Touch GetTouch(int index)
        {
#if ENABLE_INPUT_SYSTEM

            if (Touchscreen.current != null && index < Touchscreen.current.touches.Count)
            {
                var touchState = Touchscreen.current.touches[index];
                return ConvertToOldTouch(touchState);
            }

            return new Touch();

#else

            return Input.GetTouch(index);

#endif
        }

#if ENABLE_INPUT_SYSTEM

        private static Touch ConvertToOldTouch(TouchControl touchState)
        {
            return new Touch
            {
                phase = ConvertTouchPhase(touchState.phase),
                position = touchState.position.ReadValue(),
                deltaPosition = touchState.delta.ReadValue()
            };
        }

        private static TouchPhase ConvertTouchPhase(TouchPhaseControl newPhase)
        {
            switch (newPhase.value)
            {
                case UnityEngine.InputSystem.TouchPhase.Began:
                    return TouchPhase.Began;
                case UnityEngine.InputSystem.TouchPhase.Moved:
                    return TouchPhase.Moved;
                case UnityEngine.InputSystem.TouchPhase.Stationary:
                    return TouchPhase.Stationary;
                case UnityEngine.InputSystem.TouchPhase.Ended:
                    return TouchPhase.Ended;
                case UnityEngine.InputSystem.TouchPhase.Canceled:
                    return TouchPhase.Canceled;
                default:
                    return TouchPhase.Canceled;
            }
        }

        private static readonly Key[] keyCodeToKeyMap = new Key[350];

        private static void CreateKeyCodes()
        {
            keyCodeToKeyMap[(int)KeyCode.A] = Key.A;
            keyCodeToKeyMap[(int)KeyCode.B] = Key.B;
            keyCodeToKeyMap[(int)KeyCode.C] = Key.C;
            keyCodeToKeyMap[(int)KeyCode.D] = Key.D;
            keyCodeToKeyMap[(int)KeyCode.E] = Key.E;
            keyCodeToKeyMap[(int)KeyCode.F] = Key.F;
            keyCodeToKeyMap[(int)KeyCode.G] = Key.G;
            keyCodeToKeyMap[(int)KeyCode.H] = Key.H;
            keyCodeToKeyMap[(int)KeyCode.I] = Key.I;
            keyCodeToKeyMap[(int)KeyCode.J] = Key.J;
            keyCodeToKeyMap[(int)KeyCode.K] = Key.K;
            keyCodeToKeyMap[(int)KeyCode.L] = Key.L;
            keyCodeToKeyMap[(int)KeyCode.M] = Key.M;
            keyCodeToKeyMap[(int)KeyCode.N] = Key.N;
            keyCodeToKeyMap[(int)KeyCode.O] = Key.O;
            keyCodeToKeyMap[(int)KeyCode.P] = Key.P;
            keyCodeToKeyMap[(int)KeyCode.Q] = Key.Q;
            keyCodeToKeyMap[(int)KeyCode.R] = Key.R;
            keyCodeToKeyMap[(int)KeyCode.S] = Key.S;
            keyCodeToKeyMap[(int)KeyCode.T] = Key.T;
            keyCodeToKeyMap[(int)KeyCode.U] = Key.U;
            keyCodeToKeyMap[(int)KeyCode.V] = Key.V;
            keyCodeToKeyMap[(int)KeyCode.W] = Key.W;
            keyCodeToKeyMap[(int)KeyCode.X] = Key.X;
            keyCodeToKeyMap[(int)KeyCode.Y] = Key.Y;
            keyCodeToKeyMap[(int)KeyCode.Z] = Key.Z;

            keyCodeToKeyMap[(int)KeyCode.Alpha0] = Key.Digit0;
            keyCodeToKeyMap[(int)KeyCode.Alpha1] = Key.Digit1;
            keyCodeToKeyMap[(int)KeyCode.Alpha2] = Key.Digit2;
            keyCodeToKeyMap[(int)KeyCode.Alpha3] = Key.Digit3;
            keyCodeToKeyMap[(int)KeyCode.Alpha4] = Key.Digit4;
            keyCodeToKeyMap[(int)KeyCode.Alpha5] = Key.Digit5;
            keyCodeToKeyMap[(int)KeyCode.Alpha6] = Key.Digit6;
            keyCodeToKeyMap[(int)KeyCode.Alpha7] = Key.Digit7;
            keyCodeToKeyMap[(int)KeyCode.Alpha8] = Key.Digit8;
            keyCodeToKeyMap[(int)KeyCode.Alpha9] = Key.Digit9;

            keyCodeToKeyMap[(int)KeyCode.Space] = Key.Space;
            keyCodeToKeyMap[(int)KeyCode.Return] = Key.Enter;
            keyCodeToKeyMap[(int)KeyCode.Escape] = Key.Escape;
            keyCodeToKeyMap[(int)KeyCode.Backspace] = Key.Backspace;
            keyCodeToKeyMap[(int)KeyCode.Tab] = Key.Tab;
            keyCodeToKeyMap[(int)KeyCode.LeftShift] = Key.LeftShift;
            keyCodeToKeyMap[(int)KeyCode.RightShift] = Key.RightShift;
            keyCodeToKeyMap[(int)KeyCode.LeftControl] = Key.LeftCtrl;
            keyCodeToKeyMap[(int)KeyCode.RightControl] = Key.RightCtrl;
            keyCodeToKeyMap[(int)KeyCode.LeftAlt] = Key.LeftAlt;
            keyCodeToKeyMap[(int)KeyCode.RightAlt] = Key.RightAlt;
            keyCodeToKeyMap[(int)KeyCode.LeftCommand] = Key.LeftMeta;
            keyCodeToKeyMap[(int)KeyCode.RightCommand] = Key.RightMeta;

            keyCodeToKeyMap[(int)KeyCode.UpArrow] = Key.UpArrow;
            keyCodeToKeyMap[(int)KeyCode.DownArrow] = Key.DownArrow;
            keyCodeToKeyMap[(int)KeyCode.LeftArrow] = Key.LeftArrow;
            keyCodeToKeyMap[(int)KeyCode.RightArrow] = Key.RightArrow;

            keyCodeToKeyMap[(int)KeyCode.F1] = Key.F1;
            keyCodeToKeyMap[(int)KeyCode.F2] = Key.F2;
            keyCodeToKeyMap[(int)KeyCode.F3] = Key.F3;
            keyCodeToKeyMap[(int)KeyCode.F4] = Key.F4;
            keyCodeToKeyMap[(int)KeyCode.F5] = Key.F5;
            keyCodeToKeyMap[(int)KeyCode.F6] = Key.F6;
            keyCodeToKeyMap[(int)KeyCode.F7] = Key.F7;
            keyCodeToKeyMap[(int)KeyCode.F8] = Key.F8;
            keyCodeToKeyMap[(int)KeyCode.F9] = Key.F9;
            keyCodeToKeyMap[(int)KeyCode.F10] = Key.F10;
            keyCodeToKeyMap[(int)KeyCode.F11] = Key.F11;
            keyCodeToKeyMap[(int)KeyCode.F12] = Key.F12;
        }

        public static bool TryConvertKeyCode(KeyCode keyCode, out Key newKey)
        {
            int index = (int)keyCode;
            if (index >= 0 && index < keyCodeToKeyMap.Length)
            {
                newKey = keyCodeToKeyMap[index];
                return true;
            }

            newKey = default;
            return false;
        }

#endif
    }

}