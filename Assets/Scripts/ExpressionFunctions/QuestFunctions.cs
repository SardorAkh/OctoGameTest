using CustomServices;
using Naninovel;

namespace ExpressionFunctions
{
    [ExpressionFunctions]
    public static class QuestFunctions
    {
        public static bool IsQuestActive(string questId)
        {
            var questManager = Engine.GetService<QuestService>();
            return questManager.IsQuestActive(questId);
        }

        public static bool IsQuestCompleted(string questId)
        {
            var questManager = Engine.GetService<QuestService>();
            return questManager.IsQuestCompleted(questId);
        }

        public static bool HasCompletedTask(string questId, string taskId)
        {
            var questManager = Engine.GetService<QuestService>();
            return questManager.HasCompletedTask(questId, taskId);
        }

        public static string GetCurrentTaskDescription(string questId)
        {
            var questManager = Engine.GetService<QuestService>();
            return questManager.GetCurrentTaskDescription(questId);
        }
    }
}