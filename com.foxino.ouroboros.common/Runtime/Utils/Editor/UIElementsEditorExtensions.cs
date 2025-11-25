using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Ouroboros.Common.Utils
{
    public static class UIElementsEditorExtensions
    {
        public static void SetBorder(this VisualElement element, Color color, float width)
        {
            element.style.borderBottomColor = color;
            element.style.borderTopColor = color;
            element.style.borderLeftColor = color;
            element.style.borderRightColor = color;

            element.style.borderBottomWidth = width;
            element.style.borderTopWidth = width;
            element.style.borderLeftWidth = width;
            element.style.borderRightWidth = width;
        }

        public static ObjectField AddObjectField<T>(
            this VisualElement element, 
            string label, 
            Action<T> onChanged) 
            where T : UnityEngine.Object
        {
            var objectField = new ObjectField(label);
            objectField.allowSceneObjects = true;
            objectField.objectType = typeof(T);
            objectField.RegisterValueChangedCallback(c => onChanged?.Invoke(c.newValue as T));
            element.Add(objectField);

            return objectField;
        }

        public static FloatField AddFloatField(
            this VisualElement element,
            string label,
            float value,
            Action<ChangeEvent<float>> onChange)
        {
            var field = new FloatField(label);
            field.value = value;
            field.RegisterValueChangedCallback(v =>
            {
                onChange?.Invoke(v);
            });

            element.Add(field);

            return field;
        }
        
        public static IntegerField AddIntField(
            this VisualElement element,
            string label,
            int value,
            Action<ChangeEvent<int>> onChange)
        {
            var field = new IntegerField(label);
            field.value = value;
            field.RegisterValueChangedCallback(v =>
            {
                onChange?.Invoke(v);
            });

            element.Add(field);

            return field;
        }

        public static ObjectField AddObjectField<T>(
            this VisualElement element,
            string label,
            Object value,
            Action<T> onChange) where T : Object
        {
            var field = new ObjectField(label);
            field.value = value;
            field.objectType = typeof(T);
            field.RegisterValueChangedCallback(v =>
            {
                onChange?.Invoke(v.newValue as T);
            });

            element.Add(field);

            return field;
        }

        public static Toggle AddToggle(
            this VisualElement element,
            string label,
            bool value,
            Action<bool> onChange)
        {
            var field = new Toggle(label);
            field.value = value;
            field.RegisterValueChangedCallback(v =>
            {
                onChange?.Invoke(v.newValue);
            });

            element.Add(field);

            return field;
        }
        
        public static Toggle AddToggleLabelOnRight(
            this VisualElement element,
            string label,
            bool value,
            Action<bool> onChange)
        {
            var field = new Toggle();
            field.text = label;
            field.value = value;
            field.RegisterValueChangedCallback(v =>
            {
                onChange?.Invoke(v.newValue);
            });

            element.Add(field);

            return field;
        }

        public static DropdownField AddDropDown(
            this VisualElement element,
            string label,
            List<string> choices,
            int selected,
            Action<string> onChange)
        {
            var field = new DropdownField(label, choices, selected);
            field.RegisterValueChangedCallback(v =>
            {
                onChange?.Invoke(v.newValue);
            });

            element.Add(field);

            return field;
        }

        public static EnumField AddEnumField(
            this VisualElement element,
            string label,
            Enum enumValues,
            Action<Enum> onChange)
        {
            var field = new EnumField(label, enumValues);
            field.RegisterValueChangedCallback(v =>
            {
                onChange?.Invoke(v.newValue);
            });

            element.Add(field);

            return field;
        }

        public static void DrawDefaultInspector(this VisualElement element, Editor editor)
        {
            element.Add(new IMGUIContainer(() => editor.DrawDefaultInspector()));
        }
    }
}