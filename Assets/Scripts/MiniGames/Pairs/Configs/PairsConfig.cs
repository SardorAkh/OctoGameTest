using UnityEngine;

namespace MiniGames.Pairs.Configs
{
    [CreateAssetMenu(fileName = nameof(PairsConfig), menuName = "Configs/" + nameof(PairsConfig), order = 0)]
    public class PairsConfig : ScriptableObject
    {
        [field: SerializeField] public Sprite[] CardSprites { get; private set; }
        [field: SerializeField] public int PairsCount { get; private set; }
    }
}