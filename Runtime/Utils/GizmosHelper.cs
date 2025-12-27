using UnityEngine;

namespace Ouroboros.Common.Utils
{
    public static class GizmosHelper
    {
        private static Color origGizmosColor;
        private static Matrix4x4 origGizmosMatrix;

        public static void DrawWireCube(Vector3 position, float size, Color color)
        {
            DrawWireCube(position, new Vector3(size, size, size), color);
        }

        public static void DrawWireCube(Vector3 position, Vector3 size, Color color)
        {
            var origColor = Gizmos.color;
            Gizmos.color = color;
            Gizmos.DrawWireCube(position, size);
            Gizmos.color = origColor;
        }

        public static void DrawWireSphere(Vector3 position, float radius, Color color)
        {
            var origColor = Gizmos.color;
            Gizmos.color = color;
            Gizmos.DrawWireSphere(position, radius);
            Gizmos.color = origColor;
        }

        public static void DrawLine(Vector3 from, Vector3 to, Color color)
        {
            var origColor = Gizmos.color;
            Gizmos.color = color;
            Gizmos.DrawLine(from, to);
            Gizmos.color = origColor;
        }

        public static void SetColor(Color color)
        {
            origGizmosColor = Gizmos.color;
            Gizmos.color = color;
        }

        public static void ResetColor()
        {
            Gizmos.color = origGizmosColor;
        }

        public static void SetMatrix(Matrix4x4 matrix)
        {
            origGizmosMatrix = Gizmos.matrix;
            Gizmos.matrix = matrix;
        }

        public static void ResetMatrix()
        {
            Gizmos.matrix = origGizmosMatrix;
        }

        public static void SetColorAndMatrix(Color color, Matrix4x4 matrix)
        {
            SetColor(color);
            SetMatrix(matrix);
        }

        public static void ResetColorAndMatrix()
        {
            ResetColor();
            ResetMatrix();
        }
    }
}