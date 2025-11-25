using Newtonsoft.Json;
using Ouroboros.Common.Logging;
using UnityEngine;

namespace Ouroboros.Common.Persistence
{
    public class PlayerPrefsPersistence : IPersistence
    {
        public void Save<T>(T data, string key)
        {
            var json = JsonConvert.SerializeObject(data);
            PlayerPrefs.SetString(key, json);

            Logs.Debug<PlayerPrefsPersistence>($"Saved json data={json}, key={key}");
        }

        public T Load<T>(string key)
        {
            var json = PlayerPrefs.GetString(key);
            if (string.IsNullOrEmpty(json))
            {
                Logs.Debug<PlayerPrefsPersistence>($"No data for key={key}");

                return default;
            }

            Logs.Debug<PlayerPrefsPersistence>($"Loaded json data={json}, key={key}");

            return JsonConvert.DeserializeObject<T>(json);
        }

        public void Delete(string key)
        {
            PlayerPrefs.DeleteKey(key);

            Logs.Debug<PlayerPrefsPersistence>($"Deleted key={key}");
        }

        public bool HasData(string key)
        {
            return PlayerPrefs.HasKey(key);
        }
    }
}