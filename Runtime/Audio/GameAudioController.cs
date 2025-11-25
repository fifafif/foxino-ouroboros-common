using Ouroboros.Common.Game;
using Ouroboros.Common.Services;
using UnityEngine;

namespace Ouroboros.Common.Audio
{
    public class GameAudioController : MonoBehaviour, IGameStartHandler, IGameFinishHandler
    {
        [AudioClip]
        [SerializeField] private string gameOverAudioId;
        [AudioClip]
        [SerializeField] private string gameStartAudioId;

        private GameController gameController;
        private AudioSource audioSource;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            gameController = ServiceLocator.Get<GameController>();
            gameController.GameStartHandlers.Subscribe(this);
            gameController.GameFinishHandlers.Subscribe(this);
        }

        private void OnDestroy()
        {
            if (gameController != null)
            {
                gameController.GameStartHandlers.Unsubscribe(this);
                gameController.GameFinishHandlers.Unsubscribe(this);
            }
        }

        public void OnGameStart()
        {
            PlaySound(gameStartAudioId);
        }

        public void HandleGameFinish(GameFinishPayload payload)
        {
            PlaySound(gameOverAudioId);
        }

        private void PlaySound(string id)
        {
            if (string.IsNullOrEmpty(id)) return;

            AudioManager.PlaySound(id, audioSource);
        }
    }
}