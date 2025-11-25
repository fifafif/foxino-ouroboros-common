using System;
using UnityEngine;

namespace Ouroboros.Common.Logging
{
    [DefaultExecutionOrder(-10000)]
    public class LogsManager : MonoBehaviour
    {
        [SerializeField] private ActiveLogContexts activeLogContexts;

        private void Awake()
        {
            Logs.ClearActiveTypes();
            SetupActiveTypes();
        }

        private void SetupActiveTypes()
        {

#if !UNITY_EDITOR

            Logs.IsUnspecifiedContextActive = true;

#else

            if (activeLogContexts != null)
            {
                foreach (var active in activeLogContexts.Contexts)
                {
                    var type = Type.GetType(active.Type);
                    if (type == null)
                    {
                        Debug.LogError($"[LogsManager] Cannot activate or deactivate Type! Missing Type={active.Type}");
                        continue;
                    }

                    if (active.IsActive)
                    {
                        Logs.ActivateType(type);
                    }
                    else
                    {
                        Logs.DeactivateType(type);
                    }
                }

                Logs.IsUnspecifiedContextActive = activeLogContexts.IsUnspecifiedContextActive;
            }

#endif

        }

        public static Type GetType(string typeName)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                var type = assembly.GetType(typeName);
                if (type != null)
                {
                    return type;
                }
            }

            return null;
        }
    }
}