using UnityEngine;
using UnityEngine.UI;

namespace Ouroboros.Common.Audio.UI
{
    public class AudioManagerUIController : MonoBehaviour
    {
        [SerializeField] private Button musicButton;
        [SerializeField] private Button sfxButton;
        [SerializeField] private Button voiceButton;

        [Header("Optional Icons")]
        [SerializeField] private GameObject musicOnIcon;
        [SerializeField] private GameObject musicOffIcon;
        [SerializeField] private GameObject sfxOnIcon;
        [SerializeField] private GameObject sfxOffIcon;
        [SerializeField] private GameObject voiceOnIcon;
        [SerializeField] private GameObject voiceOffIcon;

        [Header("Colors")]
        [SerializeField] private Color disabledButtonColor = new Color(1f, 1f, 1f, 0.5f);

        private Color origMusicColor;
        private Color origSFXColor;
        private Color origVoiceColor;

        private void Awake()
        {
            origSFXColor = sfxButton.colors.normalColor;
            origMusicColor = musicButton.colors.normalColor;

            if (voiceButton != null)
            {
                origVoiceColor = voiceButton.colors.normalColor;
            }
        }

        private void Start()
        {
            SetupButtons();
            AudioManager.RegisterForOnLoadedEvent(UpdateButtonVisuals);
            AudioManager.RegisterOnMusicEnabled(OnMusicEnabled);
        }

        private void OnEnable()
        {
            UpdateButtonVisuals();
        }

        private void OnMusicEnabled(bool isEnabled)
        {
            UpdateButtonVisuals();
        }

        private void SetupButtons()
        {
            if (musicButton != null)
            {
                musicButton.onClick.AddListener(ToggleMusic);
            }

            if (sfxButton != null)
            {
                sfxButton.onClick.AddListener(ToggleSFX);
            }

            if (voiceButton != null)
            {
                voiceButton.onClick.AddListener(ToggleVoice);
            }
        }

        private void ToggleVoice()
        {
            if (AudioManager.instance == null) return;

            AudioManager.ToggleVoiceEnabled();

            UpdateButtonVisuals();
        }

        private void ToggleMusic()
        {
            if (AudioManager.instance == null) return;

            AudioManager.ToggleMusicEnabled();

            UpdateButtonVisuals();
        }

        private void ToggleSFX()
        {
            if (AudioManager.instance == null) return;

            AudioManager.ToggleSFXEnabled();

            UpdateButtonVisuals();
        }

        private void UpdateButtonVisuals()
        {
            if (AudioManager.instance == null) return;

            bool isMusicEnabled = AudioManager.instance.IsMusicEnabled;
            bool isSFXEnabled = AudioManager.instance.IsSFXEnabled;
            bool isVoiceEnabled = AudioManager.instance.IsVoiceEnabled;

            if (musicOnIcon != null)
                musicOnIcon.SetActive(isMusicEnabled);

            if (musicOffIcon != null)
                musicOffIcon.SetActive(!isMusicEnabled);

            if (sfxOnIcon != null)
                sfxOnIcon.SetActive(isSFXEnabled);

            if (sfxOffIcon != null)
                sfxOffIcon.SetActive(!isSFXEnabled);

            if (voiceOnIcon != null)
                voiceOnIcon.SetActive(isVoiceEnabled);

            if (voiceOffIcon != null)
                voiceOffIcon.SetActive(!isVoiceEnabled);

            UpdateButtonColor(musicButton, isMusicEnabled, origMusicColor);
            UpdateButtonColor(sfxButton, isSFXEnabled, origSFXColor);
            UpdateButtonColor(voiceButton, isVoiceEnabled, origVoiceColor);
        }

        private void UpdateButtonColor(Button button, bool isSFXEnabled, Color origColor)
        {
            if (button == null) return;

            var colors = button.colors;
            colors.normalColor = isSFXEnabled ? origColor : disabledButtonColor;
            colors.selectedColor = isSFXEnabled ? origColor : disabledButtonColor;
            button.colors = colors;
        }

        private void OnDestroy()
        {
            if (musicButton != null)
            {
                musicButton.onClick.RemoveListener(ToggleMusic);
            }

            if (sfxButton != null)
            {
                sfxButton.onClick.RemoveListener(ToggleSFX);
            }

            if (voiceButton != null)
            {
                voiceButton.onClick.RemoveListener(ToggleVoice);
            }
        }
    }
}
