using UnityEngine;
using UnityEngine.UI;

namespace CustomUI.Map
{
    public class LocationView : MonoBehaviour
    {
        [SerializeField] private GameObject lockPanel;
        [SerializeField] private Button locationButton;

        public Button LocationButton => locationButton;

        public void Lock()
        {
            lockPanel.SetActive(true);
            locationButton.interactable = false;
        }

        public void UnLock()
        {
            lockPanel.SetActive(false);
            locationButton.interactable = true;
        }
    }
}