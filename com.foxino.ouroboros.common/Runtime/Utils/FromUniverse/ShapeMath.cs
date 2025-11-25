using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ShapeMath
{
    public static float TriangleSurface(Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 dot = Vector3.Cross(b - a, c - a);

        return dot.magnitude * 0.5f;
    }
}
