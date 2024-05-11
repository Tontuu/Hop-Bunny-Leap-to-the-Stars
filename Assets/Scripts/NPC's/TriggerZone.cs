using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerZone : MonoBehaviour
{
    public bool oneShot = false;
    private bool alreadyEntered = false;
    private bool alreadyExited = false;


    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerExit;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (alreadyEntered) return;

        onTriggerEnter?.Invoke();

        if (oneShot) alreadyEntered = true;

        if (DialogueManager.Instance.CanDisableTrigger())
        {
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (alreadyExited) return;

        onTriggerExit?.Invoke();

        if (oneShot) alreadyExited = true;
    }
}
