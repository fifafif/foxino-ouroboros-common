using Ouroboros.Common.Services;
using UnityEngine;

namespace Ouroboros.Common.UI.Windows.Utils
{
    [RequireComponent(typeof(Window))]
    public class ClickWindowCloser : MonoBehaviour
    {
        private Window window;
        private UISystem uiSystem;

        private void Awake()
        {
            window = GetComponent<Window>();
            window.OnCloseAction += OnClose;
            window.OnOpenAction += OnOpen;
        }

        private void OnOpen()
        {
            uiSystem = ServiceLocator.Get<UISystem>();
            uiSystem.OnMouseReleaseOnEmptySpace -= OnMouseReleaseOnEmptySpace;
            uiSystem.OnMouseReleaseOnEmptySpace += OnMouseReleaseOnEmptySpace;
        }

        private void OnMouseReleaseOnEmptySpace()
        {
            window.Close();
        }

        private void OnDestroy()
        {
            if (uiSystem != null)
            {
                uiSystem.OnMouseReleaseOnEmptySpace -= OnMouseReleaseOnEmptySpace;
            }
        }

        private void OnClose()
        {
            if (uiSystem != null)
            {
                uiSystem.OnMouseReleaseOnEmptySpace -= OnMouseReleaseOnEmptySpace;
            }
        }
    }
}