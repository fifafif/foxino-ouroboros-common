using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace Ouroboros.Common.Utils.Debugs
{
#if UNITY_EDITOR

    [InitializeOnLoad]

#endif

    public static class DebugDrawer
    {
        private class DebugTextEntry
        {
            public Vector3 position;
            public string message;
            public Color color;
            public float endTime;
        }

        private static readonly List<DebugTextEntry> messages = new();

#if UNITY_EDITOR

        static DebugDrawer()
        {
            SceneView.duringSceneGui += OnSceneGUI;
            EditorApplication.playModeStateChanged += OnPlayModeChaged;
        }

        private static void OnPlayModeChaged(PlayModeStateChange change)
        {
            messages.Clear();
        }

#endif

        [Conditional("UNITY_EDITOR")]
        public static void DrawText(Vector3 position, string message, Color? color = null, float duration = 1f)
        {
            messages.Add(new DebugTextEntry
            {
                position = position,
                message = message,
                color = color ?? Color.white,
                endTime = Time.time + duration
            });

#if UNITY_EDITOR
            SceneView.RepaintAll(); // Force update
#endif
        }

#if UNITY_EDITOR
        private static void OnSceneGUI(SceneView sceneView)
        {
            Handles.BeginGUI();
            float currentTime = Time.time;

            messages.RemoveAll(m => m.endTime < currentTime);

            foreach (var msg in messages)
            {
                Vector3 worldPos = msg.position;
                Vector2 screenPos = HandleUtility.WorldToGUIPoint(worldPos);

                GUIStyle style = new GUIStyle(EditorStyles.label)
                {
                    normal = { textColor = msg.color },
                    fontStyle = FontStyle.Bold
                };

                GUI.Label(new Rect(screenPos.x, screenPos.y, 200, 20), msg.message, style);
            }

            Handles.EndGUI();
        }
#endif

        [Conditional("UNITY_EDITOR")]
        public static void DrawWireCube(Vector3 center, float size, Color color, float duration = 0)
        {
            DrawWireCube(center, new Vector3(size, size, size), color, duration);
        }

        [Conditional("UNITY_EDITOR")]
        public static void DrawWireCube(Vector3 center, Vector3 size, Color color, float duration = 0)
        {
            Vector3 half = size * 0.5f;

            // 8 corners of the cube
            Vector3[] corners = new Vector3[8]
            {
                center + new Vector3(-half.x, -half.y, -half.z),
                center + new Vector3( half.x, -half.y, -half.z),
                center + new Vector3( half.x, -half.y,  half.z),
                center + new Vector3(-half.x, -half.y,  half.z),

                center + new Vector3(-half.x,  half.y, -half.z),
                center + new Vector3( half.x,  half.y, -half.z),
                center + new Vector3( half.x,  half.y,  half.z),
                center + new Vector3(-half.x,  half.y,  half.z)
            };

            // Bottom square
            Debug.DrawLine(corners[0], corners[1], color, duration);
            Debug.DrawLine(corners[1], corners[2], color, duration);
            Debug.DrawLine(corners[2], corners[3], color, duration);
            Debug.DrawLine(corners[3], corners[0], color, duration);

            // Top square
            Debug.DrawLine(corners[4], corners[5], color, duration);
            Debug.DrawLine(corners[5], corners[6], color, duration);
            Debug.DrawLine(corners[6], corners[7], color, duration);
            Debug.DrawLine(corners[7], corners[4], color, duration);

            // Vertical lines
            Debug.DrawLine(corners[0], corners[4], color, duration);
            Debug.DrawLine(corners[1], corners[5], color, duration);
            Debug.DrawLine(corners[2], corners[6], color, duration);
            Debug.DrawLine(corners[3], corners[7], color, duration);
        }

        [Conditional("UNITY_EDITOR")]
        public static void DrawWireSphere(Vector3 center, float radius, Color color, float duration = 0f, int segments = 12)
        {
            float deltaTheta = (2f * Mathf.PI) / segments;
            float theta = 0f;

            // Draw three rings in XY, YZ, and XZ planes
            for (int i = 0; i < segments; i++)
            {
                Vector3 p1_xy = new Vector3(Mathf.Cos(theta), Mathf.Sin(theta), 0) * radius + center;
                Vector3 p2_xy = new Vector3(Mathf.Cos(theta + deltaTheta), Mathf.Sin(theta + deltaTheta), 0) * radius + center;

                Vector3 p1_xz = new Vector3(Mathf.Cos(theta), 0, Mathf.Sin(theta)) * radius + center;
                Vector3 p2_xz = new Vector3(Mathf.Cos(theta + deltaTheta), 0, Mathf.Sin(theta + deltaTheta)) * radius + center;

                Vector3 p1_yz = new Vector3(0, Mathf.Cos(theta), Mathf.Sin(theta)) * radius + center;
                Vector3 p2_yz = new Vector3(0, Mathf.Cos(theta + deltaTheta), Mathf.Sin(theta + deltaTheta)) * radius + center;

                Debug.DrawLine(p1_xy, p2_xy, color, duration);
                Debug.DrawLine(p1_xz, p2_xz, color, duration);
                Debug.DrawLine(p1_yz, p2_yz, color, duration);

                theta += deltaTheta;
            }
        }

        [Conditional("UNITY_EDITOR")]
        public static void DrawLineDirection(Vector3 position, Vector3 direction, Color color, float duration = 0f)
        {
            Debug.DrawLine(position, position + direction, color, duration);
        }

        [Conditional("UNITY_EDITOR")]
        public static void DrawCollision(Collision collision, Rigidbody selfBody, Color color, float sizeFactor = 1f, float duration = 0f)
        {
            DrawCollision(collision, color, sizeFactor, duration);

            var position = selfBody.position;
            DrawWireSphere(position, sizeFactor * 0.5f, color, duration);
            Debug.DrawLine(position, position + collision.impulse * sizeFactor, new Color(1f, 0f, 1f), duration);
            Debug.DrawLine(position, position + collision.relativeVelocity * sizeFactor, Color.green, duration);
        }

        [Conditional("UNITY_EDITOR")]
        public static void DrawCollision(Collision collision, Color color, float sizeFactor = 1f, float duration = 0f)
        {
            for (int i = 0; i < collision.contacts.Length; i++)
            {
                var contact = collision.contacts[i];
                DrawText(contact.point, $"p{i}", color, duration);
                DrawWireCube(contact.point, sizeFactor * 0.3f, color, duration);
                Debug.DrawLine(contact.point, contact.point + contact.normal * sizeFactor, Color.cyan, duration);
                Debug.DrawLine(contact.point, contact.point + contact.impulse * sizeFactor, color, duration);
            }
        }
    }
}