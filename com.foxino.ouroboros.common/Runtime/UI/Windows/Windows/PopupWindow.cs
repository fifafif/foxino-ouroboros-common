using System;
using TMPro;
using UnityEngine;

namespace Ouroboros.Common.UI.Windows.Windows
{
    public class PopupWindow : Window
    {
        public class Config : WindowConfig
        {
            public string Title;
            public string Message;
            public string Confirm = "Yes";
            public Action OnConfirm;
        }

        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private TextMeshProUGUI confirmText;

        private Config config;

        public override void OnInit(WindowConfig windowConfig)
        {
            config = windowConfig as Config;

            titleText.text = config.Title;
            messageText.text = config.Message;
            confirmText.text = config.Confirm;
        }

        public void OnClickConfirm()
        {
            config.OnConfirm?.Invoke();
            Close();
        }
    }
}
