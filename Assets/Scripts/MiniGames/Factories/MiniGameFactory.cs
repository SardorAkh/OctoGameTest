using CustomConfigurations;
using MiniGames.Interfaces;
using Naninovel;
using Unity.VisualScripting;
using UnityEngine;

namespace MiniGames.Factories
{
    public class MiniGameFactory
    {
        public GameObject CreateMiniGame(MiniGameData gameData)
        {
            if (gameData.MiniGamePrefab == null)
            {
                Debug.LogError($"MiniGame prefab not set for game '{gameData.GameId}'");
                return null;
            }

            var gameObject = Object.Instantiate(gameData.MiniGamePrefab);

            var gameController = gameObject.GetComponent<IMiniGame>();
            if (gameController == null)
            {
                Debug.LogError($"No IMiniGame component found on prefab for game '{gameData.GameId}'");
                Object.Destroy(gameObject);
                return null;
            }

            return gameObject;
        }
        
    }
}