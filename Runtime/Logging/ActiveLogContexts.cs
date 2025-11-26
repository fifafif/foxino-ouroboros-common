using System;
using UnityEngine;

namespace Ouroboros.Common.Logging
{
    [CreateAssetMenu(fileName = "dat_logs_active", menuName = "Ouroboros/Logs Active")]
    public class ActiveLogContexts : ScriptableObject
    {
        [Serializable]
        public class Context
        {
            public string Assembly;
            public string Type;
            public bool IsActive;

            public string GetClassName()
            {
                return Type.Substring(Type.LastIndexOf('.') + 1);
            }
        }

        public Context[] Contexts;
        public bool IsUnspecifiedContextActive;
    }
}