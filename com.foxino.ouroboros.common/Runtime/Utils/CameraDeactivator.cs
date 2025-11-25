using UnityEngine;

namespace Ouroboros.Common.Utils
{
    public class CameraDeactivator : MonoBehaviour
    {
        public Camera Behaviour;
        public bool IsDisabled;
        public bool IsDisablingForever;

        private void Awake()
        {
            if (IsDisablingForever)
            {
                gameObject.SetActive(false);
                return;
            }

#if UNITY_EDITOR
            if (IsDisabled) return;
#endif

#if UNITY_EDITOR_OSX
            return;
#endif

            Activate(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                Toggle();
            }

            if (Input.GetMouseButtonDown(0))
            {
                Activate(true);
            }
        }

        private void Activate(bool isActive)
        {
            Behaviour.enabled = isActive;
        }

        private void Toggle()
        {
            Behaviour.enabled = !Behaviour.enabled;
        }
    }
}