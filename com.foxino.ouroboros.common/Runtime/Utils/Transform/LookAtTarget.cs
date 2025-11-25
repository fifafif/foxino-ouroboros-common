using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LookAtTarget : MonoBehaviour
{
    public Transform target;
    public Vector3 rotOffset;

    // Editor only
    public bool updateInEditor = false;
    public bool updateNow;

    void LateUpdate ()
    {
        if (Application.isPlaying
            || updateInEditor)
        {
            Look();
        }

        if (updateNow)
        {
            updateNow = false;

            Look();
        }
	}

    private void Look()
    {
        if (target != null)
        {
            transform.LookAt(target);
            transform.Rotate(rotOffset);
        }
    }
}
