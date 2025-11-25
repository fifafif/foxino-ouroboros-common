using Ouroboros.Common.Persistence;
using UnityEditor;

namespace Ouroboros.Common.Settings
{
    public static class SettingsEditorUtils
    {
        [MenuItem("Ouroboros/Clear Settings")]
        public static void ClearSettings()
        {
            SettingsManager.ClearSettings(new PlayerPrefsPersistence());
        }
    }
}