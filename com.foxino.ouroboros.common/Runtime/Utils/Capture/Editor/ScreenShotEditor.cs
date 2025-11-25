using UnityEngine;
using UnityEditor;

namespace Ouroboros.Common.Utils.Capture
{
    [CustomEditor(typeof(ScreenShot))]
    public class ScreenShotEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var screenShot = (ScreenShot)target;

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("SD"))
            {
                screenShot.resWidth = 640;
                screenShot.resHeight = 480;
            }

            if (GUILayout.Button("HD"))
            {
                screenShot.resWidth = 1280;
                screenShot.resHeight = 720;
            }

            if (GUILayout.Button("FHD"))
            {
                screenShot.resWidth = 1920;
                screenShot.resHeight = 1080;
            }

            if (GUILayout.Button("UHD"))
            {
                screenShot.resWidth = 1920 * 2;
                screenShot.resHeight = 1080 * 2;
            }

            if (GUILayout.Button("2K"))
            {
                screenShot.resWidth = 2048;
                screenShot.resHeight = 1024;
            }

            if (GUILayout.Button("4K"))
            {
                screenShot.resWidth = 4096;
                screenShot.resHeight = 2048;
            }

            GUILayout.EndHorizontal();

            if (GUILayout.Button("Take ScreenShot"))
            {
                screenShot.TakeScreenShot();
            }

            if (GUILayout.Button("Open Folder"))
            {
                EditorUtility.RevealInFinder(ScreenShot.DirectoryPath);
            }
        }
    }
}