using UnityEngine;
using UnityEngine.Audio;

namespace Ouroboros.Common.Audio
{
    public struct PlayAudioPayload
    {
        public string id;
        public AudioClip clip;
        public AudioSource source;
        public AudioMixerGroup mixerGroup;
        public bool loop;
        public float volume;
        public float fadeInTime;
        public float delay;
        public float arrayNormalizedIndex;
        public bool isUsingArrayIndex;

        public PlayAudioPayload(string id) : this(id, null, 0f)
        {
        }

        public PlayAudioPayload(string id, AudioSource source, float delay) : this(id, source, null, false, 1f, 0f, delay)
        {

        }

        public PlayAudioPayload(string id, AudioMixerGroup mixerGroup) : this(id, null, mixerGroup, false, 1f, 0f, 0f)
        {

        }

        public PlayAudioPayload(string id, AudioSource audioSource, AudioMixerGroup mixerGroup) : this(id, audioSource, mixerGroup, false, 1f, 0f, 0f)
        {

        }

        public PlayAudioPayload(string id, AudioSource audioSource, AudioMixerGroup mixerGroup, float delay) : this(id, audioSource, mixerGroup, false, 1f, 0f, delay)
        {

        }

        public PlayAudioPayload(string id, AudioSource source, AudioMixerGroup mixerGroup, bool loop, float volume, float fadeInTime, float delay)
        {
            this.id = id;
            this.clip = null;
            this.source = source;
            this.loop = loop;
            this.volume = volume;
            this.fadeInTime = fadeInTime;
            this.delay = delay;
            arrayNormalizedIndex = 0;
            isUsingArrayIndex = false;
            this.mixerGroup = mixerGroup;
        }

        public PlayAudioPayload(AudioClip clip, AudioSource source, AudioMixerGroup mixerGroup, bool loop, float volume, float fadeInTime, float delay)
        {
            id = null;
            this.clip = clip;
            this.source = source;
            this.loop = loop;
            this.volume = volume;
            this.fadeInTime = fadeInTime;
            this.delay = delay;
            arrayNormalizedIndex = 0;
            isUsingArrayIndex = false;
            this.mixerGroup = mixerGroup;
        }
    }
}