using System.Threading.Tasks;

namespace Ouroboros.Common.Game
{
    public interface IGameInitializable
    {
        Task Initialize();
        void StartGame();
    }
}