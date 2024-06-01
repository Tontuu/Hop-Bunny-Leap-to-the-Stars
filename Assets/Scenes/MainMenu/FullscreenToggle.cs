using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FullscreenToggle : MonoBehaviour
{
    TextMeshProUGUI label;
    Toggle toggle;

    void Awake()
    {
        label = GetComponent<TextMeshProUGUI>();
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnSwitch);
        if (toggle.isOn)
        {
            OnSwitch(true);
        }
    }

    void OnSwitch(bool on)
    {
        if (on)
            label.text = "on >";
        else
            label.text = "< off";

        SettingsManager.SetFullscreen(on);
    }

    void OnDestroy()
    {
        toggle.onValueChanged.RemoveListener(OnSwitch);
    }
}
