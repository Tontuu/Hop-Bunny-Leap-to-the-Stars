using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clickableimage : MonoBehaviour
{
    public Image button;
    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Image>();
        button.alphaHitTestMinimumThreshold = 0.5f;
    }
}
