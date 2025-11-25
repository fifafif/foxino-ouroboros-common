using UnityEngine;

namespace Ouroboros.Common.Settings
{
    [CreateAssetMenu(fileName = "dat_settings.asset", menuName = "Ouroboros/Settings Data")]
    public class SettingsScriptableData : ScriptableObject
    {
        public SettingsData Data => data;
        
        [SerializeField] private SettingsData data;
    }
}