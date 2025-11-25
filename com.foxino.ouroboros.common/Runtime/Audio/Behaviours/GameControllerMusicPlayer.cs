using Ouroboros.Common.Game;
using Ouroboros.Common.Services;
using UnityEngine;

namespace Ouroboros.Common.Audio
{
    [RequireComponent(typeof(MusicPlayer))]
    public class GameControllerMusicPlayer : MonoBehaviour, IGameStartHandler, IGameFinishHandler
    {
        [SerializeField] private bool isPlayingOnStart;
        
        private MusicPlayer musicPlayer;
        private GameController gameController;

        private void Awake()
        {
            musicPlayer = GetComponent<MusicPlayer>();
        }

        private void Start()
        {
            gameController = ServiceLocator.Get<GameController>();
            gameController.GameStartHandlers.Subscribe(this);
            gameController.GameFinishHandlers.Subscribe(this);

            if (isPlayingOnStart)
            {
                musicPlayer.Play();
            }
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
            musicPlayer.Play();
        }

        public void HandleGameFinish(GameFinishPayload payload)
        {
            musicPlayer.Stop();
        }
    }
}
