using System;
using Ouroboros.Common.Logging;
using Ouroboros.Common.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ouroboros.Common.Audio
{
    public class MusicPlayerWidget : MonoBehaviour
    {
        public Action OnPlay { get; set; }
        public Action OnStop { get; set; }

        [SerializeField] private MusicPlayer musicPlayer;
        [SerializeField] private Button playButton;
        [SerializeField] private Button stopButton;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private GameObject playingContainer;

        private void Awake()
        {
            if (playButton != null)
            {
                playButton.onClick.RemoveListener(OnClickPlay);
                playButton.onClick.AddListener(OnClickPlay);
            }

            if (stopButton != null)
            {
                stopButton.onClick.RemoveListener(OnClickStop);
                stopButton.onClick.AddListener(OnClickStop);
            }

            UpdateUIState(false);
        }

        private void OnEnable()
        {
            AddListeners();
            UpdateUIState();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        private void AddListeners()
        {
            if (musicPlayer == null) return;

            RemoveListeners();

            musicPlayer.OnMusicPlay += OnMusicPlay;
            musicPlayer.OnMusicStop += OnMusicStop;
        }

        private void RemoveListeners()
        {
            if (musicPlayer == null) return;

            musicPlayer.OnMusicPlay -= OnMusicPlay;
            musicPlayer.OnMusicStop -= OnMusicStop;
        }

        public void ActivateIfHasClips()
        {
            var hasClips = musicPlayer.Clips.Count > 0;

            Logs.Debug<AudioManager>($"MusicPlayerWidget ActiveIfHasClips hasClips={hasClips}, isActive={gameObject.activeSelf}");

            if (hasClips != gameObject.activeSelf)
            {
                gameObject.SetActive(hasClips);
            }
        }

        public void SetPlayer(MusicPlayer musicPlayer)
        {
            if (this.musicPlayer == musicPlayer) return;
            
            RemoveListeners();
            this.musicPlayer = musicPlayer;
            AddListeners();
            UpdateUIState();
        }

        private void OnMusicStop()
        {
            UpdateUIState();
        }

        private void OnMusicPlay(MusicClipData data)
        {
            UpdateUIState();
        }

        public void OnClickPlay()
        {
            musicPlayer.Play();
        }

        public void OnClickStop()
        {
            musicPlayer.Stop();
        }

        public void OnClickNext()
        {
            musicPlayer.PlayNext();
        }

        private void UpdateUIState()
        {
            UpdateUIState(musicPlayer != null ? musicPlayer.IsPlaying : false);
        }

        private void UpdateUIState(bool isPlaying)
        {
            playButton.SetActiveSafe(!isPlaying);
            stopButton.SetActiveSafe(isPlaying);
            playingContainer.SetActiveSafe(isPlaying);

            if (nameText != null
                && musicPlayer != null
                && musicPlayer.PlayingClip != null)
            {
                nameText.text = musicPlayer.PlayingClip.Name;
            }
        }
    }
}
