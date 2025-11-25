using UnityEngine;

namespace Ouroboros.Common.Utils
{
    public static class Vector3Extensions
    {
        public static float DistanceSquared(this Vector3 vector3, Vector3 other)
        {
            return (vector3 - other).sqrMagnitude;
        }

        public static bool ContainsPoint(this Vector3 vector3, Vector3 point)
        {
            return point.x < vector3.x
                   && point.y < vector3.y
                   && point.z < vector3.z;
        }
        
        public static bool ContainsPointAbs(this Vector3 vector3, Vector3 point)
        {
            return vector3.Abs().ContainsPoint(point.Abs());
        }

        private static Vector3 Abs(this Vector3 vector3)
        {
            return new Vector3(
                Mathf.Abs(vector3.x), 
                Mathf.Abs(vector3.y), 
                Mathf.Abs(vector3.z));
        }

        public static bool IsPointPassTarget(
            this Vector3 point, 
            Vector3 direction, 
            Vector3 targetPosition, 
            float minDistancePassTarget = 0f)
        {
            var plane = new Plane(direction, point - direction * minDistancePassTarget);

            return !plane.GetSide(targetPosition);
        }

        public static Vector2 ToVector2(this Vector3 vector3)
        {
            return new Vector2(vector3.x, vector3.y);
        }

        
        public static Vector3 NormalizeXY(this Vector3 vector3)
        {
            vector3.z = 0f;
            return vector3.normalized;
        }
    }
}