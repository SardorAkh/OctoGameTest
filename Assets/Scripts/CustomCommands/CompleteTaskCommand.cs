using CustomServices;
using CustomUI.Quest;
using Naninovel;

namespace CustomCommands
{
    [CommandAlias("completeTask")]
    public class CompleteTaskCommand : Command
    {
        [ParameterAlias(NamelessParameterAlias), RequiredParameter]
        public StringParameter QuestId;

        [ParameterAlias("task"), RequiredParameter]
        public StringParameter TaskId;

        public override async UniTask ExecuteAsync(AsyncToken asyncToken = default)
        {
            var questService = Engine.GetService<QuestService>();
            questService.CompleteTask(QuestId, TaskId);

            var uiManager = Engine.GetService<IUIManager>();
            var questSystemUI = uiManager.GetUI<QuestSystemUI>();
            var quest = questService.GetActiveQuestById(QuestId);
            var task = questService.GetCurrentQuestTask(QuestId);

            if (quest != null)
            {
                questSystemUI.RemoveQuest(quest);
            }

            if (task != null)
            {
                questSystemUI.AddQuest(quest);
            }

            await UniTask.CompletedTask;
        }
    }
}