using System;
using System.Collections.Generic;
using DG.Tweening;
using MiniGames.Interfaces;
using MiniGames.Pairs.Configs;
using MiniGames.Pairs.Factories;
using Naninovel;
using UnityEngine;
using UnityEngine.UI;

namespace MiniGames.Pairs.Gameplay
{
    public class PairsGameManager : MonoBehaviour, IMiniGame
    {
        public event Action<bool> OnGameFinished;

        [Header("UI Elements")] 
        [SerializeField] private Transform cardContainer;
        [SerializeField] private PairsCard cardPrefab;
        [SerializeField] private Button closeButton;
        [SerializeField] private PairsConfig pairsConfig;
        
        [Header("Animation Settings")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform gamePanel;
        [SerializeField] private float showDuration = 0.5f;
        [SerializeField] private float hideDuration = 0.3f;
        [SerializeField] private float cardSpawnDelay = 0.1f;

        private List<PairsCard> _cards = new();
        private PairsCard _firstCard;
        private PairsCard _secondCard;
        private int _foundPairs;
        private bool _gameActive;
        private bool _isProcessingPair = false;
        private Sequence _currentSequence;

        private void Start()
        {
            canvasGroup.alpha = 0;
            canvasGroup.DOFade(1, 2f);
        }

        public async void StartGame()
        {
            _gameActive = true;
            _foundPairs = 0;
            
            await ShowGameAsync();
            await CreateCardsWithAnimation();

            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(() => StopGameAsync().Forget());
        }

        public async void StopGame()
        {
            await StopGameAsync();
        }

        private async UniTask StopGameAsync()
        {
            if (!_gameActive) return;

            _gameActive = false;
            
            await HideGameAsync();
            ClearCards();

            OnGameFinished?.Invoke(false);
        }

        private async void EndGame(bool won)
        {
            if (!_gameActive) return;

            _gameActive = false;
            
            if (won)
            {
                await PlayWinAnimation();
            }
            
            await UniTask.Delay(500);
            await HideGameAsync();
            ClearCards();

            OnGameFinished?.Invoke(won);
        }

        private async UniTask ShowGameAsync()
        {
            gameObject.SetActive(true);
            
            // Сбрасываем состояние для анимации
            canvasGroup.alpha = 0f;
            gamePanel.localScale = Vector3.one * 0.8f;
            
            _currentSequence?.Kill();
            _currentSequence = DOTween.Sequence();
            
            _currentSequence
                .Append(canvasGroup.DOFade(1f, showDuration).SetEase(Ease.OutQuart))
                .Join(gamePanel.DOScale(Vector3.one, showDuration).SetEase(Ease.OutBack, 1.2f));
                
            await _currentSequence.AsyncWaitForCompletion();
        }

        private async UniTask HideGameAsync()
        {
            _currentSequence?.Kill();
            _currentSequence = DOTween.Sequence();
            
            _currentSequence
                .Append(gamePanel.DOScale(Vector3.one * 0.8f, hideDuration).SetEase(Ease.InBack, 1.2f))
                .Join(canvasGroup.DOFade(0f, hideDuration).SetEase(Ease.InQuart));
                
            await _currentSequence.AsyncWaitForCompletion();
            gameObject.SetActive(false);
        }

        private async UniTask PlayWinAnimation()
        {
            // Анимация победы - карты подпрыгивают
            var winSequence = DOTween.Sequence();
            
            foreach (var card in _cards)
            {
                if (card != null && card.IsMatched)
                {
                    winSequence.Join(card.transform.DOPunchScale(Vector3.one * 0.2f, 0.5f, 5, 0.5f));
                }
            }
            
            // Добавляем эффект свечения всего экрана
            winSequence.Join(canvasGroup.DOFade(1.2f, 0.2f).SetEase(Ease.OutQuad)
                .SetLoops(2, LoopType.Yoyo));
                
            await winSequence.AsyncWaitForCompletion();
        }

        private void ClearCards()
        {
            _currentSequence?.Kill();
            
            foreach (var card in _cards)
            {
                if (card != null) 
                {
                    card.transform.DOKill();
                    Destroy(card.gameObject);
                }
            }

            _cards.Clear();
        }

        private async UniTask CreateCardsWithAnimation()
        {
            var factory = new PairsCardFactory();
            _cards = factory.CreateCards(
                pairsConfig,
                cardPrefab,
                cardContainer,
                (pairsCard) => { OnCardClicked(pairsCard).Forget(); }
            );

            // Анимируем появление карт с задержкой
            foreach (var card in _cards)
            {
                card.transform.localScale = Vector3.zero;
                card.GetComponent<CanvasGroup>()?.DOFade(0f, 0f);
            }

            for (int i = 0; i < _cards.Count; i++)
            {
                var card = _cards[i];
                var delay = i * cardSpawnDelay;
                
                card.transform.DOScale(Vector3.one, 0.4f)
                    .SetDelay(delay)
                    .SetEase(Ease.OutBack, 1.5f);
                    
                card.GetComponent<CanvasGroup>()?.DOFade(1f, 0.3f)
                    .SetDelay(delay)
                    .SetEase(Ease.OutQuad);
            }

            await UniTask.Delay((int)((_cards.Count * cardSpawnDelay + 0.4f) * 1000));
        }

        private async UniTask OnCardClicked(PairsCard card)
        {
            if (!_gameActive || card.IsRevealed || _isProcessingPair) return;

            await card.RevealAsync();

            if (_firstCard == null)
            {
                _firstCard = card;
            }
            else if (_secondCard == null)
            {
                _secondCard = card;
                _isProcessingPair = true;

                await UniTask.Delay(200);

                if (_firstCard.CardId == _secondCard.CardId)
                {
                    // Совпадение - играем анимацию успеха
                    await UniTask.WhenAll(
                        _firstCard.SetMatchedAsync(),
                        _secondCard.SetMatchedAsync()
                    );
                    
                    _foundPairs++;

                    if (_foundPairs >= pairsConfig.PairsCount)
                    {
                        EndGame(true);
                    }
                }
                else
                {
                    // Не совпадение - shake и скрываем
                    await UniTask.WhenAll(
                        _firstCard.PlayMismatchAnimationAsync(),
                        _secondCard.PlayMismatchAnimationAsync()
                    );
                    
                    await UniTask.WhenAll(
                        _firstCard.HideAsync(),
                        _secondCard.HideAsync()
                    );
                }

                _firstCard = null;
                _secondCard = null;
                _isProcessingPair = false;
            }
        }

        private void OnDestroy()
        {
            _currentSequence?.Kill();
            DOTween.Kill(this);
        }
    }
}