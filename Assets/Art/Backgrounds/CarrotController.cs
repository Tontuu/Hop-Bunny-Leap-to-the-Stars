using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CarrotController : MonoBehaviour
{
    static public bool reachedCarrot;
    public CinemachineVirtualCamera carrotCam;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        reachedCarrot = false;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (reachedCarrot)
        {
            StartCoroutine(ResetCamera());
        }
    }

    // Update is called once per frame

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            GetComponent<Collider2D>().enabled = false;
            reachedCarrot = true;
            animator.Play("carrot_runaway");
        }
    }


        IEnumerator ResetCamera()
        {
            yield return new WaitForSeconds(1.5f);
            reachedCarrot = false;
        }
}
