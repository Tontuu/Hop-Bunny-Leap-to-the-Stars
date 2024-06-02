using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropdownSoundTrigger : MonoBehaviour, ISelectHandler
{
    public void OnSelect(BaseEventData eventData)
    {
        MenuManager.TriggerDropdownSound();
    }
}