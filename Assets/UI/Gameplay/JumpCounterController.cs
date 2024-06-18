using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JumpCounterController : MonoBehaviour
{
    int counter = 0;
    Text text;
    // Start is called before the first frame update
    void Start()
    {
        text = this.GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
         Debug.Log(text);
         counter++;

        //  text.text = counter + "\nJUMPS";
    }
}
