using System;

namespace Ouroboros.Common.Settings
{
    [Serializable]
    public class SettingsData
    {
        [Serializable]
        public class KeyValue
        {
            public string Key;
            public string Value;

            public bool TryGetFloat(out float value)
            {
                return float.TryParse(Value, out value);
            }

            public bool TryGetInt(out int value)
            {
                return int.TryParse(Value, out value);
            }

            public bool TryGetBool(out bool value)
            {
                return bool.TryParse(Value, out value);
            }

            public object GetValue()
            {
                if (TryGetFloat(out var valueFloat))
                {
                    return valueFloat;
                }

                if (TryGetInt(out var valueInt))
                {
                    return valueInt;
                }

                if (TryGetBool(out var valueBool))
                {
                    return valueBool;
                }

                return Value;
            }
        }

        public KeyValue[] KeyValues;
    }
}