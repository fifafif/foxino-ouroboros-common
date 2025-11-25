namespace Ouroboros.Common.Game
{
    public interface IGameStateChangeHandler
    {
        void OnGameStateChange(GameController.GameState newState);
    }
}