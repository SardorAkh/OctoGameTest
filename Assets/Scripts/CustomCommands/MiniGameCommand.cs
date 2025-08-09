using CustomServices;
using Naninovel;

namespace CustomCommands
{
    [Command.CommandAlias("minigame")]
    public class MiniGameCommand : Command
    {
        [Command.RequiredParameter] public StringParameter GameId;
        [Command.ParameterAlias("resultvar")] public StringParameter ResultVariable;


        public override async UniTask ExecuteAsync(AsyncToken asyncToken = default)
        {
            var service = Engine.GetService<MiniGameService>();

            var hasWon = await service.StartGameAsync(GameId);

            if (Assigned(ResultVariable))
            {
                var variableManager = Engine.GetService<ICustomVariableManager>();
                variableManager.SetVariableValue(ResultVariable, hasWon ? "true" : "false");
            }
        }
    }
}