using CustomServices;
using CustomUI.Quest;
using Naninovel;
using UnityEngine;

namespace CustomCommands
{
    [CommandAlias("startQuest")]
    public class StartQuestCommand : Command
    {
        [ParameterAlias("questId"), RequiredParameter]
        public StringParameter QuestId;

        public override async UniTask ExecuteAsync(AsyncToken asyncToken = default)
        {
            var questService = Engine.GetService<QuestService>();
            questService.StartQuest(QuestId);

            var uiManager = Engine.GetService<IUIManager>();
            var questSystemUI = uiManager.GetUI<QuestSystemUI>();
            var quest = questService.GetActiveQuestById(QuestId);

            if (quest != null)
            {
                questSystemUI.AddQuest(quest);
            }

            await UniTask.CompletedTask;
        }
    }
}