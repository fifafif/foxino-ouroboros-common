using System;
using UnityEngine;

namespace Ouroboros.Common.Audio
{
    [Serializable]
    public class AudioData
    {
        public enum TargetType
        {
            AudioSource,
            AudioManager
        }

        [SerializeField] private AudioClip audioClip;
        [SerializeField] private AudioSource audioSource;
        [AudioClip]
        [SerializeField] private string audioId;
        [SerializeField] private TargetType target = TargetType.AudioManager;
        [SerializeField] private bool isLooping;
        [Range(0f, 1f)]
        [SerializeField] private float volume = 1f;
        [SerializeField] private float fadeInTime;
        [SerializeField] private float fadeOutTime;
        [SerializeField] private float delay;

        private int instanceId = -1;

        public void Play()
        {
            if (!Application.isPlaying)
            {
                PlayAudioEditor();
                return;
            }

            if (target == TargetType.AudioManager)
            {
                if (audioClip != null)
                {
                    instanceId = AudioManager.PlaySound(
                        audioClip, audioSource, isLooping, volume, fadeInTime, delay);
                }
                else if (!string.IsNullOrEmpty(audioId))
                {
                    var payload = new PlayAudioPayload(audioId, audioSource, delay);
                    payload.volume = volume;
                    payload.fadeInTime = fadeInTime;
                    payload.loop = isLooping;
                    instanceId = AudioManager.PlaySound(payload);
                }
            }
            else
            {
                Debug.Assert(audioSource != null, "[AudioData] Missing AudioSource!");

                audioSource.clip = audioClip;
                audioSource.loop = isLooping;
                audioSource.Play();
            }
        }

        public void Stop()
        {
            if (!Application.isPlaying)
            {
                StopAudioEditor();
                return;
            }

            if (target == TargetType.AudioManager)
            {
                if (instanceId >= 0)
                {
                    AudioManager.StopSound(instanceId, fadeOutTime);
                    instanceId = -1;
                }
            }
            else
            {
                audioSource.Stop();
            }
        }

        private void PlayAudioEditor()
        {
#if UNITY_EDITOR

            if (audioClip != null)
            {
                AudioEditorPlayer.PlayClip(audioClip, isLooping);
            }
            else
            {
                Debug.LogError($"[AudioData] Cannot preview audio by Id");
            }

#endif
        }


        private void StopAudioEditor()
        {
#if UNITY_EDITOR

            if (audioClip != null)
            {
                AudioEditorPlayer.StopClip(audioClip);
            }

#endif
        }
    }
}
