using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Ouroboros.Common.Audio
{
    [RequireComponent(typeof(Button))]
    public class ButtonAudio : MonoBehaviour
    {
        [AudioClip]
        [SerializeField] private string clickAudioId;
        [SerializeField] private AudioMixerGroup mixerGroup;

        private AudioSource audioSource;

        private void Awake()
        {
            TryGetComponent(out audioSource);

            var button = GetComponent<Button>();
            button.onClick.AddListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            AudioManager.PlaySound(new PlayAudioPayload
            {
                id = clickAudioId,
                source = audioSource,
                mixerGroup = mixerGroup,
                volume = 1
            });
        }
    }
}