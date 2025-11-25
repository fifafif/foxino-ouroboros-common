using UnityEngine;

namespace Ouroboros.Common.UI.Platform
{
    [CreateAssetMenu(fileName = "dat_ui_text_size_scriptable", menuName = "Ouroboros/UI/UITextSizeScriptable")]
    public class UITextSizeScriptable : ScriptableObject
    {
        public UIPlatformValueFloat TextSize;
    }
}