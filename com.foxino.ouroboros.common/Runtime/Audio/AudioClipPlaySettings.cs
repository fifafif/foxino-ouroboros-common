using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace Ouroboros.Common.Audio
{
    [Serializable]
    public class AudioClipPlaySettings
    {
        [Range(0f, 1f)]
        public float Probability = 1f;
        public float Delay;
        [AudioClip]
        public string AudioId;
        public int PlayEveryNthMin;
        public int PlayEveryNthMax;
        public float PlayIntervalMin;
        public float PlayIntervalMax;
        public AudioSource AudioSource;
        public AudioMixerGroup MixerGroup;

        private bool hasAudio;
        private int nextPlayAtAttemptCount;
        private static Dictionary<string, int> playAttemptCountMap = new Dictionary<string, int>();
        private float timeToPlay;

        public void Play()
        {
            if (string.IsNullOrEmpty(AudioId)) return;

            if (nextPlayAtAttemptCount == 0
                && PlayEveryNthMin > 0)
            {
                nextPlayAtAttemptCount = Random.Range(PlayEveryNthMin, PlayEveryNthMax);
            }

            if (playAttemptCountMap.TryGetValue(AudioId, out var playAttemptCount))
            {
                ++playAttemptCount;
            }
            else
            {
                playAttemptCount = 1;
            }

            if (playAttemptCount < nextPlayAtAttemptCount
                || Random.value > Probability)
            {
                playAttemptCountMap[AudioId] = playAttemptCount;
                return;
            }

            playAttemptCountMap[AudioId] = 0;
            nextPlayAtAttemptCount = Random.Range(PlayEveryNthMin, PlayEveryNthMax);
            AudioManager.PlaySound(new PlayAudioPayload(AudioId, AudioSource, MixerGroup, Delay));
        }

        public void Update(float deltaTime)
        {
            if (!hasAudio) return;

            timeToPlay -= deltaTime;
            if (timeToPlay <= 0f)
            {
                SetNextPlayTime();
                AudioManager.PlaySound(new PlayAudioPayload(AudioId, AudioSource, Delay));
            }
        }

        public void InitTimeIntervalPlay()
        {
            hasAudio = !string.IsNullOrEmpty(AudioId);
            if (!hasAudio) return;

            SetNextPlayTime();
        }

        private void SetNextPlayTime()
        {
            timeToPlay = Random.Range(PlayIntervalMin, PlayIntervalMax);
        }
    }
}