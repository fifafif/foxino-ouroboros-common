using Ouroboros.Common.Platform;
using UnityEditor;

namespace Ouroboros.Common.Logging
{
    public class LogsEditor
    {
        [MenuItem("Ouroboros/Logs/Enable Logs")]
        public static void EnableLogs()
        {
            PlatformUtilsEditor.AddToScriptDefines(Logs.LogDefine);
        }

        [MenuItem("Ouroboros/Logs/Enable Logs", true)]
        public static bool EnableLogsCheck()
        {
            return !PlatformUtilsEditor.ScriptDefinesContains(Logs.LogDefine);
        }

        [MenuItem("Ouroboros/Logs/Disable Logs")]
        public static void DisableLogs()
        {
            PlatformUtilsEditor.RemoveFromScriptDefines(Logs.LogDefine);
        }

        [MenuItem("Ouroboros/Logs/Disable Logs", true)]
        public static bool DisableLogsCheck()
        {
            return PlatformUtilsEditor.ScriptDefinesContains(Logs.LogDefine);
        }

        [MenuItem("Ouroboros/Logs/Enable Verbose Logs")]
        public static void EnableVerboseLogs()
        {
            PlatformUtilsEditor.AddToScriptDefines(Logs.LogDefineVerbose);
        }

        [MenuItem("Ouroboros/Logs/Enable Verbose Logs", true)]
        public static bool EnableVerboseLogsCheck()
        {
            return !PlatformUtilsEditor.ScriptDefinesContains(Logs.LogDefineVerbose);
        }

        [MenuItem("Ouroboros/Logs/Disable Verbose Logs")]
        public static void DisableVerboseLogs()
        {
            PlatformUtilsEditor.RemoveFromScriptDefines(Logs.LogDefineVerbose);
        }

        [MenuItem("Ouroboros/Logs/Disable Verbose Logs", true)]
        public static bool DisableVerboseLogsCheck()
        {
            return PlatformUtilsEditor.ScriptDefinesContains(Logs.LogDefineVerbose);
        }
    }
}