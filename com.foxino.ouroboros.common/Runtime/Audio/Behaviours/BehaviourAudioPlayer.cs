using UnityEngine;

namespace Ouroboros.Common.Audio.Behaviours
{
    public class BehaviourAudioPlayer : MonoBehaviour
    {
        [AudioClip]
        [SerializeField] private string clipId;

        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            PlayAudio();
        }
        
        private void PlayAudio()
        {
            AudioManager.PlaySound(clipId, audioSource);
        }
    }
}