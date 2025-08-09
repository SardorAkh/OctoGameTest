using System;
using System.Collections.Generic;
using MiniGames.Pairs.Configs;
using MiniGames.Pairs.Gameplay;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MiniGames.Pairs.Factories
{
    public class PairsCardFactory
    {
        public List<PairsCard> CreateCards(PairsConfig config, PairsCard cardPrefab, Transform container,
            Action<PairsCard> onCardClicked)
        {
            var cards = new List<PairsCard>();
            var cardData = GenerateCardData(config.PairsCount);
            ShuffleCardData(cardData);

            for (var i = 0; i < cardData.Count; i++)
            {
                var card = CreateSingleCard(cardPrefab, container, cardData[i], config, onCardClicked);
                cards.Add(card);
            }

            return cards;
        }

        private List<int> GenerateCardData(int pairsCount)
        {
            var cardData = new List<int>();

            for (var i = 0; i < pairsCount; i++)
            {
                cardData.Add(i);
                cardData.Add(i);
            }

            return cardData;
        }

        private void ShuffleCardData(List<int> cardData)
        {
            for (var i = 0; i < cardData.Count; i++)
            {
                var temp = cardData[i];
                var randomIndex = Random.Range(i, cardData.Count);
                cardData[i] = cardData[randomIndex];
                cardData[randomIndex] = temp;
            }
        }

        private PairsCard CreateSingleCard(PairsCard cardPrefab, Transform container, int cardId, PairsConfig config,
            Action<PairsCard> onCardClicked)
        {
            var card = UnityEngine.Object.Instantiate(cardPrefab, container);
            var cardSprite = config.CardSprites[cardId % config.CardSprites.Length];

            card.Initialize(cardId, cardSprite, onCardClicked);

            return card;
        }
    }
}