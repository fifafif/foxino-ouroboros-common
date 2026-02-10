using Ouroboros.Common.Services;
using UnityEngine;
using UnityEngine.UI;

namespace Ouroboros.Common.UI.Windows.Utils
{
    [RequireComponent(typeof(Button))]
    public class ButtonWindowOpener : MonoBehaviour
    {
        [SerializeField] private string windowId;
        [Tooltip("Comma separated key=values. Ie.: level=1-1,difficulty=hard")]
        [SerializeField] private string parameters;
        [SerializeField] private bool isClosingCurrentWindow;

        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(OnButtonClick);
        }

        public void OnButtonClick()
        {
            if (isClosingCurrentWindow)
            {
                var window = GetComponentInParent<Window>();
                if (window != null)
                {
                    window.Close();
                }
            }

            if (string.IsNullOrEmpty(windowId)) return;

            var config = new ParamWindowConfig(parameters);

            if (ServiceLocator.TryGet<UISystem>(out var uiSystem))
            {
                uiSystem.WindowManager.OpenWindow(windowId, config);
            }
            else
            {
                var windowsManager = GetComponentInParent<WindowsManager>();
                if (windowsManager == null)                
                {
                    Debug.LogError($"[ButtonWindowOpener] No WindowsManager found!");
                    return;
                }

                windowsManager.OpenWindow(windowId, config);
            }
        }
    }
}