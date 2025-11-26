using Ouroboros.Common.Utils.UIElements;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ouroboros.Common.UI.Platform
{
    [CustomEditor(typeof(UIPlatformBase))]
    [CanEditMultipleObjects]
    public class UIPlatformBaseEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            root.Add(new IMGUIContainer(() => DrawDefaultInspector()));
            var ui = target as UIPlatformBase;

            var buttons = root.AddRow();
            buttons.AddButton("Save Mobile", () =>
            {
                ui.SaveMobile();
                EditorUtility.SetDirty(target);
            });

            buttons.AddButton("Save Tablet", () =>
            {
                ui.SaveTablet();
                EditorUtility.SetDirty(target);
            });

            buttons.AddButton("Save Standalone", () =>
            {
                ui.SaveStandalone();
                EditorUtility.SetDirty(target);
            });

            buttons = root.AddRow();
            buttons.AddButton("Set Mobile", () =>
            {
                ui.SetMobile();
                EditorUtility.SetDirty(target);
            });

            buttons.AddButton("Set Tabled", () =>
            {
                ui.SetTablet();
                EditorUtility.SetDirty(target);
            });

            buttons.AddButton("Set Standalone", () =>
            {
                ui.SetStandalone();
                EditorUtility.SetDirty(target);
            });

            return root;
        }
    }
}