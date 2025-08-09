using System.Threading;
using MiniGames.Pairs.Configs;
using Naninovel;

namespace MiniGames.Pairs.Interfaces
{
    public interface IPairsGameManager
    {
        void Initialize(PairsConfig pairsConfig);
        UniTask<bool> PlayGameAsync(CancellationToken cancellationToken = default);
    }
}