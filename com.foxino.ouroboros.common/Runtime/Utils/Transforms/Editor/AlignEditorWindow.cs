using Ouroboros.Common.Utils.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ouroboros.Common.Utils.Transforms
{
    public class AlignEditorWindow : EditorWindow
    {
        private ObjectField targetField;
        private FloatField radiusField;
        private Transform target;
        private float radius;

        private Transform Target 
        {
            get 
            {
                if (target == null)
                {
                    return Selection.activeTransform;
                }

                return target;
            }
        }

        [MenuItem("Ouroboros/Utils/Align")]
        public static void ShowWindow()
        {
            var window = GetWindow<AlignEditorWindow>();
            window.titleContent = new GUIContent("Align");

            window.minSize = new Vector2(250, 50);
        }

        private void OnEnable()
        {
            CreateGUI();
        }

        private void CreateGUI()
        {
            var root = rootVisualElement;
            root.Clear();

            targetField = root.AddObjectField<Transform>("Target", f =>
            {
                target = f;
            });

            root.Add(targetField);

            radius = 0.5f;
            radiusField = root.AddFloatField("Radius", radius, f => 
            {
                radius = f.newValue;
            });

            root.Add(radiusField);

            var buttons = new VisualElement();
            buttons.style.flexDirection = FlexDirection.Row;
            root.Add(buttons);

            buttons.AddButton("Look At 0,0,0", LookAtCenter);
            buttons.AddButton("Spherify", Spherify);
            
            root.Add(buttons);
        }

        private void Spherify()
        {
            var dir = Target.position - GetCenter();
            if (dir == Vector3.zero)
            {
                dir = Vector3.forward;
            }

            Target.position = GetCenter() + dir.normalized * radius;
            LookAtCenter();
        }

        private void LookAtCenter()
        {
            Target.LookAt(GetCenter());
            EditorUtility.SetDirty(Target);
        }

        private Vector3 GetCenter()
        {
            return Vector3.zero;
        }
    }
}