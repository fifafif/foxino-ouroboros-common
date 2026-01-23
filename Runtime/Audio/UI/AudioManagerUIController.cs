using UnityEngine;
using UnityEngine.UI;

namespace Ouroboros.Common.Audio.UI
{
    public class AudioManagerUIController : MonoBehaviour
    {
        [SerializeField] private Button musicButton;
        [SerializeField] private Button sfxButton;

        [Header("Optional Icons")]
        [SerializeField] private GameObject musicOnIcon;
        [SerializeField] private GameObject musicOffIcon;
        [SerializeField] private GameObject sfxOnIcon;
        [SerializeField] private GameObject sfxOffIcon;

        [Header("Colors")]
        [SerializeField] private Color disabledButtonColor = new Color(1f, 1f, 1f, 0.5f);

        private Color origMusicColor;
        private Color origSFXColor;

        private void Awake()
        {
            origSFXColor = sfxButton.colors.normalColor;
            origMusicColor = musicButton.colors.normalColor;
        }

        private void Start()
        {
            SetupButtons();
            AudioManager.RegisterForOnLoadedEvent(UpdateButtonVisuals);
            AudioManager.RegisterOnMusicEnabled(OnMusicEnabled);
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

            if (musicOnIcon != null)
                musicOnIcon.SetActive(isMusicEnabled);

            if (musicOffIcon != null)
                musicOffIcon.SetActive(!isMusicEnabled);

            if (sfxOnIcon != null)
                sfxOnIcon.SetActive(isSFXEnabled);

            if (sfxOffIcon != null)
                sfxOffIcon.SetActive(!isSFXEnabled);

            UpdateButtonColor(musicButton, isMusicEnabled, origSFXColor);
            UpdateButtonColor(sfxButton, isSFXEnabled, origSFXColor);
        }

        private void UpdateButtonColor(Button button, bool isSFXEnabled, Color origColor)
        {
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
        }
    }
}
