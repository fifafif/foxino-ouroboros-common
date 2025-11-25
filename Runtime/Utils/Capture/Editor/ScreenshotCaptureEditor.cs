using UnityEngine;
using UnityEditor;

namespace Ouroboros.Utils.Capture
{
    [CustomEditor(typeof(ScreenshotCapture))]
    public class ScreenshotCaptureEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var screenShot = (ScreenshotCapture)target;

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("SD"))
            {
                SetResolution(screenShot, 640, 480);
            }

            if (GUILayout.Button("HD"))
            {
                SetResolution(screenShot, 1280, 720);
            }

            if (GUILayout.Button("FHD"))
            {
                SetResolution(screenShot, 1920, 1080);
            }

            if (GUILayout.Button("UHD"))
            {
                SetResolution(screenShot, 1920 * 2, 1080 * 2);
            }

            if (GUILayout.Button("2K"))
            {
                SetResolution(screenShot, 2048, 1024);
            }

            if (GUILayout.Button("4K"))
            {
                SetResolution(screenShot, 4096, 2048);
            }

            GUILayout.EndHorizontal();

            if (GUILayout.Button("Take Screenshot"))
            {
                screenShot.StartCapture();
            }

            if (GUILayout.Button("Open Folder"))
            {
                EditorUtility.RevealInFinder(ScreenshotCapture.DirectoryPath);
            }
        }

        private static void SetResolution(ScreenshotCapture screenShot, int width, int height)
        {
            screenShot.ResolutionWidth = width;
            screenShot.ResolutionHeight = height;
            EditorUtility.SetDirty(screenShot);
        }
    }
}