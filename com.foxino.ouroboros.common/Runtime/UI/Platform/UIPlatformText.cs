using TMPro;
using UnityEngine;

namespace Ouroboros.Common.UI.Platform
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class UIPlatformText : UIPlatformBase
    {
        [SerializeField] private UIPlatformValueFloat textSize;
        [SerializeField] private UITextSizeScriptable textSizeScriptable;

        public override void SetMobile()
        {
            SetText(GetTextSize(UIPlatformType.Mobile));
        }

        public override void SetStandalone()
        {
            SetText(GetTextSize(UIPlatformType.Standalone));
        }

        private float GetTextSize(UIPlatformType platformType)
        {
            if (textSizeScriptable != null)
            {
                return textSizeScriptable.TextSize.GetValue(platformType);
            }

            return textSize.GetValue(platformType);
        }

        private void SetText(float size)
        {
            GetComponent<TextMeshProUGUI>().fontSize = size;
        }

    }
}
