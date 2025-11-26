using UnityEngine;
using UnityEditor;
using System.IO;

namespace Ouroboros.Common.Utils.Meshes
{
    public class SaveMeshAssetWindow : EditorWindow
    {
        public GameObject target;
        public string folder = "Assets/Models/Generated";
        public string assetName = "Mesh";
        public bool replaceInScene = true;

        [MenuItem("Ouroboros/Mesh/Save Mesh Asset")]
        public static void ShowWindow()
        {
            GetWindow(typeof(SaveMeshAssetWindow));
        }

        void OnGUI()
        {
            target = EditorGUILayout.ObjectField("Target", target, typeof(GameObject), true) as GameObject;
            folder = EditorGUILayout.TextField("Folder name", folder);
            assetName = EditorGUILayout.TextField("Asset name", assetName);
            replaceInScene = EditorGUILayout.Toggle("Replace In Scene", replaceInScene);

            if (GUILayout.Button("Make it happen"))
            {
                Save();
            }
        }

        private void Save()
        {
            if (string.IsNullOrEmpty(folder) || string.IsNullOrEmpty(assetName))
            {
                Debug.LogError("Missing folder name or asset name!");

                return;
            }

            var meshes = target.GetComponentsInChildren<MeshFilter>();

            var path = Path.Combine(folder, assetName);

            // TODO: Fix this!
            if (AssetDatabase.FindAssets(path).Length > 0)
            {
                AssetDatabase.DeleteAsset(path);

                Debug.LogError("Asset Already exists");
            }

            path = string.Format("{0}.asset", path);

            var meshClone = Instantiate(meshes[0].sharedMesh);

            AssetDatabase.CreateAsset(meshClone, path);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(meshClone));

            if (replaceInScene)
            {
                meshes[0].sharedMesh = meshClone;
            }

            for (int i = 1; i < meshes.Length; ++i)
            {
                if (string.IsNullOrEmpty(meshes[i].sharedMesh.name))
                {
                    meshes[i].sharedMesh.name = assetName + i;
                }

                meshClone = Instantiate(meshes[i].sharedMesh);
                meshClone.name = meshes[i].sharedMesh.name;

                AssetDatabase.AddObjectToAsset(meshClone, path);
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(meshClone));

                Debug.Log(AssetDatabase.GetAssetPath(meshClone));

                if (replaceInScene)
                {
                    meshes[i].sharedMesh = meshClone;
                }
            }

            AssetDatabase.SaveAssets();
        }
    }
}