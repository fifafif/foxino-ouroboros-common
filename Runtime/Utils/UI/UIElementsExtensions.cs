using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ouroboros.Common.Utils.UIElements
{
    public static class UIElementsExtensions
    {
        public static TextField AddTextField(
            this VisualElement element,
            string label,
            string value,
            Action<TextField, ChangeEvent<string>> onChange)
        {
            var field = new TextField(label);
            field.value = value;
            field.RegisterValueChangedCallback(v =>
            {
                onChange?.Invoke(field, v);
            });

            element.Add(field);

            return field;
        }

        public static TextField AddTextField(
            this VisualElement element,
            string label,
            string value,
            Action<ChangeEvent<string>> onChange)
        {
            var field = new TextField(label);
            field.value = value;
            field.RegisterValueChangedCallback(v =>
            {
                onChange?.Invoke(v);
            });

            element.Add(field);

            return field;
        }

        public static Toggle AddCheckbox(
            this VisualElement element,
            string label,
            bool value,
            Action<ChangeEvent<bool>> onChange)
        {
            var field = new Toggle(label);
            field.value = value;
            field.RegisterValueChangedCallback(v =>
            {
                onChange?.Invoke(v);
            });

            element.Add(field);

            return field;
        }

        public static Button AddButton(this VisualElement element, string label, Action onClick)
        {
            var button = new Button(onClick);
            button.text = label;
            element.Add(button);

            return button;
        }

        public static Button AddButton(
            this VisualElement element, string label, Action onClick, Color color)
        {
            var button = element.AddButton(label, onClick);
            button.style.backgroundColor = color;

            return button;
        }

        public static VisualElement AddRow(this VisualElement element)
        {
            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            element.Add(row);

            return row;
        }

        public static void AddHeader(this VisualElement element, string label)
        {
            var header = new Label(label);
            header.style.unityFontStyleAndWeight = FontStyle.Bold;
            header.style.paddingTop = 10;
            element.Add(header);
        }

        public static void AddSpace(this VisualElement element, float height = 10f)
        {
            var space = new VisualElement();
            space.style.height = height;
            element.Add(space);
        }
    }
}