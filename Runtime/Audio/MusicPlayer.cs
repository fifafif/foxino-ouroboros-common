using System;
using System.Collections.Generic;
using Ouroboros.Common.Logging;
using Ouroboros.Common.Services;
using Ouroboros.Common.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Ouroboros.Common.Audio
{
    public class MusicPlayer : MonoBehaviour
    {
        public Action<MusicClipData> OnMusicPlay { get; set; }
        public Action OnMusicStop { get; set; }
        public Action OnNewClipsAdded { get;  set; }
        public AudioSource AudioSource => audioSource;
        public string Id => id;
        public bool IsPlaying { get; private set; }
        public MusicClipData PlayingClip { get; private set; }
        public List<MusicClipData> Clips => musicDatabase != null ? musicDatabase.Clips : customClips;
        
        [SerializeField] private string id;
        [SerializeField] private bool isRegisteringToAudioManager;
        [SerializeField] private MusicDatabase musicDatabase;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private bool shuffle;
        [SerializeField] private bool loopOne;
        [SerializeField] private bool loopAll;
        [SerializeField] private float fadeInDuration = 1f;
        [SerializeField] private bool isPlayingOnStart;
        [SerializeField] private float startDelay = 0f;

        private List<MusicClipData> customClips = new List<MusicClipData>();
        private int currentIndex;
        private int audioId;
        private bool isPaused;

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void Start()
        {
            if (isRegisteringToAudioManager
                && !string.IsNullOrEmpty(id))
            {
                AudioManager.RegisterMusicPlayer(this);
            }

            if (shuffle)
            {
                Shuffle();
            }

            if (isPlayingOnStart)
            {
                AudioManager.RegisterForOnLoadedEvent(() =>
                {
                    this.CoroutineWait(startDelay, () => Play());
                });
            }
        }

        private void OnDestroy()
        {
            if (isRegisteringToAudioManager)
            {
                AudioManager.UnregisterMusicPlayer(this);
            }
        }

        private void Shuffle()
        {
            if (Clips.Count <= 1) return;

            int newIndex;
            do
            {
                newIndex = Random.Range(0, Clips.Count);
            }
            while (newIndex == currentIndex);

            currentIndex = newIndex;
        }

        public MusicClipData Play(string musicName)
        {
            var clip = Clips.Find(d => d.Name == musicName);
            if (clip == null)
            {
                return null;
            }

            return Play(clip);
        }

        public MusicClipData Play()
        {
            var clip = GetCurrentMusic();
            if (clip == null)
            {
                return null;
            }

            return Play(clip);
        }

        public MusicClipData Play(MusicClipData clip)
        {
            Logs.Debug<AudioManager>($"MusicPlayer Play clip={clip}, name={name}");

            TrySetIndex(clip);

            IsPlaying = true;
            isPaused = false;
            
            audioId = AudioManager.instance.PlayMusic(clip.AudioClip, audioSource, OnMusicFinish, fadeInDuration);
            PlayingClip = clip;
            OnMusicPlay?.Invoke(clip);
            
            return clip;
        }

        public void Pause()
        {
            Logs.Debug<AudioManager>($"MusicPlayer Pause name={name}");

            if (isPaused
                || !IsPlaying) return;

            IsPlaying = false;
            isPaused = true;

            AudioManager.instance.PauseMusic(audioId, true);
        }

        public void Stop()
        {
            Logs.Debug<AudioManager>($"MusicPlayer Stop name={name}");

            if (!IsPlaying
                && !isPaused) return;

            IsPlaying = false;
            isPaused = false;
            PlayingClip = null;

            AudioManager.instance.StopMusic(audioId);

            OnMusicStop?.Invoke();
        }

        public void AddCustomClip(MusicClipData clip)
        {
            customClips.Add(clip);
        }
        
        public void AddCustomClips(List<MusicClipData> clips)
        {
            Logs.Debug<AudioManager>($"MusicPlayer AddCustomClips clips count={clips.Count}");

            customClips = clips;
            OnNewClipsAdded?.Invoke();
        }

        public void ClearCustomClips()
        {
            customClips.Clear();
        }

        private void TrySetIndex(MusicClipData clip)
        {
            var index = Clips.IndexOf(clip);
            if (index != -1)
            {
                currentIndex = index;
            }
        }

        private void OnMusicFinish()
        {
            Logs.Debug<AudioManager>($"MusicPlayer OnMusicFinish");

            if (!IsPlaying) return;

            if (!loopOne)
            {
                Next();
            }

            this.CoroutineWaitFrames(1, () => Play());
        }

        public MusicClipData PlayNext()
        {
            Logs.Debug<AudioManager>($"MusicPlayer PlayNext");

            if (!Next())
            {
                return null;
            }

            IsPlaying = false;
            return Play();
        }

        public MusicClipData PlayPrev()
        {
            Logs.Debug<AudioManager>($"MusicPlayer PlayPrev");

            Prev();
            IsPlaying = false;
            return Play();
        }

        public MusicClipData GoToNext()
        {
            Next();
            return GetCurrentMusic();
        }

        public MusicClipData GoToPrev()
        {
            Prev();
            return GetCurrentMusic();
        }

        private bool Next()
        {
            if (shuffle)
            {
                Shuffle();
                return true;
            }

            if (currentIndex >= Clips.Count - 1)
            {
                if (loopAll)
                {
                    currentIndex = 0;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            ++currentIndex;
            return true;
        }

        private void Prev()
        {
            if (currentIndex <= 0)
            {
                currentIndex = Clips.Count - 1;
            }
            else
            {
                --currentIndex;
            }
        }

        public MusicClipData GetCurrentMusic()
        {
            if (Clips.Count <= 0)
            {
                return null;
            }

            return Clips[currentIndex];
        }
    }
}