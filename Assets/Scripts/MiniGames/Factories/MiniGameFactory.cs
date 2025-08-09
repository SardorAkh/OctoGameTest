using CustomConfigurations;
using MiniGames.Interfaces;
using Naninovel;
using Unity.VisualScripting;
using UnityEngine;

namespace MiniGames.Factories
{
    public class MiniGameFactory
    {
        private readonly ICameraManager cameraManager;

        public MiniGameFactory(ICameraManager cameraManager)
        {
            this.cameraManager = cameraManager;
        }

        public GameObject CreateMiniGame(MiniGameData gameData)
        {
            if (gameData.MiniGamePrefab == null)
            {
                Debug.LogError($"UI prefab not set for game '{gameData.GameId}'");
                return null;
            }

            var gameObject = Object.Instantiate(gameData.MiniGamePrefab);

            SetupCanvas(gameObject);

            var gameController = gameObject.GetComponent<IMiniGame>();
            if (gameController == null)
            {
                Debug.LogError($"No IMiniGame component found on prefab for game '{gameData.GameId}'");
                Object.Destroy(gameObject);
                return null;
            }

            return gameObject;
        }

        private void SetupCanvas(GameObject gameObject)
        {
            var canvas = gameObject.GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = cameraManager.Camera;
                canvas.sortingOrder = 1000;
            }
        }
    }
}