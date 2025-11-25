using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Ouroboros.Common.Logging;
using Ouroboros.Common.Persistence;
using Ouroboros.Common.Services;
using UnityEngine;

namespace Ouroboros.Common.Settings
{
    public class SettingsManager : MonoBehaviour
    {
        private const string SaveKey = "ouroboros_settings";

        public Task<bool> InitTask => initTsc.Task;

        [SerializeField] private SettingsScriptableData settingsScriptableData;

        private readonly TaskCompletionSource<bool> initTsc = new TaskCompletionSource<bool>();

        private IPersistence persistence;

        private Dictionary<string, object> keyValuePairs;

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private async void Start()
        {
            persistence = ServiceLocator.Get<IPersistence>();
            Load();

            initTsc.SetResult(true);
        }

        public void Load()
        {
            Logs.Debug<SettingsManager>($"Loading");
            
            keyValuePairs = persistence.Load<Dictionary<string, object>>(SaveKey);
            if (keyValuePairs == null)
            {
                keyValuePairs = new Dictionary<string, object>();
            }

            Logs.Debug<SettingsManager>($"Loaded settings: {keyValuePairs.PrintInline()}");

            LoadDefaults(settingsScriptableData.Data);

            Logs.Debug<SettingsManager>($"Loaded!");
        }

        public void Save()
        {
            Logs.Debug<SettingsManager>($"Save!");
            persistence.Save(keyValuePairs, SaveKey);
        }

        public void LoadDefaults(SettingsData settingsData)
        {
            foreach (var kvp in settingsData.KeyValues)
            {
                if (keyValuePairs.ContainsKey(kvp.Key)) continue;

                keyValuePairs.Add(kvp.Key, kvp.GetValue());
                Logs.Debug<SettingsManager>($"Loaded default settings: {kvp.Key}={kvp.GetValue()}");
            }
        }

        public bool GetSettingsBool(string key, bool defaultValue = false)
        {
            if (!keyValuePairs.TryGetValue(key, out var value))
            {
                return defaultValue;
            }

            return (bool)value;
        }

        public void SetSettings(string key, bool value)
        {
            keyValuePairs[key] = value;
            Save();
        }

        public float GetSettingsFloat(string key, float defaultValue = 0f)
        {
            if (!keyValuePairs.TryGetValue(key, out var value))
            {
                return defaultValue;
            }

            return Convert.ToSingle(value);
        }

        public void SetSettings(string key, float value)
        {
            keyValuePairs[key] = value;
            Save();
        }

        public void SetSettings<T>(string key, T value)
        {
            keyValuePairs[key] = JsonConvert.SerializeObject(value);
            Save();
        }

        public bool HasSettings(string key)
        {
            return keyValuePairs.ContainsKey(key);
        }

        public T GetSettings<T>(string key, T defaultValue)
        {
            if (!keyValuePairs.TryGetValue(key, out var value))
            {
                return defaultValue;
            }
            
            T data;
            try
            {
                data = JsonConvert.DeserializeObject<T>(value.ToString());
            }
            catch (Exception e)
            {
                Logs.Error<SettingsManager>($"Could not deserialize settings! key={key}, value={value}, type={typeof(T)}");
                return defaultValue;
            }

            return data;
        }

        public static void ClearSettings(IPersistence playerPrefsPersistence)
        {
            Logs.Debug<SettingsManager>($"ClearSettings!");
            playerPrefsPersistence.Delete(SaveKey);
        }
    }
}