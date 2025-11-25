using UnityEngine;

namespace Ouroboros.Common.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class CollisionAudioSource : MonoBehaviour
    {
        [SerializeField] private AudioClip clip;

        private AudioSource source;

        private void Awake()
        {
            source = GetComponent<AudioSource>();
        }

        private void OnTriggerEnter(Collider other)
        {
            PlayAudio();
        }

        private void OnCollisionEnter(Collision collision)
        {
            PlayAudio();
        }

        private void PlayAudio()
        {
            source.clip = clip;
            source.Play();
        }
    }
}