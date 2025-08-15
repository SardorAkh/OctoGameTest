using TMPro;
using UnityEngine;

namespace CustomConfigurations.Quest
{
    public class QuestView : MonoBehaviour
    {
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text taskText;

        public string Id { get; private set; }

        public void SetInfo(string id, string titleText, string taskText)
        {
            Id = id;
            this.titleText.text = titleText;
            this.taskText.text = taskText;
        }
    }
}