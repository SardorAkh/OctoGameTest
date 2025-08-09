using System;
using System.Collections.Generic;
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

        [Header("UI Elements")] [SerializeField]
        private Transform cardContainer;

        [SerializeField] private PairsCard cardPrefab;
        [SerializeField] private Button closeButton;
        [SerializeField] private PairsConfig pairsConfig;

        private List<PairsCard> _cards = new();
        private PairsCard _firstCard;
        private PairsCard _secondCard;
        private int _foundPairs;
        private bool _gameActive;

        public void StartGame()
        {
            _gameActive = true;
            _foundPairs = 0;
            CreateCards();

            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(StopGame);

            gameObject.SetActive(true);
        }

        public void StopGame()
        {
            if (!_gameActive) return;

            _gameActive = false;
            gameObject.SetActive(false);
            ClearCards();

            OnGameFinished?.Invoke(false);
        }

        private void EndGame(bool won)
        {
            if (!_gameActive) return;

            _gameActive = false;
            gameObject.SetActive(false);
            ClearCards();

            OnGameFinished?.Invoke(won);
        }

        private void ClearCards()
        {
            foreach (var card in _cards)
            {
                if (card != null) Destroy(card.gameObject);
            }

            _cards.Clear();
        }

        private void CreateCards()
        {
            var factory = new PairsCardFactory();
            _cards = factory.CreateCards(
                pairsConfig,
                cardPrefab,
                cardContainer,
                (pairsCard) => { OnCardClicked(pairsCard).Forget(); }
            );

            // var cardData = new List<int>();
            //
            // for (var i = 0; i < pairsConfig.PairsCount; i++)
            // {
            //     cardData.Add(i);
            //     cardData.Add(i);
            // }
            //
            // for (var i = 0; i < cardData.Count; i++)
            // {
            //     var temp = cardData[i];
            //     var randomIndex = Random.Range(i, cardData.Count);
            //     cardData[i] = cardData[randomIndex];
            //     cardData[randomIndex] = temp;
            // }
            //
            // for (var i = 0; i < cardData.Count; i++)
            // {
            //     var card = Instantiate(cardPrefab, cardContainer);
            //
            //     var cardSprite = pairsConfig.CardSprites[cardData[i] % pairsConfig.CardSprites.Length];
            //
            //     card.Initialize(cardData[i], cardSprite, (pairsCard) => { OnCardClicked(pairsCard).Forget(); });
            //     _cards.Add(card);
            // }
        }

        private async UniTask OnCardClicked(PairsCard card)
        {
            if (!_gameActive || card.IsRevealed) return;

            card.Reveal();

            if (_firstCard == null)
            {
                _firstCard = card;
            }
            else if (_secondCard == null)

            {
                _secondCard = card;

                await UniTask.Delay(1000);

                if (_firstCard.CardId == _secondCard.CardId)
                {
                    _firstCard.SetMatched();
                    _secondCard.SetMatched();
                    _foundPairs++;

                    if (_foundPairs >= pairsConfig.PairsCount)
                    {
                        EndGame(true);
                    }
                }
                else
                {
                    _firstCard.Hide();
                    _secondCard.Hide();
                }

                _firstCard = null;
                _secondCard = null;
            }
        }
    }
}