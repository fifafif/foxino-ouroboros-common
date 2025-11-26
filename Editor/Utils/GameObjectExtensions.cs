using UnityEditor;
using UnityEngine;

public static class GameObjectExtensions
{
    public static GameObject InstantiatePrefab(this GameObject target)
    {
        GameObject go;

        if (PrefabUtility.GetPrefabType(target) == PrefabType.PrefabInstance)
        {
            Object prefab = PrefabUtility.GetCorrespondingObjectFromSource(target);
            var goPref = PrefabUtility.InstantiatePrefab(prefab);
            go = goPref as GameObject;
        }
        else if (PrefabUtility.GetPrefabType(target) == PrefabType.Prefab)
        {
            var goPref = PrefabUtility.InstantiatePrefab(target);
            go = goPref as GameObject;
        }
        else
        {
            go = GameObject.Instantiate(target) as GameObject;
        }

        return go;
    }
}