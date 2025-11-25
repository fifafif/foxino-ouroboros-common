using Ouroboros.Common.Services;
using Ouroboros.Common.Utils;
using UnityEngine;

namespace Ouroboros.Common.Audio
{
    public class MusicTrigger : MonoBehaviour
    {
        [SerializeField] private string musicName;
        [SerializeField] private bool isPlayingOnEnable;
        [SerializeField] private bool isStoppingOnDestroy;
        [SerializeField] private bool isResettingToPreviousOnDisable;
        [SerializeField] private float delay;

        private bool hasStarted;
        private MusicPlayer musicPlayer;
        private bool isPendingEnablePlay;
        private MusicClipData currentMusic;

        private void Start()
        {
            musicPlayer = ServiceLocator.Get<MusicPlayer>();
            hasStarted = true;

            if (isPlayingOnEnable)
            {
                TriggerPlay();
            }
        }

        private void TriggerPlay()
        {
            if (delay <= 0f)
            {
                Play();
            }
            else
            {
                this.CoroutineWait(delay, Play);
            }
        }

        private void OnEnable()
        {
            if (!hasStarted) return;

            TriggerPlay();
        }

        private void OnDestroy()
        {
            if (!isStoppingOnDestroy) return;

            Stop();
        }

        public void Play()
        {
            currentMusic = musicPlayer.GetCurrentMusic();
            if (currentMusic.Name == musicName) return;

            musicPlayer.Play(musicName);
        }

        public void Stop()
        {
            StopAllCoroutines();

            if (isResettingToPreviousOnDisable
                && currentMusic != null)
            {
                musicPlayer.Play(currentMusic);
            }
            else
            {
                musicPlayer.Stop();
            }
        }
    }
}
