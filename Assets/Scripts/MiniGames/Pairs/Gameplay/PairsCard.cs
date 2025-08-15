using System;
using DG.Tweening;
using Naninovel;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace MiniGames.Pairs.Gameplay
{
    public class PairsCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Card UI")] [SerializeField] private Button cardButton;
        [SerializeField] private Image cardImage;
        [SerializeField] private Image backImage;
        [SerializeField] private Image cardFrame;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Animation Settings")] [SerializeField]
        private float flipDuration = 0.3f; // Уменьшено с 0.6f до 0.3f

        [SerializeField] private float hoverScale = 1.1f;
        [SerializeField] private float hoverDuration = 0.1f; // Уменьшено с 0.2f до 0.1f
        [SerializeField] private Color matchedFrameColor = Color.green;
        [SerializeField] private Color defaultFrameColor = Color.white;

        public int CardId { get; private set; }
        public bool IsRevealed { get; private set; }
        public bool IsMatched { get; private set; }

        private Action<PairsCard> _onCardClicked;
        private Vector3 _originalScale;
        private Vector3 _originalPosition;
        private bool _isAnimating;
        private Sequence _currentSequence;

        private void Awake()
        {
            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();

            _originalScale = cardFrame.transform.localScale;
            _originalPosition = cardFrame.transform.localPosition;
        }

        public void Initialize(int cardId, Sprite cardSprite, Action<PairsCard> onClicked)
        {
            CardId = cardId;
            _onCardClicked = onClicked;
            cardImage.sprite = cardSprite;
            cardButton.onClick.AddListener(OnClick);

            if (cardFrame != null)
                cardFrame.color = defaultFrameColor;

            Hide();
        }

        private void OnClick()
        {
            if (!IsRevealed && !IsMatched && !_isAnimating)
            {
                _onCardClicked?.Invoke(this);
            }
        }

        public async UniTask RevealAsync()
        {
            if (IsRevealed || _isAnimating) return;

            _isAnimating = true;
            IsRevealed = true;

            // Анимация переворачивания карты
            _currentSequence?.Kill();
            _currentSequence = DOTween.Sequence();
            // Первая половина - поворот до 90 градусов

            _currentSequence
                .Append(transform.DOLocalRotate(Vector3.up * 90f, flipDuration * 0.5f).SetEase(Ease.InQuart))
                .AppendCallback(() =>
                {
                    // Меняем спрайты в середине анимации
                    cardImage.gameObject.SetActive(true);
                    backImage.gameObject.SetActive(false);
                })
                // Вторая половина - поворот обратно
                .Append(transform.DOLocalRotate(Vector3.up * 0, flipDuration * 0.5f).SetEase(Ease.OutQuart))
                // Добавляем небольшой bounce эффект
                .Join(transform.DOPunchScale(Vector3.one * 0.1f, flipDuration * 0.2f, 5, 0.5f) // Уменьшено с 0.3f до 0.2f
                    .SetDelay(flipDuration * 0.6f)); // Уменьшено с 0.7f до 0.6f

            await _currentSequence.AsyncWaitForCompletion();
            _isAnimating = false;
        }

        public async UniTask HideAsync()
        {
            if (!IsRevealed || _isAnimating) return;

            _isAnimating = true;
            IsRevealed = false;

            _currentSequence?.Kill();
            _currentSequence = DOTween.Sequence();

            // Анимация переворачивания обратно
            _currentSequence
                .Append(transform.DOLocalRotate(Vector3.up * -90f, flipDuration * 0.5f).SetEase(Ease.InQuart))
                .AppendCallback(() =>
                {
                    cardImage.gameObject.SetActive(false);
                    backImage.gameObject.SetActive(true);
                })
                .Append(transform.DOLocalRotate(Vector3.up * 0f, flipDuration * 0.5f).SetEase(Ease.OutQuart));

            await _currentSequence.AsyncWaitForCompletion();
            _isAnimating = false;
            OnPointerExit(null);
        }

        public void Reveal()
        {
            RevealAsync().Forget();
        }

        public void Hide()
        {
            if (IsRevealed)
            {
                HideAsync().Forget();
            }
            else
            {
                // Мгновенное скрытие для инициализации
                IsRevealed = false;
                cardImage.gameObject.SetActive(false);
                backImage.gameObject.SetActive(true);
            }
        }

        public async UniTask SetMatchedAsync()
        {
            if (IsMatched) return;

            IsMatched = true;
            IsRevealed = true;
            cardButton.interactable = false;

            // Анимация успешного совпадения
            _currentSequence?.Kill();
            _currentSequence = DOTween.Sequence();

            // Эффект "пульсации" и изменения цвета
            _currentSequence
                .Append(cardFrame.transform.DOScale(_originalScale * 1.2f, 0.1f).SetEase(Ease.OutQuad)) // Уменьшено с 0.2f до 0.1f
                .Join(cardFrame != null ? cardFrame.DOColor(matchedFrameColor, 0.1f) : null) // Уменьшено с 0.2f до 0.1f
                .Append(cardFrame.transform.DOScale(_originalScale, 0.15f).SetEase(Ease.OutBounce)); // Уменьшено с 0.3f до 0.15f

            // Добавляем эффект затухания
            if (canvasGroup != null)
            {
                _currentSequence.Join(canvasGroup.DOFade(0.8f, 0.2f).SetEase(Ease.OutQuad)); // Уменьшено с 0.4f до 0.2f
            }
            else
            {
                var color = cardImage.color;
                color.a = 0.8f;
                _currentSequence.Join(cardImage.DOColor(color, 0.2f)); // Уменьшено с 0.4f до 0.2f
            }

            await _currentSequence.AsyncWaitForCompletion();
        }

        public void SetMatched()
        {
            SetMatchedAsync().Forget();
        }

        public async UniTask PlayMismatchAnimationAsync()
        {
            if (_isAnimating) return;

            _isAnimating = true;

            // Shake анимация для неправильного выбора
            _currentSequence?.Kill();
            _currentSequence = DOTween.Sequence();

            _currentSequence
                .Append(cardFrame.transform.DOShakePosition(0.25f, 10f, 20, 90f, false, true)) // Уменьшено с 0.5f до 0.25f
                .Join(cardFrame != null ? cardFrame.DOColor(Color.red, 0.05f).SetLoops(2, LoopType.Yoyo) : null); // Уменьшено с 0.1f до 0.05f

            await _currentSequence.AsyncWaitForCompletion();
            _isAnimating = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (IsMatched || _isAnimating) return;

            // Hover эффект как в Balatro - карта поднимается и увеличивается
            _currentSequence?.Kill();
            _currentSequence = DOTween.Sequence();

            var hoverPosition = _originalPosition + Vector3.up * 20f;
            var hoverScaleVector = _originalScale * hoverScale;

            _currentSequence
                .Append(cardFrame.transform.DOLocalMove(hoverPosition, hoverDuration).SetEase(Ease.OutQuart))
                .Join(cardFrame.transform.DOScale(hoverScaleVector, hoverDuration).SetEase(Ease.OutQuart));

            // Добавляем тонкое свечение если есть frame
            if (cardFrame != null)
            {
                _currentSequence.Join(cardFrame.DOColor(Color.yellow, hoverDuration * 0.5f).SetEase(Ease.OutQuad));
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (IsMatched || _isAnimating) return;

            // Возвращаем карту в исходное состояние
            _currentSequence?.Kill();
            _currentSequence = DOTween.Sequence();

            _currentSequence
                .Append(cardFrame.transform.DOLocalMove(_originalPosition, hoverDuration).SetEase(Ease.OutQuart))
                .Join(cardFrame.transform.DOScale(_originalScale, hoverDuration).SetEase(Ease.OutQuart));

            if (cardFrame != null)
            {
                _currentSequence.Join(cardFrame.DOColor(defaultFrameColor, hoverDuration).SetEase(Ease.OutQuad));
            }
        }

        private void OnDisable()
        {
            // Останавливаем анимации при отключении
            _currentSequence?.Kill();
            cardFrame.transform.DOKill();
        }

        private void OnDestroy()
        {
            _currentSequence?.Kill();
            DOTween.Kill(this);
        }
    }
}