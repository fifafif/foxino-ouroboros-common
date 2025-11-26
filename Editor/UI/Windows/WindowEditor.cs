using Ouroboros.Common.Utils.UIElements;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ouroboros.Common.UI.Windows
{
    [CustomEditor(typeof(Window), true)]
    public class WindowEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            root.Add(new IMGUIContainer(() => DrawDefaultInspector()));

            root.AddButton("Close Others", () =>
            {
                var tab = target as Window;
                foreach (Transform child in tab.transform.parent)
                {
                    child.gameObject.SetActive(child == tab.transform);
                }
            });

            return root;
        }
    }
}