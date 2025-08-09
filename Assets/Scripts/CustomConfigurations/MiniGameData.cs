using System;
using MiniGames.Interfaces;
using UnityEngine;

namespace CustomConfigurations
{
    [Serializable]
    public class MiniGameData
    {
        public string GameId;

        public string DisplayName;

        public GameObject MiniGamePrefab;
    }
}