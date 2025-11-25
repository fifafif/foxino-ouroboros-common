using Ouroboros.Common.Logging;
using UnityEngine;

namespace Ouroboros.Common.Audio.Behaviours
{
    public class CollisionAudioPlayer : MonoBehaviour
    {
        [AudioClip]
        [SerializeField] private string clipId;

        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void OnTriggerEnter(Collider other)
        {
            Logs.Debug<AudioManager>($"[CollisionAudioPlayer] Collision other={other.gameObject}, this={this}", this);
            PlayAudio();
        }

        private void OnCollisionEnter(Collision collision)
        {
            Logs.Debug<AudioManager>($"[CollisionAudioPlayer] Collision other={collision.gameObject}, this={this}", this);
            PlayAudio();
        }

        private void PlayAudio()
        {
            AudioManager.PlaySound(clipId, audioSource);
        }
    }
}