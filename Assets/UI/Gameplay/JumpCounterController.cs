using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JumpCounterController : MonoBehaviour
{
    public static JumpCounterController Instance;
    static int counter = 0;
    public TextMeshProUGUI text;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void ResetCounter()
    {
        counter = 0;
    }
    public void UpdateCounter()
    {
        text.text = counter + "\nPULOS";
    }
    public void IncrementCounter()
    {
        counter++;
        UpdateCounter();
    }
}
