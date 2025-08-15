using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Naninovel;

namespace Tools
{
    /// <summary>
    /// Набор готовых анимаций для UI элементов
    /// </summary>
    public static class UIAnimationHelper
    {
        // Стандартные настройки анимаций
        public static class Defaults
        {
            public const float QuickDuration = 0.2f;
            public const float NormalDuration = 0.4f;
            public const float SlowDuration = 0.6f;
            
            public static readonly Ease PopInEase = Ease.OutBack;
            public static readonly Ease PopOutEase = Ease.InBack;
            public static readonly Ease SmoothEase = Ease.OutQuart;
        }

        #region Popup Animations
        
        /// <summary>
        /// Анимация появления popup'а
        /// </summary>
        public static async UniTask PopupShowAsync(Transform target, float duration = -1)
        {
            if (duration < 0) duration = Defaults.NormalDuration;
            
            target.localScale = Vector3.zero;
            await target.DOScale(Vector3.one, duration)
                .SetEase(Defaults.PopInEase, 1.2f)
                .AsyncWaitForCompletion();
        }
        
        /// <summary>
        /// Анимация скрытия popup'а
        /// </summary>
        public static async UniTask PopupHideAsync(Transform target, float duration = -1)
        {
            if (duration < 0) duration = Defaults.QuickDuration;
            
            await target.DOScale(Vector3.zero, duration)
                .SetEase(Defaults.PopOutEase, 1.2f)
                .AsyncWaitForCompletion();
        }

        #endregion

        #region Slide Animations
        
        /// <summary>
        /// Анимация выезда слева
        /// </summary>
        public static async UniTask SlideInFromLeftAsync(RectTransform target, float distance = 300f, float duration = -1)
        {
            if (duration < 0) duration = Defaults.NormalDuration;
            
            var originalPos = target.anchoredPosition;
            var startPos = originalPos;
            startPos.x -= distance;
            
            target.anchoredPosition = startPos;
            await target.DOAnchorPos(originalPos, duration)
                .SetEase(Defaults.SmoothEase)
                .AsyncWaitForCompletion();
        }
        
        /// <summary>
        /// Анимация уезда влево
        /// </summary>
        public static async UniTask SlideOutToLeftAsync(RectTransform target, float distance = 300f, float duration = -1)
        {
            if (duration < 0) duration = Defaults.NormalDuration;
            
            var targetPos = target.anchoredPosition;
            targetPos.x -= distance;
            
            await target.DOAnchorPos(targetPos, duration)
                .SetEase(Defaults.SmoothEase)
                .AsyncWaitForCompletion();
        }

        #endregion

        #region Fade Animations
        
        /// <summary>
        /// Плавное появление с alpha
        /// </summary>
        public static async UniTask FadeInAsync(CanvasGroup target, float duration = -1)
        {
            if (duration < 0) duration = Defaults.NormalDuration;
            
            target.alpha = 0f;
            await target.DOFade(1f, duration)
                .SetEase(Defaults.SmoothEase)
                .AsyncWaitForCompletion();
        }
        
        /// <summary>
        /// Плавное исчезновение с alpha
        /// </summary>
        public static async UniTask FadeOutAsync(CanvasGroup target, float duration = -1)
        {
            if (duration < 0) duration = Defaults.NormalDuration;
            
            await target.DOFade(0f, duration)
                .SetEase(Defaults.SmoothEase)
                .AsyncWaitForCompletion();
        }

        #endregion

        #region Hover Effects
        
        /// <summary>
        /// Hover эффект с увеличением
        /// </summary>
        public static Tween CreateHoverScaleEffect(Transform target, float scale = 1.1f, float duration = -1)
        {
            if (duration < 0) duration = Defaults.QuickDuration;
            
            return target.DOScale(Vector3.one * scale, duration)
                .SetEase(Defaults.SmoothEase);
        }
        
        /// <summary>
        /// Возврат из hover состояния
        /// </summary>
        public static Tween CreateHoverExitEffect(Transform target, float duration = -1)
        {
            if (duration < 0) duration = Defaults.QuickDuration;
            
            return target.DOScale(Vector3.one, duration)
                .SetEase(Defaults.SmoothEase);
        }

        #endregion

        #region Shake Effects
        
        /// <summary>
        /// Shake эффект для ошибок
        /// </summary>
        public static async UniTask ShakeErrorAsync(Transform target, float strength = 10f, float duration = 0.5f)
        {
            await target.DOShakePosition(duration, strength, 20, 90f, false, true)
                .SetEase(Ease.InOutQuad)
                .AsyncWaitForCompletion();
        }
        
        /// <summary>
        /// Punch эффект для успеха
        /// </summary>
        public static async UniTask PunchSuccessAsync(Transform target, float strength = 0.2f, float duration = 0.6f)
        {
            await target.DOPunchScale(Vector3.one * strength, duration, 5, 0.5f)
                .SetEase(Ease.OutQuad)
                .AsyncWaitForCompletion();
        }

        #endregion

        #region Color Effects
        
        /// <summary>
        /// Эффект мигания цветом
        /// </summary>
        public static async UniTask FlashColorAsync(Graphic target, Color flashColor, float duration = 0.2f, int loops = 2)
        {
            var originalColor = target.color;
            
            await target.DOColor(flashColor, duration * 0.5f)
                .SetLoops(loops * 2, LoopType.Yoyo)
                .SetEase(Ease.InOutQuad)
                .AsyncWaitForCompletion();
                
            target.color = originalColor;
        }

        #endregion

        #region Sequence Helpers
        
        /// <summary>
        /// Создает последовательность появления элементов с задержкой
        /// </summary>
        public static async UniTask StaggeredShowAsync(Transform[] targets, float delayBetween = 0.1f, float duration = -1)
        {
            if (duration < 0) duration = Defaults.NormalDuration;
            
            var sequence = DOTween.Sequence();
            
            for (int i = 0; i < targets.Length; i++)
            {
                var target = targets[i];
                target.localScale = Vector3.zero;
                
                sequence.Insert(i * delayBetween, 
                    target.DOScale(Vector3.one, duration).SetEase(Defaults.PopInEase, 1.2f));
            }
            
            await sequence.AsyncWaitForCompletion();
        }
        
        /// <summary>
        /// Волновой эффект для группы элементов
        /// </summary>
        public static async UniTask WaveEffectAsync(Transform[] targets, float amplitude = 20f, float frequency = 2f, float duration = 1f)
        {
            var sequence = DOTween.Sequence();
            
            for (int i = 0; i < targets.Length; i++)
            {
                var target = targets[i];
                var delay = i * 0.1f;
                
                sequence.Insert(delay, target.DOLocalMoveY(amplitude, duration / frequency)
                    .SetEase(Ease.InOutSine)
                    .SetLoops((int)(frequency * 2), LoopType.Yoyo));
            }
            
            await sequence.AsyncWaitForCompletion();
        }

        #endregion

        #region Progress Bar Animations
        
        /// <summary>
        /// Анимация заполнения прогресс бара
        /// </summary>
        public static async UniTask AnimateProgressBarAsync(Image progressBar, float targetFill, float duration = 1f)
        {
            await progressBar.DOFillAmount(targetFill, duration)
                .SetEase(Ease.OutQuart)
                .AsyncWaitForCompletion();
        }
        
        /// <summary>
        /// Анимация прогресс бара с эффектом "пульса"
        /// </summary>
        public static async UniTask AnimateProgressBarWithPulseAsync(Image progressBar, float targetFill, float duration = 1f)
        {
            var sequence = DOTween.Sequence();
            
            sequence
                .Append(progressBar.DOFillAmount(targetFill, duration).SetEase(Ease.OutQuart))
                .Join(progressBar.transform.DOPunchScale(Vector3.one * 0.1f, duration * 0.3f, 5, 0.5f).SetDelay(duration * 0.7f));
            
            await sequence.AsyncWaitForCompletion();
        }

        #endregion

        #region Utility Methods
        
        /// <summary>
        /// Убивает все анимации на объекте
        /// </summary>
        public static void KillAllAnimations(Transform target)
        {
            target.DOKill();
        }
        
        /// <summary>
        /// Сбрасывает transform в исходное состояние
        /// </summary>
        public static void ResetTransform(Transform target)
        {
            target.DOKill();
            target.localPosition = Vector3.zero;
            target.localRotation = Quaternion.identity;
            target.localScale = Vector3.one;
        }

        #endregion
    }
}