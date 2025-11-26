using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Ouroboros.Common.Platform
{
    public class PlatformUtilsEditor
    {
        public static void AddToScriptDefines(string scriptDefineToAdd)
        {
            AddToScriptDefines(scriptDefineToAdd, GetCurrentBuildTargetGroup());
        }

        public static void AddToScriptDefines(string scriptDefineToAdd, BuildTargetGroup group)
        {
            var defines = GetScriptDefines(group);

            if (defines.Contains(scriptDefineToAdd)) return;

            defines.Add(scriptDefineToAdd);

            SetScriptDefines(defines, group);
        }

        public static BuildTargetGroup GetCurrentBuildTargetGroup()
        {
            var target = EditorUserBuildSettings.activeBuildTarget;
            return BuildPipeline.GetBuildTargetGroup(target);
        }

        public static void RemoveFromScriptDefines(string scriptDefineToRemove)
        {
            RemoveFromScriptDefines(scriptDefineToRemove, GetCurrentBuildTargetGroup());
        }

        public static void RemoveFromScriptDefines(string scriptDefineToRemove, BuildTargetGroup group)
        {
            var defines = GetScriptDefines(group);

            if (!defines.Contains(scriptDefineToRemove)) return;

            defines = defines.Where(d => d != scriptDefineToRemove).ToList();

            SetScriptDefines(defines, group);
        }

        public static void RemoveFromScriptDefines(string[] scriptDefinesToRemove, BuildTargetGroup group)
        {
            var defines = GetScriptDefines(group);

            defines = defines.Where(d => !scriptDefinesToRemove.Contains(d)).ToList();

            SetScriptDefines(defines, group);
        }

        public static List<string> GetScriptDefines(BuildTargetGroup group)
        {
            var definesText = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
            return definesText.Split(';').ToList();
        }

        public static void SetScriptDefines(List<string> scriptDefines, BuildTargetGroup group)
        {
            var definesText = string.Join(";", scriptDefines);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, definesText);
        }

        public static bool ScriptDefinesContains(string scriptDefine)
        {
            var group = GetCurrentBuildTargetGroup();
            var defines = GetScriptDefines(group);

            return defines.Contains(scriptDefine);
        }
    }
}