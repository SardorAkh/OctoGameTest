using System.Collections.Generic;
using System.Linq;
using Naninovel;

namespace CustomConfigurations.Quest
{
    [Configuration.EditInProjectSettings]
    public class QuestConfiguration : Configuration
    {
        public List<Quest> Quests = new List<Quest>();

        public Quest GetQuest(string questId)
        {
            return Quests.FirstOrDefault(q => q.Id == questId);
        }
    }
}