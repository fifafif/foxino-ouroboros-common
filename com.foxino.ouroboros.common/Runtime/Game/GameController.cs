using Ouroboros.Common.Services;
using Ouroboros.Common.Utils;
using UnityEngine;
using System.Threading.Tasks;
using Ouroboros.Common.Logging;
using Ouroboros.Common.Performance;
using Ouroboros.Common.Observer;
// using Ouroboros.Common.Inputs;

namespace Ouroboros.Common.Game
{
    public class GameController : MonoBehaviour
    {
        public enum GameState
        {
            Waiting,
            Playing,
            Paused,
            Finished
        }

        public Observers<IGameInitHandler> GameInitHandlers { get; private set; }
            = new Observers<IGameInitHandler>();
        public Observers<IGameStartHandler> GameStartHandlers { get; private set; }
            = new Observers<IGameStartHandler>();
        public Observers<IGamePrepareHandler> GamePrepareHandlers { get; private set; }
            = new Observers<IGamePrepareHandler>();
        public Observers<IGamePauseHandler> GamePauseHandlers { get; private set; }
            = new Observers<IGamePauseHandler>();
        public Observers<IGameWillFinishHandler> GameWillFinishHandlers { get; private set; }
            = new Observers<IGameWillFinishHandler>();
        public Observers<IGameFinishHandler> GameFinishHandlers { get; private set; }
            = new Observers<IGameFinishHandler>();
        public Observers<IGameStateChangeHandler> GameStateChangeHandlers { get; private set; }
            = new Observers<IGameStateChangeHandler>();

        public GameState State { get; private set; }
        public bool IsGamePlayingOrPaused =>
            State == GameState.Playing || State == GameState.Paused;
        public bool IsGameEnding { get; private set; }
        public bool IsGameFromRestart { get; private set; }
        public bool IsGameStarting { get; private set; }
        public Task<bool> InitTask => initTcs.Task;
        public GameFinishPayload LastGameFinishPayload { get; private set; }

        [SerializeField] private bool isAutoGameStart;
        [SerializeField] private KeyCode debugGameStart = KeyCode.Space;

        private TaskCompletionSource<bool> initTcs = new TaskCompletionSource<bool>();
        private bool isPauseMenuDisabled;
        private PerformanceTracker performanceTracker;

        private static int startCount;
        private static bool hasQueuedGameStart;

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void Start()
        {
            this.CoroutineWaitFrames(1, () =>
            {
                initTcs.SetResult(true);
                GameInitHandlers.Invoke(o => o.OnGameInit());
            });

            if (isAutoGameStart)
            {
                this.CoroutineWaitFrames(5, () =>
                {
                    StartGame();
                });
            }
        }

// #if UNITY_EDITOR

//         private void Update()
//         {
//             if (State != GameState.Waiting
//                 || IsGameStarting) return;

//             if (InputWrapper.GetKeyDown(debugGameStart))
//             {
//                 StartGame();
//             }
//         }
        
// #endif

        public void TryStartGame()
        {
            if (hasQueuedGameStart)
            {
                State = GameState.Waiting;
                StartGame();
            }
        }

        public void QueueStartGame(bool isRestart = false)
        {
            Logs.Debug<GameController>($"QueueStartGame");

            IsGameFromRestart = isRestart;
            hasQueuedGameStart = true;
        }

        public void RestartGame(string source = null)
        {
            QueueStartGame(true);
        }

        public void SetGameOver(
            GameFinishPayload.LossReason lossReason = GameFinishPayload.LossReason.PlayerDead, 
            string sourceObjectId = null)
        {
            if (State != GameState.Playing
                && State != GameState.Paused)
            {
                return;
            }

            IsGameEnding = true;

            var payload = new GameFinishPayload
            {
                IsWin = false,
                LossReasonType = lossReason,
                SourceObjectId = sourceObjectId
            };

            LastGameFinishPayload = payload;

            GameWillFinishHandlers.Invoke(o => o.HandleGameWillFinish());
            GameFinishHandlers.Invoke(o => o.HandleGameFinish(payload));

            ChangeState(GameState.Finished);
        }

        public void SetGameWin(float delay)
        {
            if (State != GameState.Playing) return;

            IsGameEnding = true;
            GameWillFinishHandlers.Invoke(o => o.HandleGameWillFinish());

            this.CoroutineWait(delay, () =>
            {
                if (State != GameState.Playing) return;

                SetGameWin();
            });
        }

        public void SetGameWin()
        {
            if (State != GameState.Playing) return;

            var payload = new GameFinishPayload
            {
                IsWin = true
            };

            LastGameFinishPayload = payload;
            GameFinishHandlers.Invoke(o => o.HandleGameFinish(payload));

            ChangeState(GameState.Finished);
        }

        public void Pause()
        {
            if (State != GameState.Playing) return;

            GamePauseHandlers.Invoke(o => o.OnGamePause(true));
            ChangeState(GameState.Paused);
        }

        public void Unpause()
        {
            if (State != GameState.Paused)
            {
                return;
            }
            
            GamePauseHandlers.Invoke(o => o.OnGamePause(false));
            ChangeState(GameState.Playing);
        }

        public void StartGame()
        {
            if (State != GameState.Waiting
                && State != GameState.Finished) return;

            Logs.Debug<GameController>($"StartGame!");

            ++startCount;
            hasQueuedGameStart = false;
            IsGameEnding = false;
            IsGameStarting = true;

            GamePrepareHandlers.Invoke(o => o.OnGamePrepare());

            this.CoroutineWaitFrames(2, () =>
            {
                GameStartHandlers.Invoke(o => o.OnGameStart());
                ChangeState(GameState.Playing);
                IsGameStarting = false;
            });
        }

        private void ChangeState(GameState newState)
        {
            State = newState;

            GameStateChangeHandlers.Invoke(o => o.OnGameStateChange(newState));

            Logs.Debug<GameController>($"ChangeState state={State}");
        }
    }
}
