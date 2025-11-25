using Ouroboros.Common.Utils;
using System;
using UnityEngine;

public class AreaConstrainDestructor : MonoBehaviour
{
    public Action OnBeginDestroy { get; set; }

    public Vector3 Area;
    public Vector3 AreaMin;
    public Vector3 AreaMax;
    public bool IsKeepingAlive;

    public void Init(Vector3 area)
    {
        Area = area;
    }

    private void Update()
    {
        if (Area.ContainsPointAbs(transform.position)) return;
        //if (AreaMin.ContainsPoint(AreaMax, transform.position)) return;

        Destroy();
    }

    private void Destroy()
    {
        OnBeginDestroy?.Invoke();

        if (!IsKeepingAlive)
        {
            Destroy(gameObject);
        }
    }
}
