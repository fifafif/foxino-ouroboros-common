using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class PlaceInSpaceSphere : MonoBehaviour
{
    public GameObject target;
    public GameObject parent;
    public int count;
    public float outerRadius;
    public float innerRadius;
    public float elementRadius;
    public float elementSize = 1f;
    public float elementSizeRandom = 0f;
    public AnimationCurve distanceToSizeCurve;
    public bool useDistanceToSizeCurve;

    public bool PlaceNow;

    private void Update()
    {
        if (PlaceNow)
        {
            PlaceNow = false;

            Create();
        }
    }

    private void Create()
    {
        for (int i = parent.transform.childCount - 1; i >= 0; --i)
        {
            DestroyImmediate(parent.transform.GetChild(i).gameObject);
        }
        
        for (int i = 0; i < count; ++i)
        {
            int iter = 0;

            
            while (++iter < 10000)
            {
                var pos = Random.insideUnitSphere * outerRadius;

                if (pos.sqrMagnitude <= innerRadius * innerRadius)
                {
                    continue;
                }

                bool success = true;

                for (int o = 0; o < parent.transform.childCount; ++o)
                {
                    if ((parent.transform.GetChild(o).localPosition - pos).sqrMagnitude < 2 * elementRadius)
                    {
                        success = false;
                        break;
                    }
                }

                if (success)
                {
                    GameObject go;

#if UNITY_EDITOR

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

#endif
                    {
                        go = Instantiate(target) as GameObject;
                    }

                    go.transform.parent = parent.transform;
                    go.transform.localPosition = pos;
                    go.transform.localRotation = Random.rotation;

                    var scale = 1f;

                    if (useDistanceToSizeCurve
                        && outerRadius > 0f)
                    {
                        var d = pos.magnitude / outerRadius;
                        scale = distanceToSizeCurve.Evaluate(d);
                    }

                    go.transform.localScale = go.transform.localScale * Random.Range(1 - elementSizeRandom, 1 + elementSizeRandom) * elementSize * scale;

                    

                    break;
                }
            }
        }
    }
}
