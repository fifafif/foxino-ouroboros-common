using Ouroboros.Common.Services;
using Ouroboros.Common.Utils;
using UnityEngine;

namespace Ouroboros.Common.UI.Windows.Utils
{
    public class WindowOpener : MonoBehaviour
    {
        [SerializeField] private string windowId;
        [SerializeField] private string parameters;
        [SerializeField] private float openDelay;

        private void Start()
        {
            this.CoroutineWaitRealtime(openDelay, OpenWindow);
        }

        private void OpenWindow()
        {
            OpenWindow(windowId);
        }

        public void OpenWindow(string windowId)
        {
            var windowsManager = ServiceLocator.Get<WindowsManager>();

            if (string.IsNullOrEmpty(parameters))
            {
                windowsManager.OpenWindow(windowId);
            }
            else
            {
                windowsManager.OpenWindow(windowId, new ParamWindowConfig(parameters));
            }
        }
    }
}