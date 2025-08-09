using System;
using System.Threading.Tasks;
using MiniGames.Interfaces;
using Naninovel;
using UnityEngine;

namespace MiniGames.Pairs
{
    public class PairsMiniGame : MonoBehaviour, IMiniGame
    {
        public void StartGame()
        {
            throw new NotImplementedException();
        }

        public void StopGame()
        {
            throw new NotImplementedException();
        }

        public event Action<bool> OnGameFinished;
    }
}