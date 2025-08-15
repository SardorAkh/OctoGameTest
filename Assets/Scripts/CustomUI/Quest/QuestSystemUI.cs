using System.Collections.Generic;
using System.Linq;
using CustomConfigurations.Quest;
using DG.Tweening;
using UnityEngine;
using Naninovel;

namespace CustomUI.Quest
{
    public class QuestSystemUI : Naninovel.UI.CustomUI
    {
        [SerializeField] private Transform questsParent;
        [SerializeField] private QuestView questViewPrefab;
        
        [Header("Animation Settings")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform uiPanel;
        [SerializeField] private float showDuration = 0.4f;
        [SerializeField] private float hideDuration = 0.3f;
        [SerializeField] private float questSpawnDelay = 0.1f;
        [SerializeField] private bool slideFromSide = true;
        [SerializeField] private float slideDistance = 300f;

        private List<QuestView> _currentQuests = new();
        private Sequence _mainSequence;
        private bool _isVisible = false;

        protected override void Awake()
        {
            base.Awake();
            
            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();
                
            if (uiPanel == null)
                uiPanel = GetComponent<RectTransform>();
                
            // Скрываем UI изначально
            SetInitialHiddenState();
            
        }

        private void SetInitialHiddenState()
        {
            if (canvasGroup != null)
                canvasGroup.alpha = 0f;
                
            if (slideFromSide && uiPanel != null)
            {
                var pos = uiPanel.anchoredPosition;
                pos.x -= slideDistance;
                uiPanel.anchoredPosition = pos;
            }
            else if (uiPanel != null)
            {
                uiPanel.localScale = Vector3.zero;
            }
        }

        public void OnShow()
        {
            if (_isVisible) return;
            
            ShowAnimated().Forget();
        }

        public  void OnHide()
        {
            if (!_isVisible) return;
            
            HideAnimated().Forget();
        }

        private async UniTaskVoid ShowAnimated()
        {
            _isVisible = true;
            gameObject.SetActive(true);

            _mainSequence?.Kill();
            _mainSequence = DOTween.Sequence();

            if (slideFromSide)
            {
                // Анимация выезда сбоку
                var targetPos = uiPanel.anchoredPosition;
                targetPos.x += slideDistance;
                
                _mainSequence
                    .Append(uiPanel.DOAnchorPosX(targetPos.x, showDuration).SetEase(Ease.OutBack, 1.2f))
                    .Join(canvasGroup.DOFade(1f, showDuration * 0.8f).SetEase(Ease.OutQuad));
            }
            else
            {
                // Анимация масштабирования
                _mainSequence
                    .Append(uiPanel.DOScale(Vector3.one, showDuration).SetEase(Ease.OutBack, 1.5f))
                    .Join(canvasGroup.DOFade(1f, showDuration * 0.8f).SetEase(Ease.OutQuad));
            }

            await _mainSequence.AsyncWaitForCompletion();
            
            // Вызываем базовый Show после анимации
            base.Show();
        }

        private async UniTaskVoid HideAnimated()
        {
            _isVisible = false;

            _mainSequence?.Kill();
            _mainSequence = DOTween.Sequence();

            if (slideFromSide)
            {
                // Анимация уезда в сторону
                var targetPos = uiPanel.anchoredPosition;
                targetPos.x -= slideDistance;
                
                _mainSequence
                    .Append(canvasGroup.DOFade(0f, hideDuration).SetEase(Ease.InQuad))
                    .Join(uiPanel.DOAnchorPosX(targetPos.x, hideDuration).SetEase(Ease.InBack, 1.2f));
            }
            else
            {
                // Анимация сжатия
                _mainSequence
                    .Append(canvasGroup.DOFade(0f, hideDuration).SetEase(Ease.InQuad))
                    .Join(uiPanel.DOScale(Vector3.zero, hideDuration).SetEase(Ease.InBack, 1.2f));
            }

            await _mainSequence.AsyncWaitForCompletion();
            
            // Вызываем базовый Hide после анимации
            base.Hide();
            gameObject.SetActive(false);
        }

        public void AddQuest(CustomConfigurations.Quest.Quest quest)
        {
            var questView = Instantiate(questViewPrefab, questsParent);
            questView.SetInfo(quest.Id, quest.Title, quest.GetCurrentTask().description);
            _currentQuests.Add(questView);

            // Анимация появления нового квеста
            AnimateQuestAppearance(questView).Forget();
        }

        public void RemoveQuest(CustomConfigurations.Quest.Quest quest)
        {
            var questView = _currentQuests.FirstOrDefault(q => q.Id == quest.Id);
            if (questView == null) return;

            _currentQuests.Remove(questView);
            
            // Анимация исчезновения квеста
            AnimateQuestDisappearanceAndDestroy(questView).Forget();
        }

        private async UniTaskVoid AnimateQuestAppearance(QuestView questView)
        {
            // Подготавливаем элемент для анимации
            var rectTransform = questView.GetComponent<RectTransform>();
            var canvasGroup = questView.GetComponent<CanvasGroup>();
            
            if (canvasGroup == null)
                canvasGroup = questView.gameObject.AddComponent<CanvasGroup>();

            // Начальное состояние
            rectTransform.localScale = Vector3.zero;
            canvasGroup.alpha = 0f;
            
            // Создаем анимацию появления
            var sequence = DOTween.Sequence();
            
            sequence
                .Append(rectTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack, 1.7f))
                .Join(canvasGroup.DOFade(1f, 0.4f).SetEase(Ease.OutQuad))
                // Добавляем небольшой highlight эффект
                .AppendCallback(() => PlayHighlightEffect(questView));

            await sequence.AsyncWaitForCompletion();
        }

        private async UniTaskVoid AnimateQuestDisappearanceAndDestroy(QuestView questView)
        {
            var rectTransform = questView.GetComponent<RectTransform>();
            var canvasGroup = questView.GetComponent<CanvasGroup>();
            
            if (canvasGroup == null)
                canvasGroup = questView.gameObject.AddComponent<CanvasGroup>();

            // Анимация исчезновения
            var sequence = DOTween.Sequence();
            
            sequence
                .Append(rectTransform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack, 1.2f))
                .Join(canvasGroup.DOFade(0f, 0.3f).SetEase(Ease.InQuad))
                // Добавляем эффект сдвига влево
                .Join(rectTransform.DOAnchorPosX(200f, 0.3f).SetEase(Ease.InQuad));

            await sequence.AsyncWaitForCompletion();
            
            Destroy(questView.gameObject);
        }

        private void PlayHighlightEffect(QuestView questView)
        {
            // Эффект подсветки для нового квеста
            var image = questView.GetComponent<UnityEngine.UI.Image>();
            if (image != null)
            {
                var originalColor = image.color;
                var highlightColor = Color.yellow;
                highlightColor.a = originalColor.a;
                
                image.DOColor(highlightColor, 0.2f)
                    .SetLoops(2, LoopType.Yoyo)
                    .OnComplete(() => image.color = originalColor);
            }
        }

        public void UpdateQuest(CustomConfigurations.Quest.Quest quest)
        {
            UpdateQuestAnimated(quest).Forget();
        }

        private async UniTaskVoid UpdateQuestAnimated(CustomConfigurations.Quest.Quest quest)
        {
            var questView = _currentQuests.FirstOrDefault(q => q.Id == quest.Id);
            if (questView == null) return;

            // Анимация обновления - небольшое "встряхивание"
            var rectTransform = questView.GetComponent<RectTransform>();
            
            await rectTransform.DOPunchScale(Vector3.one * 0.1f, 0.3f, 5, 0.5f).AsyncWaitForCompletion();
            
            // Обновляем информацию
            questView.SetInfo(quest.Id, quest.Title, quest.GetCurrentTask().description);
            
            // Эффект обновления
            PlayHighlightEffect(questView);
        }

        public void ShowAllQuests()
        {
            ShowAllQuestsAnimated().Forget();
        }

        private async UniTaskVoid ShowAllQuestsAnimated()
        {
            // Анимация показа всех квестов с задержкой
            for (int i = 0; i < _currentQuests.Count; i++)
            {
                var questView = _currentQuests[i];
                var rectTransform = questView.GetComponent<RectTransform>();
                
                // Начинаем со скрытого состояния
                rectTransform.localScale = Vector3.zero;
                
                // Анимируем появление с задержкой
                rectTransform.DOScale(Vector3.one, 0.4f)
                    .SetDelay(i * questSpawnDelay)
                    .SetEase(Ease.OutBack, 1.3f);
            }

            // Ждем завершения всех анимаций
            await UniTask.Delay((int)((_currentQuests.Count * questSpawnDelay + 0.4f) * 1000));
        }
    }
}