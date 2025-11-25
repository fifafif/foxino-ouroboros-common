using UnityEditor;
using UnityEngine.UIElements;
using Ouroboros.Common.Utils.UIElements;

namespace Ouroboros.Common.Utils.Transforms
{
    [CustomEditor(typeof(CameraFacingRotator))]
    public class CameraFacingRotatorEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            root.Add(new IMGUIContainer(() => DrawDefaultInspector()));
            root.AddButton("Rotate", () => (target as CameraFacingRotator).Rotate());

            return root;
        }
    }
}