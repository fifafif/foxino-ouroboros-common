using System;
using System.Collections.Generic;
using Ouroboros.Common.Logging;
using Ouroboros.Common.Services;
using Ouroboros.Common.Utils;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Random = UnityEngine.Random;

#if ADDRESSABLES_ENABLED
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
#endif

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

#if ADDRESSABLES_ENABLED
        private Dictionary<MusicClipData, AsyncOperationHandle<AudioClip>> addressableHandles =
            new Dictionary<MusicClipData, AsyncOperationHandle<AudioClip>>();
#endif

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

#if ADDRESSABLES_ENABLED
            // Release all Addressables handles
            foreach (var handle in addressableHandles.Values)
            {
                if (handle.IsValid())
                    Addressables.Release(handle);
            }
            addressableHandles.Clear();
#endif
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
            Logs.Debug<MusicPlayer>($"MusicPlayer Play clip={clip.Name}, name={name}");

            TrySetIndex(clip);
            IsPlaying = true;
            isPaused = false;

            // Launch async without blocking
            PlayAsync(clip).Forget();

            return clip;
        }

        private async UniTask<AudioClip> LoadClipAsync(MusicClipData clip, IProgress<float> progress = null)
        {
            // Priority 1: Use cached clip if available
            if (clip.AudioClip != null)
                return clip.AudioClip;

#if ADDRESSABLES_ENABLED
            // Priority 2: Try Addressables
            if (clip.AddressableReference != null && clip.AddressableReference.RuntimeKeyIsValid())
            {
                var result = await LoadFromAddressables(clip, progress);
                if (result != null) return result;
            }
#endif

            // Priority 3: Try StreamingAssets
            if (!string.IsNullOrEmpty(clip.StreamingAssetsPath))
            {
                var result = await LoadFromStreamingAssets(clip, progress);
                if (result != null) return result;
            }

            // Priority 4: Nothing available
            Logs.Warning<MusicPlayer>($"No loading source for music: {clip.Name}");
            return null;
        }

#if ADDRESSABLES_ENABLED
        private async UniTask<AudioClip> LoadFromAddressables(MusicClipData clip, IProgress<float> progress)
        {
            try
            {
                // First, check if the location exists (important for WebGL)
                var locateHandle = Addressables.LoadResourceLocationsAsync(clip.AddressableReference.RuntimeKey);
                await locateHandle.ToUniTask();

                if (locateHandle.Status != AsyncOperationStatus.Succeeded || locateHandle.Result.Count == 0)
                {
                    Logs.Warning<MusicPlayer>($"Addressables location not found for: {clip.Name}");
                    Addressables.Release(locateHandle);
                    return null;
                }

                Addressables.Release(locateHandle);

                // Now load the actual asset
                var handle = clip.AddressableReference.LoadAssetAsync<AudioClip>();

                // Report progress
                while (!handle.IsDone)
                {
                    progress?.Report(handle.PercentComplete);
                    await UniTask.Yield();
                }
                progress?.Report(1f);

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    var loadedClip = handle.Result;
                    clip.AudioClip = loadedClip; // Cache it

                    // Store handle for cleanup
                    if (!addressableHandles.ContainsKey(clip))
                        addressableHandles[clip] = handle;

                    Logs.Debug<MusicPlayer>($"Loaded from Addressables: {clip.Name}");
                    return loadedClip;
                }
                else
                {
                    Logs.Error<MusicPlayer>($"Addressables load failed: {clip.Name}, Status: {handle.Status}");
                    Addressables.Release(handle);
                    return null;
                }
            }
            catch (Exception ex)
            {
                Logs.Error<MusicPlayer>($"Addressables exception: {clip.Name}, {ex.Message}");
                return null;
            }
        }
#endif

        private async UniTask<AudioClip> LoadFromStreamingAssets(MusicClipData clip, IProgress<float> progress)
        {
            Logs.Debug<MusicPlayer>($"Loading from StreamingAssets: {clip.StreamingAssetsPath}");

            var audioType = AudioStreamingLoader.GetAudioTypeFromPath(clip.StreamingAssetsPath);
            var tcs = new UniTaskCompletionSource<AudioClip>();

            // Wrap existing coroutine
            StartCoroutine(AudioStreamingLoader.LoadAudioClip(
                clip.StreamingAssetsPath,
                (loadedClip) =>
                {
                    if (loadedClip != null)
                    {
                        clip.AudioClip = loadedClip; // Cache it
                        progress?.Report(1f);
                    }
                    tcs.TrySetResult(loadedClip);
                },
                audioType
            ));

            return await tcs.Task;
        }

        private async UniTaskVoid PlayAsync(MusicClipData clip)
        {
            try
            {
                var audioClip = await LoadClipAsync(clip, null);

                if (audioClip != null)
                {
                    audioId = AudioManager.instance.PlayMusic(audioClip, audioSource, OnMusicFinish, fadeInDuration);
                    PlayingClip = clip;
                    OnMusicPlay?.Invoke(clip);
                }
                else
                {
                    Logs.Error<MusicPlayer>($"Failed to load: {clip.Name}");
                    IsPlaying = false;
                    OnMusicFinish(); // Trigger next track
                }
            }
            catch (Exception ex)
            {
                Logs.Error<MusicPlayer>($"PlayAsync exception: {ex.Message}");
                IsPlaying = false;
                OnMusicFinish();
            }
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