using System;
using System.Collections;
using Ouroboros.Common.Services;
using Ouroboros.Common.Utils;
using UnityEngine;

namespace Ouroboros.Common.Audio
{
    [RequireComponent(typeof(AudioManager))]
    public class BPMAudioController : MonoBehaviour
    {
        public enum BeatDetectionMethod
        {
            Manual = 0,
            ManualFromMusic = 1,
            AudioProcessor = 2
        }
        
        [SerializeField] private int beatInBarCount = 4;
        [SerializeField] private float manualBPM = 90;
        [SerializeField] private float manualFirstBeatOffset;
        
        public float BPM { get; private set; }
        public float BeatDuration { get; private set; }        
        public float TimeToNextBeat => (float)(NextBeatTime - elapsedClipTime);
        public double NextBeatTime => lastBeatTime + BeatDuration;
        public float ElapsedBeatTime => (float)(elapsedClipTime - lastBeatTime);
        public double ElapsedClipTime => elapsedClipTime;
        public float ElapsedBeatTimeNormalized
        {
            get
            {
                if (BeatDuration <= 0)
                {
                    return 0f;
                }
                
                return ElapsedBeatTime / BeatDuration;
            }
        }

        public int ElapsedBeatInBarCount => BeatCount % BeatInBarCount;
        public int RemainingBeatInBarCount => BeatInBarCount - ElapsedBeatInBarCount;

        public int BeatCount { get; private set; }
        public int BarCount { get; private set; }
        public Action<int, int> OnBeat { get; set; }
        public Action<int, int> OnBar { get; set; }
        
        public int BeatInBarCount => beatInBarCount;
        private double elapsedClipTime;
        private double lastBeatTime;
        private double nextBeatTime;
        private bool isGameRunning;
        private bool isWaitingForFirstBeat;

        private float origSfxVolume;
        private float origMusicVolume;
        private Coroutine beatCoroutine;
        private bool isBeatDisabled;
        
        private AudioManager audioManager;
        private MusicPlayer musicPlayer;
        private AudioSource musicPlayerAudioSource;
        private float firstBeatOffset;
        
        private void Awake()
        {
            ServiceLocator.Register(this);
            
            audioManager = GetComponent<AudioManager>();
            //audioManager.OnMusicPlay += OnMusicPlay;
            
            musicPlayer = GetComponentInChildren<MusicPlayer>();
            musicPlayer.OnMusicPlay += OnMusicPlay;
            musicPlayerAudioSource = musicPlayer.AudioSource;
        }

        public void StartBeating()
        {
            Debug.Log($"[GameAudio] StartGame");

            isGameRunning = true;
            isWaitingForFirstBeat = true;
            lastBeatTime = 0;
            BeatCount = -1;
            BarCount = -1;

            beatCoroutine = StartCoroutine(BeatRoutine());
        }

        private void SetBPM(float bpm, float firstBeatOffset)
        {
            BPM = bpm;
            this.firstBeatOffset = firstBeatOffset;
            BeatDuration = BPMToDuration(BPM);
        }

        public void EndGame()
        {
            isGameRunning = false;
            this.CoroutineStop(beatCoroutine);
        }

        public void Pause()
        {
            this.CoroutineStop(beatCoroutine);
            isGameRunning = false;
        }

        public void Unpause()
        {
            isGameRunning = true;
            isWaitingForFirstBeat = true;
        }

        private void OnMusicPlay(PlayingAudio obj)
        {
            SetBPM(manualBPM, 0f);
            StartBeating();
        }

        private void OnMusicPlay(MusicClipData musicData)
        {
            // Debug.Log($"XXX BPM={BPMAnalyzer.AnalyzeBpm(musicData.AudioClip)}");

            SetBPM(musicData.BPM, musicData.FirstBeatOffset);
            StartBeating();
        }

        public static float BPMToDuration(float bpm)
        {
            if (bpm <= 0)
            {
                return float.MaxValue;
            }

            return 60f / bpm;
        }

        private void Beat()
        {
            if (isBeatDisabled) return;

            ++BeatCount;

            if (BeatCount % beatInBarCount == 0)
            {
                ++BarCount;
                OnBar?.Invoke(BeatCount, BarCount);
            }

            var lastDur = elapsedClipTime - lastBeatTime;
            lastBeatTime = elapsedClipTime;

            //Debug.Log($"[GameAudio] Beat! BeatCount={BeatCount}, BarCount={BarCount}, BeatDuration={BeatDuration}");

            OnBeat?.Invoke(BeatCount, BarCount);
        }

        private IEnumerator BeatRoutine()
        {
            elapsedClipTime = GetElapsedAudioClipTime();
            // clipTime = Time.realtimeSinceStartupAsDouble;
            nextBeatTime = elapsedClipTime + firstBeatOffset;

            while (true)
            {
                elapsedClipTime = GetElapsedAudioClipTime();
                if (elapsedClipTime >= nextBeatTime)
                {
                    Beat();

                    nextBeatTime += BeatDuration;
                }

                yield return null;
            }
        }

        private double GetElapsedAudioClipTime()
        {
            var time = (double)musicPlayerAudioSource.timeSamples / musicPlayerAudioSource.clip.frequency;
            return time;
        }

        public bool IsBeatInNewBar(int beat)
        {
            return beat % beatInBarCount == 0;
        }
    }
}