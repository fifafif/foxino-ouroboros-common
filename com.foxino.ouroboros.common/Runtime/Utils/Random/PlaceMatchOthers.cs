using Ouroboros.Common.Utils.Transforms;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlaceMatchOthers : MonoBehaviour
{
    public GameObject[] target = new GameObject[0];
    public float[] angleLimits = new float[0];
    public float angleLimitOverlap;
    public Transform othersParent;
    public GameObject parent;
    public float elementSize = 1f;
    public bool useElementScale;
    public Vector3 otherOffset;

    public Vector3 randomPosition;
    public Vector3 randomRotation;

    public bool PlaceNow;

    void Update()
    {
        if (PlaceNow)
        {
            PlaceNow = false;

            Place();
        }
    }

    private void Place()
    {
        for (int i = parent.transform.childCount - 1; i >= 0; --i)
        {
            DestroyImmediate(parent.transform.GetChild(i).gameObject);
        }

        var candidates = new List<int>();

        for (int i = 0; i < othersParent.childCount; ++i)
        {
            candidates.Clear();
            var other = othersParent.GetChild(i);

            var angle = other.CalculateAxeUpAngle(Vector3.up);

            Debug.Log(angle, other);

            var rot1 = other.transform.rotation;

            for (int j = 0; j < angleLimits.Length - 1; ++j)
            {
                var min = Mathf.Min(angleLimits[j], angleLimits[j + 1]);
                var max = Mathf.Max(angleLimits[j], angleLimits[j + 1]);

                if (angle >= min - angleLimitOverlap
                    && angle <= max + angleLimitOverlap)
                {
                    candidates.Add(j);
                }

                if (angle + 360 >= min - angleLimitOverlap
                    && angle + 360 <= max + angleLimitOverlap)
                {
                    candidates.Add(j);
                }
            }

            if (candidates.Count == 0) continue;

            var index = candidates[Random.Range(0, candidates.Count)];
            
            var targetGo = target[index];

            //var go = targetGo.InstantiatePrefab();
            var go = Instantiate(targetGo);

            var pos = other.transform.position + other.transform.rotation * otherOffset;

            pos += new Vector3(
                            Rnd.BellCurve() * randomPosition.x,
                            Rnd.BellCurve() * randomPosition.y,
                            Rnd.BellCurve() * randomPosition.z);

            go.transform.SetParent(parent.transform, true);
            go.transform.position = pos;
            go.transform.rotation = targetGo.transform.rotation;

            if (useElementScale)
            {
                go.transform.localScale = targetGo.transform.lossyScale;
            }
            else
            {
                go.transform.localScale *= elementSize;
            }

            var rot = new Vector3(
                    Random.Range(-randomRotation.x, randomRotation.x),
                    Random.Range(-randomRotation.y, randomRotation.y),
                    Random.Range(-randomRotation.z, randomRotation.z));

            go.transform.localRotation *= Quaternion.Euler(rot);
        }
    }
}
