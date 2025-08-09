using CustomServices;
using Naninovel;

namespace CustomCommands
{
    public class StartQuestCommand : Command
    {
        [ParameterAlias(NamelessParameterAlias), RequiredParameter]
        public StringParameter QuestId;

        public override async UniTask ExecuteAsync(AsyncToken asyncToken = default)
        {
            var questManager = Engine.GetService<QuestService>();
            questManager.StartQuest(QuestId);
        }
    }
}