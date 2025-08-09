using System;
using UnityEngine;
using UnityEngine.UI;

namespace MiniGames.Pairs.Gameplay
{
    public class PairsCard : MonoBehaviour
    {
        [Header("Card UI")] [SerializeField] private Button cardButton;
        [SerializeField] private Image cardImage;
        [SerializeField] private Image backImage;

        public int CardId { get; private set; }
        public bool IsRevealed { get; private set; }
        public bool IsMatched { get; private set; }

        private Action<PairsCard> _onCardClicked;

        public void Initialize(int cardId, Sprite cardSprite, Action<PairsCard> onClicked)
        {
            CardId = cardId;
            _onCardClicked = onClicked;
            cardImage.sprite = cardSprite;
            cardButton.onClick.AddListener(OnClick);

            Hide();
        }

        private void OnClick()
        {
            if (!IsRevealed && !IsMatched)
            {
                _onCardClicked?.Invoke(this);
            }
        }

        public void Reveal()
        {
            IsRevealed = true;
            cardImage.gameObject.SetActive(true);
            backImage.gameObject.SetActive(false);
        }

        public void Hide()
        {
            IsRevealed = false;
            cardImage.gameObject.SetActive(false);
            backImage.gameObject.SetActive(true);
        }

        public void SetMatched()
        {
            IsMatched = true;
            IsRevealed = true;

            cardButton.interactable = false;
            var color = cardImage.color;
            color.a = 0.7f;
            cardImage.color = color;
        }
    }
}