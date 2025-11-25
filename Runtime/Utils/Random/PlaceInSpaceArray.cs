using UnityEngine;

[ExecuteInEditMode]
public class PlaceInSpaceArray : MonoBehaviour
{
    public GameObject[] target = new GameObject[0];
    public GameObject parent;
    public float elementSize = 1f;
    public bool useElementScale;

    public Vector3 arrayLength;
    public bool useArrayLength;
    public Vector3 arrayCount;
    public Vector3 arrayDistance;
    public Vector3 randomPosition;
    public Vector3 randomRotation;
    public Vector3 excludeCenterSize;

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

        var distance = arrayDistance;
        if (useArrayLength)
        {
            distance = new Vector3(
                arrayLength.x / arrayCount.x,
                arrayLength.y / arrayCount.y,
                arrayLength.z / arrayCount.z);
        }

        int fromX = (int)arrayCount.x / 2 - (int)excludeCenterSize.x / 2;
        int fromY = (int)arrayCount.y / 2 - (int)excludeCenterSize.y / 2;
        int fromZ = (int)arrayCount.z / 2 - (int)excludeCenterSize.z / 2;

        int toX = fromX + (int)excludeCenterSize.x;
        int toY = fromY + (int)excludeCenterSize.y;
        int toZ = fromZ + (int)excludeCenterSize.z;

        var exclude = excludeCenterSize.x > 0 && excludeCenterSize.y > 0 && excludeCenterSize.z > 0;

        for (int x = 0; x < arrayCount.x; ++x)
        {
            for (int y = 0; y < arrayCount.y; ++y)
            {
                for (int z = 0; z < arrayCount.z; ++z)
                {
                    //if ((Mathf.Abs(z - arrayCount.z / 2) < Mathf.CeilToInt(excludeCenterSize.z / 2))
                    //    && (Mathf.Abs(x - arrayCount.x / 2) < Mathf.CeilToInt(excludeCenterSize.x / 2))
                    //    && (Mathf.Abs(y - arrayCount.y / 2) < Mathf.CeilToInt(excludeCenterSize.y / 2)))
                    //{
                    //    continue;
                    //}

                    if (exclude
                        && x >= fromX && x < toX
                        && y >= fromY && y < toY
                        && z >= fromZ && z < toZ)
                    {
                        continue;
                    }
                    //Debug.LogFormat("{0}, {1}, {2}", x, y, z);

                    var targetGo = target[Random.Range(0, target.Length)];

                    //var go = targetGo.InstantiatePrefab();
                    var go = GameObject.Instantiate(targetGo);

                    var pos = new Vector3(x * distance.x, y * distance.y, z * distance.z);

                    /*
                    pos += new Vector3(
                            Random.Range(-randomPosition.x, randomPosition.x),
                            Random.Range(-randomPosition.y, randomPosition.y),
                            Random.Range(-randomPosition.z, randomPosition.z));*/

                    pos += new Vector3(
                            Rnd.BellCurve() * randomPosition.x,
                            Rnd.BellCurve() * randomPosition.y,
                            Rnd.BellCurve() * randomPosition.z);

                    go.transform.SetParent(parent.transform, true);
                    go.transform.rotation = targetGo.transform.rotation;
                    go.transform.position = targetGo.transform.position + pos;

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
                    //go.transform.localScale = go.transform.localScale * Random.Range(0.8f, 1.2f);

                }
            }
        }
    }
}
