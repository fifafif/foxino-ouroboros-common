using Ouroboros.Common.Logging;
using Ouroboros.Common.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Audio;

namespace Ouroboros.Common.Audio
{
    public class AudioManager : MonoBehaviour
    {
        private const int SourceCount = 20;
        private const string MusicVolumeKey = "Audio_MusicVolume";
        private const string SFXVolumeKey = "Audio_SFXVolume";
        private const string MusicEnabledKey = "Audio_MusicEnabled";
        private const string SFXEnabledKey = "Audio_SFXEnabled";

        public static AudioManager instance { get; private set; }

        public bool IsAudioEnabled { get; private set; }
        public bool IsMusicEnabled { get; private set; }
        public bool IsSFXEnabled { get; private set; }
        public Action<PlayingAudio> OnMusicPlay { get; set; }
        public AudioDatabase[] Databases => databases;
        public RegisteredCallback OnSettingsLoaded { get; private set; } = new RegisteredCallback();
        public Action<bool> OnMusicEnabled { get; set; }

        [SerializeField] private AudioDatabase[] databases;
        [SerializeField] private AudioMixer mixer;
        [SerializeField] public AudioMixerGroup mixerGroup;
        [SerializeField] public AudioMixerGroup sfxMixerGroup;
        [SerializeField] public AudioMixerGroup musicMixerGroup;
        [SerializeField] public AudioMixerGroup voiceMixerGroup;

        [Range(0f, 1f)]
        public float MasterVolume = 1f;
        [Range(0f, 1f)]
        public float MusicVolume = 1f;
        [Range(0f, 1f)]
        public float SfxVolume = 1f;
        [Range(0f, 1f)]
        public float VoiceVolume = 1f;

        [SerializeField] public bool isMusicEnabledByDefault = true;

        private float baseMasterVolume;
        private float baseMusicVolume;
        private float baseSfxVolume;
        private float baseVoiceVolume;

        private float musicVolumeMultiplier = 1f;
        private float sfxVolumeMultiplier = 1f;

        private int currentSoundSource;
        private AudioSource[] soundSources;
        private AudioSource musicSource;
        private Dictionary<string, AudioClipBase> clipDatabaseMap = new Dictionary<string, AudioClipBase>();
        private Action onMusicFinish;
        private List<PlayingAudio> playingAudios = new List<PlayingAudio>();
        private List<int> audiosToDelete = new List<int>();
        private int audioIds;
        private float lastPlayedSoundTime;
        private AudioClip lastPlayedSoundClip;
        private bool hasInit;
        private Dictionary<string, MusicPlayer> musicPlayers = new Dictionary<string, MusicPlayer>();
        private static string StreamingAssetsPath;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;

            Init();
        }

        public static void SetStreamingAssetsPath(string path)
        {
            Logs.Debug<AudioManager>($"SetStreamingAssetsPath path={path}");
            StreamingAssetsPath = path;
        }

        public static string GetStreamingAssetsPath()
        {
            if (!string.IsNullOrEmpty(StreamingAssetsPath))
            {
                return StreamingAssetsPath;
            }

            return Application.streamingAssetsPath;
        }

        public static void RegisterForOnLoadedEvent(Action onLoaded)
        {
            Assert.IsNotNull(instance, AssertMessage);
            instance.OnSettingsLoaded.Register(onLoaded);
        }

        public static void RegisterOnMusicEnabled(Action<bool> onEnabled)
        {
            Assert.IsNotNull(instance, AssertMessage);
            instance.OnMusicEnabled += onEnabled;
        }

        public static void UnregisterOnMusicEnabled(Action<bool> onEnabled)
        {
            Assert.IsNotNull(instance, AssertMessage);
            instance.OnMusicEnabled -= onEnabled;
        }

        private void Start()
        {
            CaptureBaseVolumes();
            LoadAudioSettings();
            ApplyDefaultVolumes();

            OnSettingsLoaded?.Invoke();
        }

        private void CaptureBaseVolumes()
        {
            baseMasterVolume = MasterVolume;
            baseMusicVolume = MusicVolume;
            baseSfxVolume = SfxVolume;
            baseVoiceVolume = VoiceVolume;

            Logs.Debug<AudioManager>($"Captured base volumes: Master={baseMasterVolume}, Music={baseMusicVolume}, SFX={baseSfxVolume}, Voice={baseVoiceVolume}");
        }

        private void LoadAudioSettings()
        {
            musicVolumeMultiplier = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
            sfxVolumeMultiplier = PlayerPrefs.GetFloat(SFXVolumeKey, 1f);

            IsMusicEnabled = PlayerPrefs.GetInt(MusicEnabledKey, isMusicEnabledByDefault ? 1 : 0) == 1;
            IsSFXEnabled = PlayerPrefs.GetInt(SFXEnabledKey, 1) == 1;

            CalculateMusicVolume();
            CalculateSFXVolume();

            Logs.Debug<AudioManager>($"Loaded AudioSettings: MusicEnabled={IsMusicEnabled}, MusicMultiplier={musicVolumeMultiplier}, MusicVolume={MusicVolume}, SFXEnabled={IsSFXEnabled}, SFXMultiplier={sfxVolumeMultiplier}, SfxVolume={SfxVolume}");
        }

        private void ApplyDefaultVolumes()
        {
            SetMasterVolume(MasterVolume);
            SetMusicMixerVolume(MusicVolume);
            SetSFXMixerVolume(SfxVolume);
            mixer.SetFloat("voice_volume", CalculateVolume(VoiceVolume));
        }

        private void SetMusicMixerVolume(float volume)
        {
            mixer.SetFloat("music_volume", CalculateVolume(volume));
        }

        private void SetSFXMixerVolume(float volume)
        {
            mixer.SetFloat("sfx_volume", CalculateVolume(volume));
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }

        private const string AssertMessage = "AudioManager not found! Does exist in the scene and has been initialized?";

        public static void RegisterMusicPlayer(MusicPlayer player)
        {
            Assert.IsNotNull(instance, AssertMessage);

            if (string.IsNullOrEmpty(player.Id))
            {
                Logs.Error<AudioManager>($"Cannot register MusicPlayer! Id is null! player={player}", player);
                return;
            }

            instance.musicPlayers[player.Id] = player;
        }

        public static void UnregisterMusicPlayer(MusicPlayer musicPlayer)
        {
            if (instance == null
                || string.IsNullOrEmpty(musicPlayer.Id)) return;

            instance.musicPlayers.Remove(musicPlayer.Id);
        }

        public static MusicPlayer GetMusicPlayer(string id)
        {
            Assert.IsNotNull(instance, AssertMessage);

            instance.musicPlayers.TryGetValue(id, out var musicPlayer);
            return musicPlayer;
        }

        public static void EnableAudio(bool isEnabled)
        {
            Assert.IsNotNull(instance, AssertMessage);

            instance.EnableAudioInternal(isEnabled);
        }

        private void EnableAudioInternal(bool isEnabled)
        {
            IsAudioEnabled = isEnabled;

            float volume = isEnabled ? 1f : 0;

            SetMasterVolume(CalculateVolume(volume));
            MasterVolume = volume;
        }

        private void SetMusicPitch(float timeScale)
        {
            mixer.SetFloat("pitch_music", timeScale);
        }

        public static void SetMusicVolume(float volume)
        {
            instance.SetMusicVolumeInternal(volume);
        }

        public static void SetVoiceVolume(float volume)
        {
            instance.SetVoiceVolumeInternal(volume);
        }

        public static void SetSFXVolume(float volume)
        {
            instance.SetSFXVolumeInternal(volume);
        }

        public static void ToggleMusicEnabled()
        {
            instance.ToggleMusicEnabledInternal();
        }

        public static void ToggleSFXEnabled()
        {
            instance.ToggleSFXEnabledInternal();
        }

        public static void SetMusicEnabled(bool enabled)
        {
            instance.SetMusicEnabledInternal(enabled);
        }

        public static void SetSFXEnabled(bool enabled)
        {
            instance.SetSFXEnabledInternal(enabled);
        }

        private void ToggleMusicEnabledInternal()
        {
            SetMusicEnabledInternal(!IsMusicEnabled);
        }

        private void ToggleSFXEnabledInternal()
        {
            SetSFXEnabledInternal(!IsSFXEnabled);
        }

        private void SetMusicEnabledInternal(bool enabled)
        {
            Logs.Debug<AudioManager>($"SetMusicEnabledInternal enabled={enabled}");
            IsMusicEnabled = enabled;

            CalculateMusicVolume();
            SetMusicMixerVolume(MusicVolume);

            OnMusicEnabled?.Invoke(enabled);

            PlayerPrefs.SetInt(MusicEnabledKey, IsMusicEnabled ? 1 : 0);
            PlayerPrefs.Save();
        }

        private void CalculateMusicVolume()
        {
            MusicVolume = baseMusicVolume * musicVolumeMultiplier * (IsMusicEnabled ? 1f : 0f);
        }

        private void SetSFXEnabledInternal(bool enabled)
        {
            Logs.Debug<AudioManager>($"SetSFXEnabledInternal enabled={enabled}");
            IsSFXEnabled = enabled;

            CalculateSFXVolume();
            SetSFXMixerVolume(SfxVolume);

            PlayerPrefs.SetInt(SFXEnabledKey, IsSFXEnabled ? 1 : 0);
            PlayerPrefs.Save();
        }

        private void CalculateSFXVolume()
        {
            SfxVolume = baseSfxVolume * sfxVolumeMultiplier * (IsSFXEnabled ? 1f : 0f);
        }

        private void SetSFXVolumeInternal(float volume)
        {
            Logs.Debug<AudioManager>($"SetSFXVolumeInternal volume={volume}");

            sfxVolumeMultiplier = Mathf.Clamp01(volume);

            CalculateSFXVolume();
            SetSFXMixerVolume(SfxVolume);

            PlayerPrefs.SetFloat(SFXVolumeKey, sfxVolumeMultiplier);
            PlayerPrefs.Save();
        }

        private void SetMusicVolumeInternal(float volume)
        {
            Logs.Debug<AudioManager>($"SetMusicVolumeInternal volume={volume}");

            musicVolumeMultiplier = Mathf.Clamp01(volume);

            CalculateMusicVolume();
            SetMusicMixerVolume(CalculateVolume(MusicVolume));

            PlayerPrefs.SetFloat(MusicVolumeKey, musicVolumeMultiplier);
            PlayerPrefs.Save();
        }

        private void SetVoiceVolumeInternal(float volume)
        {
            Logs.Debug<AudioManager>($"SetVoiceVolumeInternal volume={volume}");
            VoiceVolume = Mathf.Clamp01(volume);
            mixer.SetFloat("voice_volume", CalculateVolume(volume));
        }

        public void Init()
        {
            if (hasInit) return;

            hasInit = true;

            for (var i = 0; i < Databases.Length; ++i)
            {
                var database = Databases[i];
                database.Init();

                for (var j = 0; j < database.Clips.Count; ++j)
                {
                    clipDatabaseMap[database.Clips[j].Id] = database.Clips[j];
                }
            }

            soundSources = new AudioSource[SourceCount];

            for (int i = 0; i < SourceCount; ++i)
            {
                soundSources[i] = gameObject.AddComponent<AudioSource>();
            }

            musicSource = gameObject.AddComponent<AudioSource>();

            if (musicMixerGroup != null)
            {
                musicSource.outputAudioMixerGroup = musicMixerGroup;
            }
            else if (mixerGroup != null)
            {
                musicSource.outputAudioMixerGroup = mixerGroup;
            }
        }

        public static int PlaySoundTryOnTarget(
            string audioId, AudioSource audioSource, GameObject target = null, float delay = 0f)
        {
            if (string.IsNullOrEmpty(audioId))
            {
                return -1;
            }

            AudioSource source;

            if (target != null
                && target.TryGetComponent<AudioSource>(out var targetSource))
            {
                source = targetSource;
            }
            else
            {
                source = audioSource;
            }

            return PlaySound(audioId, source, false, 0f, delay);
        }

        public static int PlaySound(string id)
        {
            return instance.PlaySoundInternal(new PlayAudioPayload(id));
        }

        public static int PlaySound(AudioClip clip)
        {
            return PlaySound(clip, false);
        }

        public static int PlaySound(
            AudioClip clip,
            bool loop = false,
            float volume = 1f,
            float fadeInTime = 0f)
        {
            return PlaySound(clip, null, loop, volume, fadeInTime);
        }

        public static int PlaySound(
            string id,
            AudioSource source = null,
            bool loop = false,
            float fadeInTime = 0f,
            float delay = 0f)
        {
            return PlaySound(new PlayAudioPayload(id, source, null, loop, 1f, fadeInTime, delay));
        }

        public static int PlaySound(PlayAudioPayload payload)
        {
            return instance.PlaySoundInternal(payload);
        }

        public static int PlaySound(
            AudioClip clip,
            AudioSource source = null,
            bool loop = false,
            float volume = 1f,
            float fadeInTime = 0f,
            float delay = 0f)
        {
            return instance.PlaySoundInternal(new PlayAudioPayload(clip, source, null, loop, volume, fadeInTime, delay));
        }

        public int PlaySoundInternal(PlayAudioPayload payload)
        {
            if (payload.clip == null)
            {
                var clipBase = FindAudio(payload.id);
                if (clipBase == null)
                {
                    Logs.Warning<AudioManager>($"Couldn't find audio id={payload.id}");
                    return -1;
                }

                if (Time.timeAsDouble - clipBase.LastPlayAtTime <= 0.05)
                {
                    return -1;
                }

                AudioClipData audioData;
                if (payload.isUsingArrayIndex)
                {
                    audioData = clipBase.GetAudioData(payload.arrayNormalizedIndex);
                }
                else
                {
                    audioData = clipBase.GetAudioData();
                }

                if (audioData == null)
                {
                    Logs.Warning<AudioManager>($"Couldn't find audio id={payload.id}");
                    return -1;
                }


                payload.clip = audioData.AudioClip;
                payload.volume *= audioData.FinalVolume;

                clipBase.LastPlayAtTime = Time.timeAsDouble;
            }

            Logs.Debug<AudioManager>($"PlaySound id={payload.id}, clip={payload.clip}, source={payload.source}", payload.source);

            var audio = CreatePlayingAudio(payload);
            if (audio == null)
            {
                return -1;
            }

            // if (lastPlayedSoundClip == payload.clip
            //     && (Time.time - lastPlayedSoundTime) < 0.05)
            // {
            //     return -1;
            // }

            lastPlayedSoundTime = Time.time;
            lastPlayedSoundClip = payload.clip;

            if (payload.fadeInTime > 0f)
            {
                audio.FadeIn(payload.fadeInTime);
            }

            audio.IsLooping = payload.loop;
            audio.AudioSource.loop = payload.loop;
            audio.AudioSource.volume = payload.volume * SfxVolume;
            audio.Play(payload.delay);

            playingAudios.Add(audio);

            return audio.Id;
        }

        public static void StopSound(int id)
        {
            StopSound(id, 0f);
        }

        public static void StopSound(int id, float fadeOutTime)
        {
            instance.StopSoundInternal(id, fadeOutTime);
        }

        private void StopSoundInternal(int id, float fadeOutTime)
        {
            Logs.Debug<AudioManager>($"StopSound id={id}, fadeOutTime={fadeOutTime}");

            var audio = GetPlayingAudio(id);
            if (audio == null) return;

            if (fadeOutTime > 0f)
            {
                audio.FadeOut(fadeOutTime);
            }
            else
            {
                audio.Stop();
            }
        }

        public void PauseMusic(int id, bool isPause)
        {
            var audio = GetPlayingAudio(id);
            if (audio == null) return;

            if (isPause)
            {
                audio.Pause();
            }
            else
            {
                audio.Play();
            }
        }

        public int PlayMusic(AudioClip clip, AudioSource audioSource, Action onMusicFinish, float fadeInDuration = 0f)
        {
            Logs.Debug<AudioManager>($"PlayMusic clip={clip}, source={audioSource}, fadeInDuration={fadeInDuration}");

            if (audioSource == null)
            {
                audioSource = musicSource;
            }

            if (clip != null)
            {
                audioSource.clip = clip;
            }

            var audio = CreatePlayingAudio(new PlayAudioPayload
            {
                clip = clip,
                source = audioSource,
                volume = 1
            });

            if (audio == null)
            {
                return -1;
            }

            if (fadeInDuration > 0f)
            {
                audio.FadeIn(fadeInDuration);
            }

            audio.OnFinish = onMusicFinish;
            audio.IsSingleClip = true;
            audio.Play();
            playingAudios.Add(audio);

            OnMusicPlay?.Invoke(audio);

            return audio.Id;
        }

        public void PlayMusic(AudioClip clip = null, Action onFinish = null)
        {
            PlayMusic(clip, null, onFinish);
        }

        public void StopMusic(int audioId)
        {
            var audio = GetPlayingAudio(audioId);
            if (audio == null) return;

            audio.Stop();
        }

        public void StopMusic(float fadeOutTime = 0f)
        {
            if (fadeOutTime > 0f)
            {
                StartCoroutine(MusicFadeOutEnumerator(fadeOutTime));
            }
            else
            {
                musicSource.Stop();
            }
        }

        public void ResumeMusic()
        {
            musicSource.Play();
        }

        public void ToggleMusic()
        {
            if (IsMusicEnabled)
            {
                StopMusic();
            }
            else
            {
                ResumeMusic();
            }
        }

        public void SetMasterVolume(float volume)
        {
            //mixer.SetFloat("volume_master", CalculateVolume(volume));
        }

        private PlayingAudio CreatePlayingAudio(PlayAudioPayload payload)
        {
            var audio = new PlayingAudio(payload.clip, SetupSource(payload))
            {
                Id = audioIds,
                Volume = payload.volume
            };

            ++audioIds;

            return audio;
        }

        private AudioClipBase FindAudio(string id)
        {
            if (!clipDatabaseMap.TryGetValue(id, out var clipData))
            {
                return null;
            }

            return clipData;
        }

        private static float CalculateVolume(float volume)
        {
            volume = Mathf.Log10(volume + 0.0001f) * 20;
            return volume;
        }

        private AudioSource SetupSource(PlayAudioPayload audioPayload)
        {
            var source = audioPayload.source;
            if (source == null)
            {
                source = GetNextAudioSource();
                if (audioPayload.mixerGroup == null)
                {
                    source.outputAudioMixerGroup = mixerGroup;
                }
                else
                {
                    source.outputAudioMixerGroup = audioPayload.mixerGroup;
                }
            }

            return source;
        }

        private AudioSource GetNextAudioSource()
        {
            var source = soundSources[currentSoundSource];

            ++currentSoundSource;

            if (currentSoundSource >= SourceCount)
            {
                currentSoundSource = 0;
            }

            return source;
        }

        private IEnumerator MusicFadeOutEnumerator(float fadeOutTime)
        {
            if (fadeOutTime > 0f)
            {
                float volumeStart = musicSource.volume;
                float timeLeft = fadeOutTime;

                while (timeLeft > 0f)
                {
                    timeLeft -= Time.deltaTime;

                    musicSource.volume = Mathf.Lerp(0f, volumeStart, timeLeft / fadeOutTime);

                    yield return null;
                }
            }

            musicSource.Stop();
        }

        private void Update()
        {
            if (onMusicFinish != null
                && !musicSource.isPlaying)
            {
                var clone = onMusicFinish;
                onMusicFinish = null;

                clone();
            }

            var dt = Time.deltaTime;
            audiosToDelete.Clear();

            for (int i = 0; i < playingAudios.Count; ++i)
            {
                var audio = playingAudios[i];

                if (!audio.IsValid)
                {
                    audiosToDelete.Add(i);
                    continue;
                }

                audio.Update(dt);

                if (audio.IsFinished())
                {
                    audiosToDelete.Add(i);
                }
            }

            for (int i = audiosToDelete.Count - 1; i >= 0; --i)
            {
                var index = audiosToDelete[i];
                playingAudios[index].OnFinish?.Invoke();
                playingAudios.RemoveAt(index);
            }
        }

        private PlayingAudio GetPlayingAudio(int id)
        {
            foreach (var audio in playingAudios)
            {
                if (audio.Id == id) return audio;
            }

            return null;
        }

        public static void ClearAllSettings()
        {
            PlayerPrefs.DeleteKey(MusicVolumeKey);
            PlayerPrefs.DeleteKey(MusicEnabledKey);
            PlayerPrefs.DeleteKey(SFXVolumeKey);
            PlayerPrefs.DeleteKey(SFXEnabledKey);
            PlayerPrefs.Save();
        }
    }
}