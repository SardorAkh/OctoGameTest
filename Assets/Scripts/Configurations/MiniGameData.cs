using System;
using UnityEngine;

namespace Configurations
{
    [Serializable]
    public class MiniGameData
    {
        public string gameId;

        public string displayName;

        public GameObject uiPrefab;

        public int winThreshold = 50;

        public float maxDuration = 60f;

        public bool isReplayable = true;
    }
}