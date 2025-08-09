using CustomServices;
using Naninovel;
using UnityEngine;

namespace CustomCommands
{
    [CommandAlias("minigame")]
    public class MiniGameCommand : Command
    {
        [RequiredParameter, ParameterAlias("gameid")] public StringParameter GameId;
        [ParameterAlias("resultvar")] public StringParameter ResultVariable;


        public override async UniTask ExecuteAsync(AsyncToken asyncToken = default)
        {
            var service = Engine.GetService<MiniGameService>();
            Debug.Log(service);
            var hasWon = await service.StartGameAsync(GameId);

            if (Assigned(ResultVariable))
            {
                var variableManager = Engine.GetService<ICustomVariableManager>();
                variableManager.SetVariableValue(ResultVariable, hasWon ? "true" : "false");
            }
        }
    }
}