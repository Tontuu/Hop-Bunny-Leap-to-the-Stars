using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class birdtrigger : MonoBehaviour
{
    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter2D()
    {
        animator.SetTrigger("Fly");
        Destroy(gameObject, 5f);
    }
}
