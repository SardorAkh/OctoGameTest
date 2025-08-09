using CustomServices;
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
            var questManager = Engine.GetService<QuestService>();
            questManager.CompleteTask(QuestId, TaskId);
            await UniTask.CompletedTask;
        }
    }
}