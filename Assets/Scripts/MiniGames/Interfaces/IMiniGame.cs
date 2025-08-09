using System;
using Naninovel;

namespace MiniGames.Interfaces
{
    public interface IMiniGame
    {
        void StartGame();
        void StopGame();

        event Action<bool> OnGameFinished;
    }
}