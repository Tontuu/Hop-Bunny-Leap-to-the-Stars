using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerZone : MonoBehaviour
{
    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerExit;
    private void OnTriggerEnter2D(Collider2D other)
    {
        onTriggerEnter?.Invoke();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        onTriggerExit?.Invoke();
    }
}
