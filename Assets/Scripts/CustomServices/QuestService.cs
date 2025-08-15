using System;
using System.Collections.Generic;
using System.Linq;
using CustomConfigurations.Quest;
using CustomUI.Quest;
using JetBrains.Annotations;
using Naninovel;
using Naninovel.UI;
using UnityEngine;

namespace CustomServices
{
    [InitializeAtRuntime]
    public class QuestService : IEngineService
    {
        private readonly QuestConfiguration _questConfiguration;
        private readonly ICustomVariableManager _variableManager;
        private Dictionary<string, Quest> _activeQuests = new Dictionary<string, Quest>();

        public QuestService(
            QuestConfiguration questConfiguration,
            ICustomVariableManager variableManager
        )
        {
            _questConfiguration = questConfiguration;
            _variableManager = variableManager;
        }

        public async UniTask InitializeServiceAsync()
        {
            await UniTask.CompletedTask;
        }

        public void ResetService() => _activeQuests.Clear();
        public void DestroyService() => _activeQuests.Clear();

        public void StartQuest(string questId)
        {
            var questData = _questConfiguration?.GetQuest(questId);
            if (questData == null) return;

            var quest = JsonUtility.FromJson<Quest>(JsonUtility.ToJson(questData));
            _activeQuests[questId] = quest;

            SaveQuestState(quest);
        }

        public void CompleteTask(string questId, string taskId)
        {
            if (!_activeQuests.TryGetValue(questId, out var quest)) return;

            var task = quest.Tasks.FirstOrDefault(t => t.id == taskId);
            if (task != null)
            {
                task.isCompleted = true;
                SaveQuestState(quest);

                if (quest.IsCompleted)
                {
                    _variableManager.SetVariableValue($"quest_{questId}_completed", "true");
                }
            }
        }

        public QuestTask GetCurrentQuestTask(string questId)
        {
            if (!_activeQuests.TryGetValue(questId, out var quest)) return null;
            return quest.GetCurrentTask();
        }

        public string GetCurrentTaskDescription(string questId)
        {
            if (!_activeQuests.TryGetValue(questId, out var quest)) return "";

            var currentTask = quest.GetCurrentTask();
            return currentTask?.description ?? "";
        }

        public bool IsQuestActive(string questId)
        {
            return _activeQuests.ContainsKey(questId) && !_activeQuests[questId].IsCompleted;
        }

        public bool IsQuestCompleted(string questId)
        {
            var completed = _variableManager.GetVariableValue($"quest_{questId}_completed");
            return completed == "true";
        }

        public bool HasCompletedTask(string questId, string taskId)
        {
            if (!_activeQuests.TryGetValue(questId, out var quest)) return false;
            var task = quest.Tasks.FirstOrDefault(t => t.id == taskId);
            return task?.isCompleted ?? false;
        }

        private void SaveQuestState(Quest quest)
        {
            var completedTasks = quest.Tasks.Where(t => t.isCompleted).Select(t => t.id).ToArray();
            var serializedTasks = string.Join(",", completedTasks);
            _variableManager.SetVariableValue($"quest_{quest.Id}_tasks", serializedTasks);
        }

        public void LoadQuestState(string questId)
        {
            var questData = _questConfiguration?.GetQuest(questId);
            if (questData == null) return;

            var quest = JsonUtility.FromJson<Quest>(JsonUtility.ToJson(questData));
            var completedTasksStr = _variableManager.GetVariableValue($"quest_{questId}_tasks");

            if (!string.IsNullOrEmpty(completedTasksStr))
            {
                var completedTaskIds = completedTasksStr.Split(',');
                foreach (var task in quest.Tasks)
                {
                    if (completedTaskIds.Contains(task.id))
                        task.isCompleted = true;
                }
            }

            _activeQuests[questId] = quest;
        }

        [CanBeNull]
        public Quest GetActiveQuestById(string questId)
        {
            return _activeQuests.GetValueOrDefault(questId);
        }
    }
}