using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerZone : MonoBehaviour
{
    DialogueTrigger dialogueTrigger;
    public bool oneShot = false;
    private bool alreadyEntered = false;
    private bool alreadyExited = false;

    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerExit;
    public DialogueManager dialogueManager;

    void Awake()
    {
        dialogueTrigger = GetComponent<DialogueTrigger>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        dialogueTrigger.TriggerDialogue();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        dialogueManager.EndDialogue();
    }
}
