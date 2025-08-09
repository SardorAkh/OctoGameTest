using System.Collections.Generic;
using System.Linq;

namespace CustomConfigurations.Quest
{
    [System.Serializable]
    public class QuestTask
    {
        public string id;
        public TaskType type;
        public string target;
        public string description;
        public List<string> requiredTasks = new List<string>();
        public bool isCompleted;

        public bool IsAvailable(List<QuestTask> allTasks)
        {
            if (requiredTasks.Count == 0) return true;
            return requiredTasks.All(reqId => allTasks.Any(t => t.id == reqId && t.isCompleted));
        }
    }
}