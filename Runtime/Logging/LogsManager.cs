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
                    var type = GetTypeFromContext(active);
                    if (type == null)
                    {
                        Debug.LogError($"[LogsManager] Cannot activate or deactivate Type! Missing Assembly={active.Assembly}, Type={active.Type}");
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

        private static Type GetTypeFromContext(ActiveLogContexts.Context context)
        {
            // If assembly is specified, look in that specific assembly
            if (!string.IsNullOrEmpty(context.Assembly))
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var assembly in assemblies)
                {
                    if (assembly.GetName().Name == context.Assembly)
                    {
                        var type = assembly.GetType(context.Type);
                        if (type != null)
                        {
                            return type;
                        }
                    }
                }
            }

            // Fallback: try Type.GetType which searches mscorlib and executing assembly
            var fallbackType = Type.GetType(context.Type);
            if (fallbackType != null)
            {
                return fallbackType;
            }

            // Final fallback: search all assemblies
            return GetType(context.Type);
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