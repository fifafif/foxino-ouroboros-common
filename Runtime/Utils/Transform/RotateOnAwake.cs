using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateOnAwake : MonoBehaviour
{
    public Vector3 Rotation;
    public bool IsDisabled;

    private void Awake()
    {
        if (IsDisabled) return;

        transform.Rotate(Rotation);
    }
}
