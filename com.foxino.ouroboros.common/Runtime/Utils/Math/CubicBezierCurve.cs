using UnityEngine;

namespace Ouroboros.Common.Utils.Math
{
    public static class CubicBezierCurve
    {
		public static Vector3 GetPoint(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float t)
		{
			var c = 1f - t;

			// The Bernstein polynomials.
			var bb0 = c * c * c;
			var bb1 = 3 * t * c * c;
			var bb2 = 3 * t * t * c;
			var bb3 = t * t * t;

			var point = p1 * bb0 + p2 * bb1 + p3 * bb2 + p4 * bb3;

			return point;
		}

		public static Vector3 GetTangent(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float t)
		{
			// See: http://bimixual.org/AnimationLibrary/beziertangents.html

			var q0 = p1 + ((p2 - p1) * t);
			var q1 = p2 + ((p3 - p2) * t);
			var q2 = p3 + ((p4 - p3) * t);

			var r0 = q0 + ((q1 - q0) * t);
			var r1 = q1 + ((q2 - q1) * t);
			var tangent = r1 - r0;

			return tangent;
		}
	}
}