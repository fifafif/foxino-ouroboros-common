using UnityEngine;

namespace Ouroboros.Common.Audio.Behaviours
{
    public class ManualAudioPlayer : MonoBehaviour
    {
        [AudioClip]
        [SerializeField] private string clipId;

        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void PlayAudio()
        {
            AudioManager.PlaySound(clipId, audioSource);
        }
    }
}