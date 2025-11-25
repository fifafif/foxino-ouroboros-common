using UnityEngine;

[ExecuteInEditMode]
public class TransformProperties : MonoBehaviour
{
    public bool UpdateNow;

    void Update()
    {
        if (UpdateNow)
        {
            UpdateNow = false;

            Print();
        }
    }

    private void Print()
    {
        Debug.Log(TargetAxeUpAngle(transform, Vector3.up));
    }

    public static float TargetAxeUpAngle(Transform target, Vector3 axe)
    {
        var dir = target.up;
        dir.z = 0;

        var angle = Vector3.Angle(axe, dir);

        if (dir.x < 0)
        {
            angle = 360 - angle;
        }

        return angle;
    }
}
