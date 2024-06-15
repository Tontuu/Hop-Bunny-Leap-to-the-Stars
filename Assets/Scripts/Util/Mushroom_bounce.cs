using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom_bounce : MonoBehaviour
{
    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        animator.SetTrigger("Bounce");
        SoundManager.Instance.PlaySound2D("Mushroom-Bounce");
    }
}
