using System.Collections.Generic;

namespace Ouroboros.Common.UI.Windows.Utils
{
    public class ParamWindowConfig : WindowConfig
    {
        public readonly Dictionary<string, string> paramMap = new Dictionary<string, string>();

        public ParamWindowConfig()
        {
        }

        public ParamWindowConfig(string key, string value)
        {
            paramMap = new Dictionary<string, string>()
            {
                { key, value }
            };
        }

        public ParamWindowConfig(string parameters)
        {
            var paramSplit = parameters.Split(',');
            foreach(var param in paramSplit)
            {
                var keyValueSplit = param.Split('=');
                if (keyValueSplit.Length != 2) continue;

                paramMap[keyValueSplit[0]] = keyValueSplit[1];
            }
        }

        public bool TryGetParameterValue(string key, out string value)
        {
            return paramMap.TryGetValue(key, out value);
        }

        public bool HasParamWithValue(string key, string value)
        {
            return 
                paramMap.TryGetValue(key, out var storedValue)
                && storedValue == value;
        }
    }
}