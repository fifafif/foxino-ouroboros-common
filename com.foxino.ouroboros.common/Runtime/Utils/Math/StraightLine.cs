using UnityEngine;

namespace Ouroboros.Common.Utils.Math
{
    public struct StraightLine
    {
        public Vector3 P1;
        public Vector3 P2;

        public float CalculateDistance(Vector3 point)
        {
            var closest = FindClosestOnLine(point);

            return Vector3.Distance(closest, point);
        }

        public Vector3 FindClosestOnLine(Vector3 point)
        {
            var  dir = P2 - P1;
            var length = dir.magnitude;
            dir.Normalize();
            var projectLength = Mathf.Clamp(Vector3.Dot(point - P1, dir), 0f, length);

            return P1 + dir * projectLength;
        }
    }
}