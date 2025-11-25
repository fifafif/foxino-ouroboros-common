using UnityEngine;
using Ouroboros.Common.Utils.Transforms;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class PlaceArraySegments : MonoBehaviour
{
    public int Count;
    public Vector3 RandomRotationAngle;
    public Transform Parent;
    public ArraySegmentTransform Segment;

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
        #if UNITY_EDITOR

        if (Parent == null)
        {
            Parent = transform;
        }

        Parent.DestroyImmediateAllChildren();

        ArraySegmentTransform lastSegment = null;

        for (int i = 0; i < Count; ++i)
        {
            GameObject go;
            #if UNITY_EDITOR
            if (PrefabUtility.GetPrefabType(Segment.gameObject) == PrefabType.PrefabInstance)
            {
                go = PrefabUtility.InstantiatePrefab(PrefabUtility.GetCorrespondingObjectFromSource(Segment.gameObject)) as GameObject;
            }
            else
            #endif
            {
                go = Instantiate(Segment.gameObject) as GameObject;
            }

            go.transform.SetParent(Parent, true);

            var segment = go.GetComponent<ArraySegmentTransform>();

            if (lastSegment != null)
            {
                segment.transform.MatchOther(lastSegment.EndPoint);
            }
            else
            {
                segment.transform.Reset();
            }

            lastSegment = segment;

            var rot = Quaternion.Euler(Rnd.RandomVector3(RandomRotationAngle));
            segment.transform.localRotation *= rot;

            if (segment.StartPoint != null)
            {
                segment.transform.localPosition += segment.StartPoint.localPosition;
                segment.transform.localRotation = Quaternion.Inverse(segment.StartPoint.localRotation);
            }
        }

        #endif
    }
}
