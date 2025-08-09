using System.Linq;
using CustomConfigurations;
using MiniGames.Factories;
using MiniGames.Interfaces;
using Naninovel;
using UnityEngine;

namespace CustomServices
{
    public class MiniGameService : IEngineService
    {
        private readonly MiniGameConfiguration _miniGameConfiguration;
        private readonly MiniGameFactory _miniGameFactory;
        private readonly ICameraManager _cameraManager;
        private GameObject _currentGame;
        private bool _isGameActive = false;

        public MiniGameService(
            MiniGameConfiguration miniGameConfiguration,
            ICameraManager cameraManager
        )
        {
            _miniGameConfiguration = miniGameConfiguration;
            _cameraManager = cameraManager;
            _miniGameFactory = new(_cameraManager);
        }

        public async UniTask InitializeServiceAsync()
        {
            await UniTask.CompletedTask;
        }

        public void ResetService()
        {
            StopCurrentGame();
        }

        public void DestroyService()
        {
            StopCurrentGame();
        }

        public async UniTask<bool> StartGameAsync(string gameId)
        {
            if (_isGameActive)
            {
                Debug.LogWarning("Game already active!");
                return false;
            }

            var gameData = _miniGameConfiguration.miniGames.FirstOrDefault(game => game.GameId == gameId);

            if (gameData == null)
            {
                Debug.LogError($"There's not a such game with id {gameId}");
                return false;
            }

            _currentGame = _miniGameFactory.CreateMiniGame(gameData);

            if (_currentGame == null)
            {
                return false;
            }

            var gameController = _currentGame.GetComponent<IMiniGame>();

            _isGameActive = true;

            return await gameController.StartGame();
        }

        private void StopCurrentGame()
        {
            if (_currentGame != null)
            {
                Object.Destroy(_currentGame);
                _currentGame = null;
            }

            _isGameActive = false;
        }
    }
}