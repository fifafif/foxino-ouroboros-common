using UnityEditor;
using UnityEngine;

namespace Ouroboros.Common.Utils
{
    public class EditorUpdaterWindow : EditorWindow
    {
        private bool isUpdating;

        [MenuItem("Ouroboros/Editor Updater")]
        public static void ShowWindow()
        {
            GetWindow(typeof(EditorUpdaterWindow));
        }

        void OnGUI()
        {
            if (!isUpdating)
            {
                if (GUILayout.Button("Start Updating"))
                {
                    isUpdating = true;

                    EditorApplication.update -= UpdateEditor;
                    EditorApplication.update += UpdateEditor;
                }
            }
            else
            {
                if (GUILayout.Button("Stop Updating"))
                {
                    isUpdating = false;

                    EditorApplication.update -= UpdateEditor;
                }
            }
        }

        private void UpdateEditor()
        {
            EditorApplication.QueuePlayerLoopUpdate();
        }
    }
}