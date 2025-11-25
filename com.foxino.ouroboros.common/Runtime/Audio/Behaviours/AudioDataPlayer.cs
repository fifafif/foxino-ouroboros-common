using UnityEngine;

namespace Ouroboros.Common.Audio.Behaviours
{
    public class AudioDataPlayer : MonoBehaviour
    {
        [SerializeField] private AudioData audioData;

        [Header("Play Control")]
        [SerializeField] private bool isPlayOnStart = true;
        [SerializeField] private bool isPlayOnEnable = true;
        [SerializeField] private bool isStopOnDisable = true;

        private bool hasStarted;

        private void Start()
        {
            hasStarted = true;

            if (isPlayOnStart
                || isPlayOnEnable)
            {
                PlayAudio();            
            }
        }

        private void OnEnable()
        {
            if (isPlayOnEnable
                && hasStarted)
            {
                PlayAudio();
            }
        }

        private void OnDisable()
        {
            if (isStopOnDisable)
            {
                StopAudio();
            }
        }

        private void OnDestroy()
        {
            if (isStopOnDisable)
            {
                StopAudio();
            }
        }

        private void StopAudio()
        {
            audioData.Stop();
        }

        private void PlayAudio()
        {
            audioData.Play();
        }
    }
}