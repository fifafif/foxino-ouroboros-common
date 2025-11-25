using UnityEditor;
using UnityEngine;

namespace Ouroboros.Common.Utils
{
    public class TimeScaleWindow : EditorWindow
    {
        private float timeScale = 1f;

        [MenuItem("Ouroboros/Utils/Time Scale")]
        public static void ShowWindow()
        {
            GetWindow(typeof(TimeScaleWindow));
        }

        void OnGUI()
        {
            timeScale = EditorGUILayout.Slider(timeScale, 0.1f, 5f);

            GUILayout.BeginHorizontal();

            DrawSpeedButton(0.1f);
            DrawSpeedButton(0.25f);
            DrawSpeedButton(0.5f);
            var color = GUI.color;
            GUI.color = Color.green;

            DrawSpeedButton(1f);
            
            GUI.color = color;

            DrawSpeedButton(2f);
            DrawSpeedButton(5f);
            DrawSpeedButton(10f);
            DrawSpeedButton(100f);

            GUILayout.EndHorizontal();

            Time.timeScale = timeScale;
        }

        private void DrawSpeedButton(float speed)
        {
            if (GUILayout.Button($"{speed}x"))
            {
                timeScale = speed;
            }
        }
    }
}