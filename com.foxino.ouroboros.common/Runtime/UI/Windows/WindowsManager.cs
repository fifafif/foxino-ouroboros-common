using Ouroboros.Common.Logging;
using Ouroboros.Common.Platform;
using Ouroboros.Common.Services;
using Ouroboros.Common.UI.Platform;
using Ouroboros.Common.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ouroboros.Common.UI.Windows
{
    public class WindowsManager : MonoBehaviour
    {
        private List<Window> windows = new List<Window>();
        private List<Window> openedWindows = new List<Window>();

        public void Awake()
        {
            ServiceLocator.Register(this);

            var allWindows = GetComponentsInChildren<Window>(true);
            var platformType = PlatformUtils.IsMobile() ? UIPlatformType.Mobile : UIPlatformType.Standalone;
            
            foreach (var window in allWindows)
            {
                if (!window.IsOnCorrectPlatform(platformType)) continue;

                windows.Add(window);
                window.gameObject.SetActive(false);
            }
        }

        public void CloseAllWindows()
        {
            for (var i = openedWindows.Count - 1; i >= 0; --i)
            {
                if (openedWindows[i] == null
                    || openedWindows[i].gameObject == null)
                {
                    openedWindows.RemoveAt(i);
                    continue;
                }

                openedWindows[i].Close();
            }
        }

        public T OpenWindow<T>(WindowConfig config = null) where T : Window
        {
            var window = GetWindow<T>();
            if (window == null)
            {
                Debug.LogError($"[WindowManager] Window not found! Type={typeof(T)}");
                return null;
            }

            OpenWindowInternal(window, config);

            return window;
        }
        
        public T OpenWindowIfNotOpen<T>(WindowConfig config = null) where T : Window
        {
            if (IsWindowOpen<T>())
            {
                return GetOpenedWindow<T>();
            }

            return OpenWindow<T>(config);
        }

        public Window OpenWindow(string id, WindowConfig config = null)
        {
            var window = GetWindow(id);
            if (window == null)
            {
                Debug.LogError($"[WindowManager] Window not found! Id={id}");
                return null;
            }

            OpenWindowInternal(window, config);

            return window;
        }
        
        public Window OpenPopup(string id, WindowConfig config = null)
        {
            var window = GetWindow(id);
            if (window == null)
            {
                Debug.LogError($"[WindowManager] Window not found! Id={id}");
                return null;
            }

            OpenWindowInternal(window, config);

            return window;
        }

        public void CloseWindow<T>() where T : Window
        {
            var window = GetOpenedWindow<T>();
            if (window == null) return;

            CloseWindowInternal(window);
        }

        public void CloseWindow(Window window)
        {
            CloseWindowInternal(window);
        }

        public void CloseWindow(string id)
        {
            var window = GetWindow(id);
            if (window == null)
            {
                Debug.LogError($"[WindowManager] Window not found! Id={id}");
                return;
            }

            CloseWindowInternal(window);
        }

        private void OpenWindowInternal(Window window, WindowConfig config)
        {
            var top = GetTopWindow();
            if (top != null)
            {
                top.LoseFocus();
            }

            Logs.Debug<WindowsManager>($"Open Widnow {window}");

            InitOpenedWindow(window, config);
        }

        private void InitOpenedWindow(Window window, WindowConfig config)
        {
            window.Init(this, config);
            window.Open();

            openedWindows.Remove(window);
            openedWindows.Add(window);
        }

        private void CloseWindowInternal(Window window)
        {
            Logs.Debug<WindowsManager>($"Close Widnow {window}");
            window.Close();

            var isTop = IsTopWindow(window);

            openedWindows.RemoveBackward(window);

            if (isTop)
            {
                var top = GetTopWindow();
                if (top != null)
                {
                    top.AcquireFocus();
                }
            }
        }

        public void RemoveClosedWindow(Window window)
        {
            openedWindows.Remove(window);
        }

        private T GetWindow<T>() where T : Window
        {
            foreach (var window in windows)
            {
                if (window is T)
                {
                    return window as T;
                }
            }

            return null;
        }

        private Window GetWindow(string id)
        {
            foreach (var window in windows)
            {
                if (window.Id == id)
                {
                    return window;
                }
            }

            return null;
        }

        public T GetOpenedWindow<T>() where T : Window
        {
            foreach (var window in openedWindows)
            {
                if (window is T openedWindow)
                {
                    return openedWindow;
                }
            }

            return null;
        }

        public Window GetOpenedWindow(string id)
        {
            foreach (var window in openedWindows)
            {
                if (window.Id == id)
                {
                    return window;
                }
            }

            return null;
        }

        public Window GetTopWindow()
        {
            return openedWindows.Last();
        }

        public bool IsTopWindow(Window window)
        {
            return window == GetTopWindow();
        }

        public bool IsWindowOpen(string id)
        {
            return GetOpenedWindow(id) != null;
        }

        public bool IsWindowOpen<T>() where T : Window
        {
            return GetOpenedWindow<T>() != null;
        }

        public void RegisterWindow(Window window)
        {
            if (!window.IsOnCorrectPlatform(UIPlatformUtils.GetUIPlatformType())) return;
            
            windows.Add(window);
        }

        public void UnregisterWindow(Window window)
        {
            windows.Remove(window);
            openedWindows.Remove(window);
        }
    }
}
