using System.Threading.Tasks;
using MiniGames.Interfaces;
using Naninovel;
using UnityEngine;

namespace MiniGames.Pairs
{
    public class PairsMiniGame : MonoBehaviour, IMiniGame
    {
        public async UniTask<bool> StartGame()
        {
            await UniTask.CompletedTask;

            return true;
        }
    }
}