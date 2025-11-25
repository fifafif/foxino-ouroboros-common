using Ouroboros.Common.Services;
using System;
using UnityEngine;

namespace Ouroboros.Common.UI.Windows
{
    public class WindowsRegistrar : MonoBehaviour
    {
        public Action OnRegistered { get; set; }

        [SerializeField] private string openWindowId;
        [SerializeField] private bool isKeepingWindowsParent;

        private WindowsManager windowsManager;
        private Window[] windows;

        private void Start()
        {
            windowsManager = ServiceLocator.Get<WindowsManager>();

            windows = GetComponentsInChildren<Window>(true);
            foreach (var window in windows)
            {
                window.gameObject.SetActive(false);
                windowsManager.RegisterWindow(window);

                if (!isKeepingWindowsParent)
                {
                    window.transform.SetParent(windowsManager.transform, false);
                }
            }

            OnRegistered?.Invoke();

            if (!string.IsNullOrEmpty(openWindowId))
            {
                windowsManager.CloseAllWindows();
                windowsManager.OpenWindow(openWindowId);
            }
        }

        private void OnDestroy()
        {
            if (windowsManager != null)
            {
                foreach (var window in windows)
                {
                    windowsManager.UnregisterWindow(window);
                    Destroy(window.gameObject);
                }
            }
        }
    }
}