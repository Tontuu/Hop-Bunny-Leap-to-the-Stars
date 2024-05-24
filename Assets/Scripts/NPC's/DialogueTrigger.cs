using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueObject dialogue;

    public void TriggerDialogue()
    {
        Debug.Log("DEBUG: Npc POS - " + transform.localPosition);
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue, transform.position);
    }
}