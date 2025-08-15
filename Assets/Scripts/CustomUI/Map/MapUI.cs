using DG.Tweening;
using Naninovel;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using CustomServices;

namespace CustomUI.Map
{
    public class MapUI : Naninovel.UI.CustomUI
    {
        [Header("Animation")] [SerializeField] private float animationDuration = 0.3f;
        [SerializeField] private AnimationCurve showCurve;

        [SerializeField] private LocationView libraryLocationView;
        [SerializeField] private LocationView classroomLocationView;
        [SerializeField] private LocationView principalOfficeLocationView;

        private ICustomVariableManager variableManager;
        private IScriptPlayer scriptPlayer;
        private QuestService questService;

        private bool _isVisible = false;

        protected override void Awake()
        {
            base.Awake();

            variableManager = Engine.GetService<ICustomVariableManager>();
            scriptPlayer = Engine.GetService<IScriptPlayer>();
            questService = Engine.GetService<QuestService>();

            libraryLocationView.LocationButton.onClick.AddListener(() => GoToLibrary());
            classroomLocationView.LocationButton.onClick.AddListener(() => GoToClassroom());
            principalOfficeLocationView.LocationButton.onClick.AddListener(() => GoToPrincipalOffice());
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateLocationAvailability();
        }

        private void UpdateLocationAvailability()
        {
            // Проверяем состояние квестов
            bool hasMetUme = questService.HasCompletedTask("main_quest", "talk_to_npc1");
            bool hasMetChinatsu = questService.HasCompletedTask("main_quest", "find_npc2");
            bool hasPlayedMinigame = questService.HasCompletedTask("main_quest", "play_minigame");
            bool hasArtifact = variableManager.TryGetVariableValue<bool>("has_artifact", out var artifactVar)
                               && artifactVar.ToString() == "True";
            bool hasReturnedToLocation2 = questService.HasCompletedTask("main_quest", "return_to_location2");
            bool questCompleted = questService.IsQuestCompleted("main_quest");
            bool location3Blocked = variableManager.TryGetVariableValue<bool>("location3_blocked", out var blockedVar)
                                   && blockedVar.ToString() == "True";

            // Библиотека - всегда доступна
            SetLocationAvailable(libraryLocationView, true, "Библиотека академии");

            // 3-А класс
            bool classroomAvailable = hasMetUme;
            string classroomText = "3-А класс";
            if (!hasMetUme)
            {
                classroomAvailable = false;
                classroomText = "Заперто";
            }
            else if (questCompleted)
            {
                classroomText = "3-А класс";
            }
            else if (hasReturnedToLocation2)
            {
                classroomText = "3-А класс (пуст)";
            }
            else if (hasPlayedMinigame)
            {
                classroomText = "3-А класс";
            }

            SetLocationAvailable(classroomLocationView, classroomAvailable, classroomText);

            // Кабинет директора
            bool officeAvailable = hasPlayedMinigame && !location3Blocked;
            string officeText = "Кабинет директора";
            if (!hasPlayedMinigame)
            {
                officeAvailable = false;
                officeText = "Требуется пропуск";
            }
            else if (location3Blocked)
            {
                officeAvailable = false;
                officeText = "Пусто";
            }

            SetLocationAvailable(principalOfficeLocationView, officeAvailable, officeText);
        }

        private void SetLocationAvailable(LocationView locationView, bool available, string tooltip)
        {
            locationView.LocationButton.interactable = available;

            if (available) locationView.UnLock();
            else locationView.Lock();

            var colors = locationView.LocationButton.colors;
            colors.normalColor = available ? Color.white : Color.gray;
            locationView.LocationButton.colors = colors;

            var text = locationView.LocationButton.GetComponentInChildren<Text>();
            if (text != null)
            {
                text.text = tooltip;
                text.color = available ? Color.black : Color.gray;
            }
        }

        private async void GoToLibrary()
        {
            Hide();

            // Детальная проверка состояния для выбора нужного скрипта
            bool questCompleted = questService.IsQuestCompleted("main_quest");
            bool hasReturnedToLocation2 = questService.HasCompletedTask("main_quest", "return_to_location2");
            bool hasMetChinatsu = questService.HasCompletedTask("main_quest", "find_npc2");
            bool hasMetUme = questService.HasCompletedTask("main_quest", "talk_to_npc1");
            
            string scriptToLoad;
            if (questCompleted)
            {
                scriptToLoad = "Location1_Static"; // После завершения квеста
            }
            else if (hasReturnedToLocation2)
            {
                scriptToLoad = "Location1_Final"; // Финальный выбор
            }
            else if (hasMetChinatsu)
            {
                scriptToLoad = "Location1_AfterChinatsu"; // После знакомства с Чинацу
            }
            else if (hasMetUme)
            {
                scriptToLoad = "Location1_AfterUme"; // После знакомства с Уме
            }
            else
            {
                scriptToLoad = "Location1_Initial"; // Первое посещение
            }

            await scriptPlayer.PreloadAndPlayAsync(scriptToLoad);
        }

        private async void GoToClassroom()
        {
            Hide();

            bool questCompleted = questService.IsQuestCompleted("main_quest");
            bool hasArtifact = variableManager.TryGetVariableValue<bool>("has_artifact", out var artifactVar)
                               && artifactVar.ToString() == "True";
            bool hasPlayedMinigame = questService.HasCompletedTask("main_quest", "play_minigame");
            bool hasMetChinatsu = questService.HasCompletedTask("main_quest", "find_npc2");

            string scriptToLoad;
            if (questCompleted)
            {
                scriptToLoad = "Location2_Static"; // После завершения квеста
            }
            else if (hasArtifact)
            {
                scriptToLoad = "Location2_Return"; // Возврат с артефактом
            }
            else if (hasPlayedMinigame)
            {
                scriptToLoad = "Location2_AfterMinigame"; // После мини-игры
            }
            else if (hasMetChinatsu)
            {
                scriptToLoad = "Location2_AfterMeeting"; // После знакомства
            }
            else
            {
                scriptToLoad = "Location2_Meeting"; // Первая встреча
            }

            await scriptPlayer.PreloadAndPlayAsync(scriptToLoad);
        }

        private async void GoToPrincipalOffice()
        {
            Hide();
            await scriptPlayer.PreloadAndPlayAsync("Location3_Artifact");
        }

        public void RefreshMap()
        {
            UpdateLocationAvailability();
        }

        public void OnShow()
        {
            UpdateLocationAvailability();
            if (_isVisible) return;

            ShowAsync().Forget();
        }

        public void OnHide()
        {
            if (!_isVisible) return;

            HideAsync().Forget();
        }

        public async UniTask ShowAsync(AsyncToken asyncToken = default)
        {
            _isVisible = true;
            gameObject.SetActive(true);

            transform.localScale = Vector3.zero;
            var tween = transform.DOScale(Vector3.one, animationDuration)
                .SetEase(showCurve);
            await tween.AsyncWaitForCompletion();

            UpdateLocationAvailability();
        }

        public async UniTask HideAsync(AsyncToken asyncToken = default)
        {
            _isVisible = false;

            var tween = transform.DOScale(Vector3.zero, animationDuration * 0.5f);
            await tween.AsyncWaitForCompletion();

            gameObject.SetActive(false);
        }
    }
}