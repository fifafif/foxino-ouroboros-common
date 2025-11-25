using UnityEngine;

namespace Ouroboros.Common.Utils.Math
{
    public static class MathUtils
    {
        public static Vector3 Vector3Epsilon => new Vector3(float.Epsilon, float.Epsilon, float.Epsilon);
        public const float MsToKmh = 3.6f;
        public const float KmhToMs = 1 / 3.6f;

        public static Quaternion LookRotation2D(Vector2 direction)
        {
            var rotatedToTarget = Quaternion.Euler(0, 0, 90) * direction;
            return Quaternion.LookRotation(Vector3.forward, rotatedToTarget);
        }

        public static float ApproximateBellCurve(float x)
        {
            x = Mathf.Clamp01(x);

            return 1.0f - Mathf.Pow(2.0f * (x - 0.5f), 2.0f);
        }
    }
}