namespace Ouroboros.Common.Game
{
    public interface IGameFinishHandler
    {
        void HandleGameFinish(GameFinishPayload payload);
    }
}