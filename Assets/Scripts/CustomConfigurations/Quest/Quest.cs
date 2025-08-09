using System.Collections.Generic;
using System.Linq;

namespace CustomConfigurations.Quest
{
    public class Quest
    {
        public string Id;
        public string Title;
        public List<QuestTask> Tasks = new List<QuestTask>();

        public QuestTask GetCurrentTask()
        {
            return Tasks.FirstOrDefault(t => !t.isCompleted && t.IsAvailable(Tasks));
        }

        public bool IsCompleted => Tasks.All(t => t.isCompleted);
        public bool IsStarted => Tasks.Any(t => t.isCompleted);
    }
}