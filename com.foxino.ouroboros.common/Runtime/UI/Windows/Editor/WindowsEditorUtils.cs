using Ouroboros.Common.UI.Platform;
using UnityEditor;
using UnityEngine;

namespace Ouroboros.Common.UI.Windows
{
    public static class WindowsEditorUtils
    {
        [MenuItem("Ouroboros/UI/Select Opened Window", priority = 210)]
        public static void SelectOpenedWindow()
        {
            var windowsManager = Object.FindObjectOfType<WindowsManager>();
            if (windowsManager == null) return;

            Selection.activeObject = windowsManager.GetTopWindow();
            EditorGUIUtility.PingObject(Selection.activeObject);
        }

        [MenuItem("Ouroboros/UI/Refresh UI Platform", priority = 211)]
        public static void RefreshUIPlatform()
        {
            var objects = Object.FindObjectsOfType<UIPlatformBase>(false);
            foreach (var obj in objects)
            {
                Debug.Log($"UI Platform udpated on {obj.name}", obj);
                obj.SetFromPlatform();
            }
        }
    }
}
