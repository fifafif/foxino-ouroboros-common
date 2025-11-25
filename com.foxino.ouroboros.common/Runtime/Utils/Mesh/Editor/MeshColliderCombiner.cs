// The script must be placed in an "Editor" folder in your Unity project.
// This allows it to use UnityEditor classes and function as a tool in the menu.

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Ouroboros.Common.Utils.Meshes
{
    public class MeshColliderCombiner : MonoBehaviour
    {
        [MenuItem("Ouroboros/Mesh/Combine Mesh Colliders on Children")]
        private static void CombineChildren()
        {
            GameObject parentObject = Selection.activeGameObject;

            if (parentObject == null)
            {
                Debug.LogError("Please select a single parent GameObject with children to combine.");
                return;
            }

            // Get all children of the selected parent.
            List<GameObject> childrenToCombine = new List<GameObject>();
            // foreach (Transform child in parentObject.transform)
            // {
            //     childrenToCombine.Add(child.gameObject);
            // }

            childrenToCombine = parentObject.GetComponentsInChildren<MeshCollider>().Where(c => c.name.Contains("side")).Select(c => c.gameObject).ToList();

            if (childrenToCombine.Count <= 1)
            {
                Debug.LogError("The selected parent object must have more than one child to combine.");
                return;
            }

            // Create a new GameObject to hold the combined mesh. This will be a child of the parent.
            GameObject combinedObject = new GameObject("CombinedMeshColliders");
            combinedObject.transform.SetParent(parentObject.transform);

            // The Mesh.CombineMeshes method takes an array of CombineInstance.
            List<CombineInstance> combineInstances = new List<CombineInstance>();
            List<GameObject> objectsToDisable = new List<GameObject>();

            // Loop through all child objects to gather their mesh data.
            foreach (GameObject obj in childrenToCombine)
            {
                // We need both a MeshFilter and a MeshCollider to perform the combination.
                // MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
                MeshCollider meshCollider = obj.GetComponent<MeshCollider>();

                if (meshCollider == null)
                {
                    Debug.LogWarning($"Skipping {obj.name} because it's missing a MeshFilter or MeshCollider component.");
                    continue;
                }

                // Create a CombineInstance for the current child's mesh.
                CombineInstance combine = new CombineInstance();

                // Set the mesh to be combined.
                combine.mesh = meshCollider.sharedMesh;

                // Get the world-space transform matrix of the object.
                // This is crucial to ensure the combined mesh retains the correct position, rotation, and scale
                // relative to the parent object.
                combine.transform = meshCollider.transform.localToWorldMatrix;

                // Add the CombineInstance to our list.
                combineInstances.Add(combine);

                // Mark the original child object for disabling after the process is complete.
                objectsToDisable.Add(obj);
            }

            if (combineInstances.Count == 0)
            {
                Debug.LogError("No valid child GameObjects with a MeshFilter and MeshCollider were found.");
                DestroyImmediate(combinedObject);
                return;
            }

            // Create a new mesh to hold the combined geometry.
            Mesh combinedMesh = new Mesh();

            // This is the core function call that combines all the meshes.
            // The 'mergeSubMeshes' argument is set to 'true' to combine all materials into one,
            // which is ideal for a single MeshCollider.
            combinedMesh.CombineMeshes(combineInstances.ToArray(), true, true);

            // Add a MeshFilter and MeshCollider to our new combined GameObject.
            MeshFilter newMeshFilter = combinedObject.AddComponent<MeshFilter>();
            MeshCollider newMeshCollider = combinedObject.AddComponent<MeshCollider>();

            // Assign the combined mesh to the new components.
            newMeshFilter.sharedMesh = combinedMesh;
            newMeshCollider.sharedMesh = combinedMesh;

            // Set the new object's local transform to identity to keep it at the same position
            // as the parent.
            combinedObject.transform.localPosition = Vector3.zero;
            combinedObject.transform.localRotation = Quaternion.identity;
            combinedObject.transform.localScale = Vector3.one;

            Debug.Log($"Successfully combined {objectsToDisable.Count} children of '{parentObject.name}' into a single MeshCollider.");

            // Disable the original children now that we have the combined version.
            foreach (GameObject obj in objectsToDisable)
            {
                obj.SetActive(false);
            }
        }
    }
}