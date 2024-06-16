using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullCrashFX : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        SoundManager.Instance.PlaySound2D("Crash");
        Destroy(gameObject, 0.3f);
    }
}
