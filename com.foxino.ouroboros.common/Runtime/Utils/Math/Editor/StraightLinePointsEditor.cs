using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Ouroboros.Common.Utils.UIElements;

namespace Ouroboros.Common.Utils.Math
{
    [CustomEditor(typeof(StraightLinePoints))]
    public class StraightLinePointsEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var element = new VisualElement();

            element.Add(new IMGUIContainer(() => DrawDefaultInspector()));

            element.AddButton("Collect Points", CollectPoints);
            
            return element;
        }

        private void CollectPoints()
        {
            var line = target as StraightLinePoints;
            line.Clear();

            foreach (Transform child in line.transform)
            {
                line.AddPoint(child);
            }

            line.UpdateLine();

        }
    }
}