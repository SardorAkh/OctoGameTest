using System.Collections.Generic;
using System.Linq;
using CustomConfigurations.Quest;
using UnityEngine;

namespace CustomUI.Quest
{
    public class QuestSystemUI : Naninovel.UI.CustomUI
    {
        [SerializeField] private Transform questsParent;
        [SerializeField] private QuestView questViewPrefab;

        private List<QuestView> _currentQuests = new();

        public void AddQuest(CustomConfigurations.Quest.Quest quest)
        {
            var questView = Instantiate(questViewPrefab, questsParent);
            questView.SetInfo(quest.Id, quest.Title, quest.GetCurrentTask().description);
            _currentQuests.Add(questView);
        }

        public void RemoveQuest(CustomConfigurations.Quest.Quest quest)
        {
            var questView = _currentQuests.First(q => q.Id == quest.Id);
            _currentQuests.Remove(questView);
            Destroy(questView.gameObject);
        }
    }
}