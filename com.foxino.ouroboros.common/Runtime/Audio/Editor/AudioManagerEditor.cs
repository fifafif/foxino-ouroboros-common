using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ouroboros.Common.Audio
{
    [CustomEditor(typeof(AudioManager))]
    public class AudioManagerEditor : Editor
    {
        // Base volume sliders (inspector values)
        private Slider baseMasterVolumeSlider;
        private Slider baseMusicVolumeSlider;
        private Slider baseSfxVolumeSlider;
        private Slider baseVoiceVolumeSlider;

        // Runtime volume sliders
        private Slider runtimeMasterVolumeSlider;
        private Slider runtimeMusicVolumeSlider;
        private Slider runtimeSfxVolumeSlider;
        private Slider runtimeVoiceVolumeSlider;

        // Runtime enabled toggles
        private Toggle musicEnabledToggle;
        private Toggle sfxEnabledToggle;

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            // Add default inspector for other fields (databases, mixer, etc.)
            InspectorElement.FillDefaultInspector(root, serializedObject, this);

            // ========== BASE VOLUME SECTION ==========
            var baseVolumeSection = CreateSection("Base Volumes (Inspector Defaults)", new Color(0.3f, 0.5f, 0.7f, 0.3f));

            var baseHelpBox = new HelpBox("These are the default volumes set in the inspector. They are preserved and used as base values for calculations.", HelpBoxMessageType.Info);
            baseVolumeSection.Add(baseHelpBox);

            // Base Master Volume
            baseMasterVolumeSlider = CreateVolumeSlider("Master Volume",
                serializedObject.FindProperty("MasterVolume"),
                null);
            baseVolumeSection.Add(baseMasterVolumeSlider);

            // Base Music Volume
            baseMusicVolumeSlider = CreateVolumeSlider("Music Volume",
                serializedObject.FindProperty("MusicVolume"),
                null);
            baseVolumeSection.Add(baseMusicVolumeSlider);

            // Base SFX Volume
            baseSfxVolumeSlider = CreateVolumeSlider("SFX Volume",
                serializedObject.FindProperty("SfxVolume"),
                null);
            baseVolumeSection.Add(baseSfxVolumeSlider);

            // Base Voice Volume
            baseVoiceVolumeSlider = CreateVolumeSlider("Voice Volume",
                serializedObject.FindProperty("VoiceVolume"),
                null);
            baseVolumeSection.Add(baseVoiceVolumeSlider);

            root.Add(baseVolumeSection);

            var clearButton = new Button(() =>
            {
                AudioManager.ClearAllSettings();
            });

            clearButton.text = "Clear All Settings";
            root.Add(clearButton);

            // ========== RUNTIME VOLUME SECTION (Play Mode Only) ==========
            if (Application.isPlaying)
            {
                var runtimeVolumeSection = CreateSection("Runtime Controls (Play Mode)", new Color(0.7f, 0.5f, 0.3f, 0.3f));

                var runtimeHelpBox = new HelpBox("Adjust user volume multipliers and enabled states. These are saved to PlayerPrefs.", HelpBoxMessageType.Info);
                runtimeVolumeSection.Add(runtimeHelpBox);

                // Music Enabled Toggle
                musicEnabledToggle = new Toggle("Music Enabled");
                musicEnabledToggle.style.marginBottom = 5;
                if (AudioManager.instance != null)
                {
                    musicEnabledToggle.value = AudioManager.instance.IsMusicEnabled;
                }
                musicEnabledToggle.RegisterValueChangedCallback(evt =>
                {
                    if (AudioManager.instance != null)
                    {
                        AudioManager.SetMusicEnabled(evt.newValue);
                    }
                });
                runtimeVolumeSection.Add(musicEnabledToggle);

                // Runtime Music Volume Multiplier
                runtimeMusicVolumeSlider = CreateRuntimeSlider("Music Volume Multiplier",
                    () => AudioManager.instance != null ? AudioManager.instance.MusicVolume / Mathf.Max(0.0001f, serializedObject.FindProperty("MusicVolume").floatValue) : 1f,
                    (value) => AudioManager.SetMusicVolume(value));
                runtimeVolumeSection.Add(runtimeMusicVolumeSlider);

                // SFX Enabled Toggle
                sfxEnabledToggle = new Toggle("SFX Enabled");
                sfxEnabledToggle.style.marginBottom = 5;
                sfxEnabledToggle.style.marginTop = 10;
                if (AudioManager.instance != null)
                {
                    sfxEnabledToggle.value = AudioManager.instance.IsSFXEnabled;
                }
                sfxEnabledToggle.RegisterValueChangedCallback(evt =>
                {
                    if (AudioManager.instance != null)
                    {
                        AudioManager.SetSFXEnabled(evt.newValue);
                    }
                });
                runtimeVolumeSection.Add(sfxEnabledToggle);

                // Runtime SFX Volume Multiplier
                runtimeSfxVolumeSlider = CreateRuntimeSlider("SFX Volume Multiplier",
                    () => AudioManager.instance != null ? AudioManager.instance.SfxVolume / Mathf.Max(0.0001f, serializedObject.FindProperty("SfxVolume").floatValue) : 1f,
                    (value) => AudioManager.SetSFXVolume(value));
                runtimeVolumeSection.Add(runtimeSfxVolumeSlider);

                // Runtime Voice Volume
                runtimeVoiceVolumeSlider = CreateRuntimeSlider("Voice Volume",
                    () => AudioManager.instance != null ? AudioManager.instance.VoiceVolume : 1f,
                    (value) => AudioManager.SetVoiceVolume(value));
                runtimeVolumeSection.Add(runtimeVoiceVolumeSlider);

                root.Add(runtimeVolumeSection);

                // Schedule updates for runtime values
                root.schedule.Execute(() => UpdateRuntimeValues()).Every(100);
            }

            return root;
        }

        private VisualElement CreateSection(string title, Color borderColor)
        {
            var section = new VisualElement();
            section.style.marginTop = 10;
            section.style.marginBottom = 10;
            section.style.paddingLeft = 5;
            section.style.paddingRight = 5;
            section.style.paddingTop = 8;
            section.style.paddingBottom = 8;
            section.style.borderTopWidth = 2;
            section.style.borderBottomWidth = 2;
            section.style.borderLeftWidth = 2;
            section.style.borderRightWidth = 2;
            section.style.borderTopColor = borderColor;
            section.style.borderBottomColor = borderColor;
            section.style.borderLeftColor = borderColor;
            section.style.borderRightColor = borderColor;
            section.style.backgroundColor = new Color(borderColor.r, borderColor.g, borderColor.b, 0.1f);

            var label = new Label(title);
            label.style.unityFontStyleAndWeight = FontStyle.Bold;
            label.style.fontSize = 13;
            label.style.marginBottom = 8;
            section.Add(label);

            return section;
        }

        private void UpdateRuntimeValues()
        {
            if (AudioManager.instance == null || !Application.isPlaying) return;

            // Update toggles
            if (musicEnabledToggle != null)
            {
                musicEnabledToggle.SetValueWithoutNotify(AudioManager.instance.IsMusicEnabled);
            }
            if (sfxEnabledToggle != null)
            {
                sfxEnabledToggle.SetValueWithoutNotify(AudioManager.instance.IsSFXEnabled);
            }

            // Update runtime sliders with current multiplier values
            var baseMusicVolume = serializedObject.FindProperty("MusicVolume").floatValue;
            var baseSfxVolume = serializedObject.FindProperty("SfxVolume").floatValue;

            if (runtimeMusicVolumeSlider != null && baseMusicVolume > 0.0001f)
            {
                float multiplier = AudioManager.instance.MusicVolume / baseMusicVolume;
                if (AudioManager.instance.IsMusicEnabled && multiplier > 0)
                {
                    runtimeMusicVolumeSlider.SetValueWithoutNotify(multiplier);
                }
            }

            if (runtimeSfxVolumeSlider != null && baseSfxVolume > 0.0001f)
            {
                float multiplier = AudioManager.instance.SfxVolume / baseSfxVolume;
                if (AudioManager.instance.IsSFXEnabled && multiplier > 0)
                {
                    runtimeSfxVolumeSlider.SetValueWithoutNotify(multiplier);
                }
            }

            if (runtimeVoiceVolumeSlider != null)
            {
                runtimeVoiceVolumeSlider.SetValueWithoutNotify(AudioManager.instance.VoiceVolume);
            }
        }

        private Slider CreateVolumeSlider(string label, SerializedProperty property, System.Action<float> onValueChanged)
        {
            var slider = new Slider(label, 0f, 1f);
            slider.showInputField = true;
            slider.value = property.floatValue;

            slider.RegisterValueChangedCallback(evt =>
            {
                property.floatValue = evt.newValue;
                serializedObject.ApplyModifiedProperties();
                onValueChanged?.Invoke(evt.newValue);
            });

            // Update slider when property changes externally
            slider.TrackPropertyValue(property, prop =>
            {
                slider.SetValueWithoutNotify(prop.floatValue);
            });

            return slider;
        }

        private Slider CreateRuntimeSlider(string label, System.Func<float> getCurrentValue, System.Action<float> onValueChanged)
        {
            var slider = new Slider(label, 0f, 1f);
            slider.showInputField = true;

            if (getCurrentValue != null)
            {
                slider.value = getCurrentValue();
            }

            slider.RegisterValueChangedCallback(evt =>
            {
                onValueChanged?.Invoke(evt.newValue);
            });

            return slider;
        }
    }
}
