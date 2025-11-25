using Ouroboros.Common.UI.Windows.Windows;
using System;

namespace Ouroboros.Common.UI.Windows.Utils
{
    public static class WindowUtils
    {
        public static ConfirmationPopupWindow OpenConfirmationQuitGame(
            WindowsManager windowManager, Action onConfirm, Action onCancel)
        {
            return windowManager.OpenWindow<ConfirmationPopupWindow>(
                new ConfirmationPopupWindow.Config
                {
                    Title = "Quit Game?",
                    Message = "Do you want to quit current game and go to home menu?",
                    Confirm = "Yes",
                    Cancel = "No",
                    OnConfirm = onConfirm,
                    OnCancel = onCancel
                });
        }
    }
}