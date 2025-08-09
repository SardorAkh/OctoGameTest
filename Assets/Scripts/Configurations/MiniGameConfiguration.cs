using System.Collections.Generic;
using Naninovel;
using UnityEngine;

namespace Configurations
{
    [EditInProjectSettings]
    public class MiniGameConfiguration : Configuration
    {
        [Header("Mini Games Settings")] public List<MiniGameData> miniGames = new();
        [Header("UI Settings")] public Canvas miniGameCanvas;
    }
}