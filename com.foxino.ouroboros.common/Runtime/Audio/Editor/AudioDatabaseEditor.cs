using Ouroboros.Common.Utils;
using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ouroboros.Common.Audio
{
    [CustomEditor(typeof(AudioDatabase))]
    public class AudioDatabaseEditor : Editor
    {
        private VisualElement root;

        private static readonly Color rowBgColor = new Color(0.175f, 0.175f, 0.175f, 1f);
        private static readonly Color borderColor = new Color(0.35f, 0.35f, 0.35f, 1f);
        private static readonly Color groupBorderColor = new Color(0.25f, 0.25f, 0.25f, 1f);
        private static readonly Color dangerColor = new Color(0.8f, 0f, 0f, 1f);
        private static readonly Color warningColor = new Color(0.8f, 0.5f, 0f, 1f);

        public override VisualElement CreateInspectorGUI()
        {
            root = new VisualElement();

            var database = target as AudioDatabase;

            CreateGUI(database);

            return root;
        }

        private void SaveAsset(AudioDatabase database)
        {
            EditorUtility.SetDirty(database);
        }

        private void CreateGUI(AudioDatabase database)
        {
            root.Clear();
            root.Add(CreateClipsGUI(database));
            root.Add(CreateButtons(database));
        }

        private VisualElement CreateButtons(AudioDatabase database)
        {
            var element = new VisualElement();
            element.style.flexDirection = FlexDirection.Row;

            var btn = new Button(() => AddClip(database, new AudioClipSingle()));
            btn.text = "Add Single";
            element.Add(btn);

            btn = new Button(() => AddClip(database, new AudioClipGroup()));
            btn.text = "Add Group";
            element.Add(btn);

            return element;
        }

        private VisualElement CreateClipsGUI(AudioDatabase database)
        {
            var scrollView = new ScrollView(ScrollViewMode.Vertical);

            for (var i = 0; i < database.Clips.Count; ++i)
            {
                scrollView.Add(CreateClipGUI(database.Clips[i], database));
            }

            return scrollView;
        }

        private void AddClip(AudioDatabase database, AudioClipBase data)
        {
            UnityEditor.Undo.RecordObject(database, "Add Clip to AudioDatabase");
            database.Clips.Add(data);
            SaveAsset(database);
            CreateGUI(database);
        }

        private void DeleteClip(AudioClipBase audioClipBase, AudioDatabase database)
        {
            database.Clips.Remove(audioClipBase);
            SaveAsset(database);
            CreateGUI(database);
        }

        private VisualElement CreateClipGUI(AudioClipBase audioClipBase, AudioDatabase database)
        {
            var element = new VisualElement();
            element.style.marginBottom = 5;
            element.style.backgroundColor = rowBgColor;
            element.SetBorder(borderColor, 1f);

            element.Add(CreateClipHeader(audioClipBase, database));

            switch (audioClipBase)
            {
                case AudioClipSingle single:
                    element.Add(CreateClipSingleGUI(single.AudioData, database));
                    break;

                case AudioClipGroup group:
                    element.Add(CreateClipGroupGUI(group, database));
                    break;

                default:
                    throw new Exception($"AudioClipBase type not supported! Type={audioClipBase.GetType()}");
            }

            return element;
        }

        private VisualElement CreateClipHeader(AudioClipBase audioClipBase, AudioDatabase database)
        {
            var element = new VisualElement();
            element.style.flexDirection = FlexDirection.Row;

            var idField = new TextField("Id");
            idField.value = audioClipBase.Id;
            idField.style.flexGrow = 1f;
            idField.RegisterValueChangedCallback(o =>
            {
                audioClipBase.Id = o.newValue;
                SaveAsset(database);
            });

            element.Add(idField);

            var deleteBtn = new Button(() => DeleteClip(audioClipBase, database));
            deleteBtn.text = "X";
            deleteBtn.style.width = 20f;
            deleteBtn.style.backgroundColor = dangerColor;

            element.Add(deleteBtn);

            return element;
        }

        private VisualElement CreateClipSingleGUI(AudioClipData single, AudioDatabase database)
        {
            var element = new VisualElement();

            var clipElement = new VisualElement();
            clipElement.style.flexDirection = FlexDirection.Row;
            element.Add(clipElement);

            var clipField = new ObjectField();
            clipField.label = "Audio Clip";
            clipField.objectType = typeof(AudioClip);
            clipField.allowSceneObjects = false;
            clipField.value = single.AudioClip;
            clipField.style.flexGrow = 1f;
            clipField.style.flexShrink = 1f;
            clipField.style.minWidth = 0; // Allow shrinking below content size
            clipField.style.overflow = Overflow.Hidden; // Clip overflow content
            clipField.RegisterValueChangedCallback(o =>
            {
                single.AudioClip = o.newValue as AudioClip;
                SaveAsset(database);
            });

            clipElement.Add(clipField);

            var playBtn = new Button(() => 
            {
                if (single.AudioClip == null) return;

                AudioEditorPlayer.StopClip(single.AudioClip);
                AudioEditorPlayer.PlayClip(single.AudioClip);
            });

            playBtn.text = "►";
            clipElement.Add(playBtn);
            
            var slider = new Slider("Volume", 0f, 1f);
            slider.value = single.Volume;
            slider.RegisterValueChangedCallback(o =>
            {
                single.Volume = o.newValue;
                SaveAsset(database);
            });

            element.Add(slider);
            
            return element;
        }

        private VisualElement CreateClipGroupGUI(AudioClipGroup group, AudioDatabase database)
        {
            var element = new VisualElement();

            var slider = new Slider("Volume", 0f, 1f);
            slider.value = group.Volume;
            slider.RegisterValueChangedCallback(o =>
            {
                group.Volume = o.newValue;
                SaveAsset(database);
            });

            element.Add(slider);
            foreach (var data in group.Clips)
            {
                element.Add(CreateGroupClipGUI(group, data, database));
            }

            var addBtn = new Button(() => AddClipToGroup(group, database));
            addBtn.text = "Add";
            element.Add(addBtn);

            return element;
        }
        private VisualElement CreateGroupClipGUI(AudioClipGroup group, AudioClipGroup.Clip data, AudioDatabase database)
        {
            var element = new VisualElement();
            element.style.borderTopColor = groupBorderColor;
            element.style.borderTopWidth = 1f;
            var header = new VisualElement();
            header.style.flexDirection = FlexDirection.Row;
            element.Add(header);

            var weightSlider = new Slider("Weight", 0f, 1f);
            weightSlider.value = data.Weight;
            weightSlider.style.flexGrow = 1f;
            weightSlider.RegisterValueChangedCallback(o =>
            {
                data.Weight = o.newValue;
                SaveAsset(database);
            });

            header.Add(weightSlider);

            var deleteBtn = new Button(() => DeleteClipFromGroup(group, data, database));
            deleteBtn.text = "X";
            deleteBtn.style.backgroundColor = warningColor;

            header.Add(deleteBtn);

            element.Add(CreateClipSingleGUI(data.AudioData, database));

            return element;
        }

        private void DeleteClipFromGroup(AudioClipGroup group, AudioClipGroup.Clip data, AudioDatabase database)
        {
            group.Clips.Remove(data);
            SaveAsset(database);
            CreateGUI(database);
        }

        private void AddClipToGroup(AudioClipGroup single, AudioDatabase database)
        {
            single.Clips.Add(new AudioClipGroup.Clip());
            SaveAsset(database);
            CreateGUI(database);
        }
    }
}