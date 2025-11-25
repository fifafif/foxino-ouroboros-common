using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Ouroboros.Common.Utils
{
    public abstract class ItemPickerPropertyDrawer : PropertyDrawer
    {
        private const float buttonWidth = 20f;

        protected bool IsOpen { get; private set; }

        public virtual float DrawCustomButtons(Rect positionRect, string itemId)
        {
            return 0f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(
                    position,
                    label.text,
                    "CrystalIdAttribute can be used only with strings!");

                return;
            }

            var rect = position;
            rect.x = position.xMax - buttonWidth * 2;
            rect.width = buttonWidth;

            if (GUI.Button(rect, IsOpen ? "▲" : "▼"))
            {
                IsOpen = !IsOpen;
            }

            var customButtonWidth = DrawCustomButtons(rect, property.stringValue);

            if (!IsOpen)
            {
                rect = position;
                rect.width -= buttonWidth * 2;
                rect.width -= customButtonWidth;

                EditorGUI.PropertyField(rect, property, label);
            }

            if (IsOpen)
            {
                DrawPopup(position, property, label, property.stringValue);
            }

            rect = position;
            rect.x = position.xMax - buttonWidth;
            rect.width = buttonWidth;

            if (GUI.Button(rect, "→"))
            {
                PingItem(FindItem(property.stringValue), property.stringValue);
            }
        }

        protected abstract string[] GetItemIds();

        protected abstract UnityEngine.Object FindItem(string itemId);

        private void DrawPopup(
            Rect position, SerializedProperty property, GUIContent label, string itemId)
        {
            var ids = GetItemIds();
            position.width -= buttonWidth * 2;

            var index = Array.FindIndex(ids, i => i == itemId);
            var newIndex = EditorGUI.Popup(position, label.text, index, ids);
            if (index != newIndex)
            {
                property.stringValue = ids[newIndex];
                IsOpen = false;
            }
        }

        private void PingItem(Object item, string itemId)
        {
            if (item == null)
            {
                Debug.LogError($"No item found! Id={itemId}");
                EditorUtility.DisplayDialog(
                    "Agent Not Found!", $"Item not found. Id={itemId}", "OK");

                return;
            }

            EditorGUIUtility.PingObject(item);
            Selection.activeObject = item;
        }
    }
}