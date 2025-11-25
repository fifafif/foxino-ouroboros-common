using System;
using Ouroboros.Common.Logging;
using UnityEngine;

namespace Ouroboros.Common.Audio
{
    public class PlayingAudio
    {
        public bool IsValid => AudioSource != null;
        public Action OnFinish { get; set; }

        public int Id;
        public AudioClip AudioClip;
        public AudioSource AudioSource;
        public float Volume = 1f;
        public float Delay;
        public bool IsLooping;
        public bool IsSingleClip;

        public enum Fade
        {
            None,
            In,
            Out
        }

        public float Time { get { return AudioSource.clip != null ? AudioSource.time : 0; } }

        public Fade FadeMode;

        private float fadeLengthSec;
        private float fadeStartTimeSec;
        private float fadeElapsedTimeSec;
        private bool isLoopNow;
        private bool isPaused;
        private float repeatTime;
        private float playStartTime;
        private bool hasStartedPlaying;

        public PlayingAudio(AudioClip clip, AudioSource source)
        {
            AudioClip = clip;
            AudioSource = source;
            //AudioSource.clip = clip;
        }

        public void Play(float delay = 0f)
        {
            isPaused = false;
            hasStartedPlaying = false;
            Delay = delay;

            // Reset fade state when restarting
            if (FadeMode == Fade.In)
            {
                fadeElapsedTimeSec = 0f;
                AudioSource.volume = 0f;
            }
            else if (FadeMode == Fade.None)
            {
                AudioSource.volume = Volume;
            }

            if (delay <= 0f)
            {
                PlayNow();
                Update(0f);
            }
        }

        private void PlayNow()
        {
            playStartTime = UnityEngine.Time.time;
            hasStartedPlaying = true;

            if (IsLooping
                || IsSingleClip)
            {
                AudioSource.clip = AudioClip;
                AudioSource.Play();
            }
            else
            {
                AudioSource.PlayOneShot(AudioClip, Volume);
            }
        }

        public void FadeIn(float lengthSec = 1)
        {
            fadeLengthSec = lengthSec;
            fadeStartTimeSec = Time;
            fadeElapsedTimeSec = 0f;
            FadeMode = Fade.In;
            AudioSource.volume = 0f; // Start from silence
        }

        public void FadeOut(float lengthSec = 1)
        {
            fadeLengthSec = lengthSec;
            fadeStartTimeSec = Time;
            fadeElapsedTimeSec = 0f;
            FadeMode = Fade.Out;
        }

        public void Update(float deltaTime)
        {
            if (AudioSource == null
                || isPaused) return;

            if (Delay > 0f)
            {
                Delay -= deltaTime;
                if (Delay > 0f) return;

                PlayNow();
            }

            if (AudioSource.clip == null) return;

            if (FadeMode == Fade.In)
            {
                if (fadeLengthSec > 0f)
                {
                    float f = Mathf.Min(fadeElapsedTimeSec / fadeLengthSec, 1f);
                    AudioSource.volume = f * Volume;

                    // Fade complete, set to normal mode
                    if (f >= 1f)
                    {
                        FadeMode = Fade.None;
                    }
                }
            }
            else if (FadeMode == Fade.Out)
            {
                if (fadeLengthSec > 0f)
                {
                    float f = Mathf.Min(fadeElapsedTimeSec / fadeLengthSec, 1f);
                    AudioSource.volume = (1 - f) * Volume;

                    if (f >= 1f)
                    {
                        Stop();
                    }
                }
            }

            fadeElapsedTimeSec += deltaTime;

            float timeNorm = AudioSource.time / AudioClip.length;
            const float loopEnd = 0.9f;

            if (!isLoopNow
                && timeNorm >= loopEnd)
            {
                isLoopNow = true;
                ++repeatTime;
            }
            else if (isLoopNow
                && timeNorm < loopEnd)
            {
                isLoopNow = false;
            }
        }

        public bool IsFinished()
        {
            if (!hasStartedPlaying || Delay > 0f || isPaused)
            {
                return false;
            }

            if (IsLooping)
            {
                return false;
            }

            if (AudioClip != null)
            {
                var elapsedTime = UnityEngine.Time.time - playStartTime;
                var timeExpired = elapsedTime >= AudioClip.length;

                return timeExpired;
            }

            return !AudioSource.isPlaying;
        }

        public void Stop()
        {
            isPaused = false;
            hasStartedPlaying = false;

            // Actually stop the audio source
            AudioSource.Stop();

            // Reset fade state
            FadeMode = Fade.None;
            fadeElapsedTimeSec = 0f;

            // Reset loop tracking
            isLoopNow = false;
            repeatTime = 0f;
        }

        public void Pause()
        {
            isPaused = true;
            AudioSource.Pause();
        }
    }
}
