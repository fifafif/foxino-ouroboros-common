using Ouroboros.Common.Utils;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Ouroboros.Common.Audio
{
    [CustomPropertyDrawer(typeof(AudioClipAttribute))]
    public class AudioClipDrawer : ItemPickerPropertyDrawer
    {
        public override float DrawCustomButtons(Rect positionRect, string itemId)
        {
            if (IsOpen) return 0f;

            const float playButtonWidth = 20f;

            var rect = positionRect;
            positionRect.width -= playButtonWidth;

            rect = positionRect;
            rect.x = rect.xMax - playButtonWidth;
            rect.width = playButtonWidth;

            if (GUI.Button(rect, "▶"))
            {
                PlayAudio(itemId);
            }

            return playButtonWidth;
        }
        /*public override void OnGUI(
            Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(
                    position, 
                    label.text, 
                    "AudioClipAttribute can be used only with strings!");

                return;
            }

            const float playButtonWidth = 20f;

            var rect = position;
            rect.width -= playButtonWidth;

            EditorGUI.PropertyField(rect, property, label);

            rect = position;
            rect.x = position.xMax - playButtonWidth;
            rect.width = playButtonWidth;

            if (GUI.Button(rect, "▶"))
            {
                PlayAudio(property.stringValue);
            }
        }*/

        protected override Object FindItem(string itemId)
        {
            var databases = AssetFinder.FindAssetsByType<AudioDatabase>();

            var agent = databases
                .FirstOrDefault(d => d.Clips.FirstOrDefault(a => a.Id == itemId) != null);

            return agent;
        }

        protected override string[] GetItemIds()
        {
            var databases = AssetFinder.FindAssetsByType<AudioDatabase>();

            return databases.SelectMany(d => d.Clips.Select(c => c.Id)).ToArray();
        }

        private void PlayAudio(string audioId)
        {
            AudioEditorUtils.PlayClip(audioId);
        }
    }
}