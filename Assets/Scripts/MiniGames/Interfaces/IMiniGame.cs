using Naninovel;

namespace MiniGames.Interfaces
{
    public interface IMiniGame
    {
        UniTask<bool> StartGame();
    }
}