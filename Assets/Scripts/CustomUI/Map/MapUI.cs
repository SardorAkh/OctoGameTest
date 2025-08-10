using Naninovel;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUI.Map
{
    public class MapUI : Naninovel.UI.CustomUI
    {
        [SerializeField] private Button forestButton;
        [SerializeField] private Button lakeButton;
        [SerializeField] private Button houseButton;

        protected override void Awake()
        {
            base.Awake();

            forestButton.onClick.AddListener(() => GoToLocation("Forest"));
            lakeButton.onClick.AddListener(() => GoToLocation("Lake"));
            houseButton.onClick.AddListener(() => GoToLocation("House"));
        }

        private async void GoToLocation(string locationScript)
        {
            Hide();
            await Engine.GetService<IScriptPlayer>().PreloadAndPlayAsync(locationScript);
        }
    }
}