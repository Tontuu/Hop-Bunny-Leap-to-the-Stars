using UnityEngine;
using UnityEngine.EventSystems;

public class DropdownSoundTrigger : MonoBehaviour, ISelectHandler
{
    public void OnSelect(BaseEventData eventData)
    {
        MenuManager.TriggerDropdownSound();
        UI.TriggerDropdownSound();
    }
}