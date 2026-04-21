using Ouroboros.Common.Audio;
using UnityEngine;

namespace Ouroboros.Common.UI.Windows
{
    [RequireComponent(typeof(Window))]
    public class WindowAudio : MonoBehaviour
    {
        [SerializeField] private AudioData openAudio;
        [SerializeField] private bool isOpenAudioStopOnClose;
        [SerializeField] private AudioData closeAudio;

        private void Awake()
        {
            var window = GetComponent<Window>();
            window.OnOpenAction += OnWindowOpen;
            window.OnCloseAction += OnWindowClose;
        }

        private void OnWindowClose()
        {
            if (isOpenAudioStopOnClose)
            {
                openAudio.Stop();
            }

            closeAudio.Play();
        }

        private void OnWindowOpen()
        {
            openAudio.Play();
        }
    }
}