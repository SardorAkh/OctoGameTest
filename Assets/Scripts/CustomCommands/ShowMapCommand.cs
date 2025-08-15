using Naninovel;

namespace CustomUI.Map
{
    [CommandAlias("map")]
    public class ShowMapCommand : Command
    {
        public override async UniTask ExecuteAsync(AsyncToken asyncToken = default)
        {
            var mapUI = Engine.GetService<IUIManager>().GetUI<MapUI>();
            if (mapUI != null)
            {
                // mapUI.up
                mapUI.Show();
            }

            await UniTask.CompletedTask;
        }
    }
}