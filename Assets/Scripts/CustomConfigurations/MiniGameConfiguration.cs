using System.Collections.Generic;
using Naninovel;
using UnityEngine;

namespace CustomConfigurations
{
    [EditInProjectSettings]
    public class MiniGameConfiguration : Configuration
    {
        [Header("Mini Games Settings")] public List<MiniGameData> miniGames = new();
    }
}