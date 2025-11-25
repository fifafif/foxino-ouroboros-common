using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Ouroboros.Common.Cameras
{
    public static class CameraEditorUtils
    {
        [MenuItem("Ouroboros/Utils/Find Main Camera")]
        public static void FindMainCamera()
        {
            var camera = Object.FindObjectsOfType<Camera>().FirstOrDefault(c => c.tag == "MainCamera");
            if (camera == null) return;

            Selection.activeObject = camera;
            EditorGUIUtility.PingObject(camera);
        }
    }
}