using System;
using TMPro;
using UnityEngine;

namespace Ouroboros.Common.UI.Windows.Windows
{
    public class ConfirmationPopupWindow : Window
    {
        public class Config : WindowConfig
        {
            public string Title;
            public string Message;
            public string Confirm = "Yes";
            public string Cancel = "No";
            public Action OnCancel;
            public Action OnConfirm;
        }

        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private TextMeshProUGUI confirmText;
        [SerializeField] private TextMeshProUGUI cancelText;

        private Config config;

        public override void OnInit(WindowConfig windowConfig)
        {
            config = windowConfig as Config;

            titleText.text = config.Title;
            messageText.text = config.Message;
            confirmText.text = config.Confirm;
            cancelText.text = config.Cancel;
        }

        public void OnClickCancel()
        {
            config.OnCancel?.Invoke();
            Close();
        }

        public void OnClickConfirm()
        {
            config.OnConfirm?.Invoke();
            Close();
        }
    }
}
