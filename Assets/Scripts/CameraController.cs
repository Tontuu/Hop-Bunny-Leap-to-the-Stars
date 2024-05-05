using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    private CinemachineVirtualCamera vcam;

    const int orthoSize = 15;
    const int orthoSizeWideScreen = 13;

    // Truncate to 1 decimal places so we can work with it easily.
    const float targetAspectRatio = 1.7f;

    // Start is called before the first frame update
    void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
    }

    void Update()
    {
        float currentAspectRatio = (float)Math.Truncate((float)Screen.width / (float)Screen.height * 10f) / 10f;
        if (currentAspectRatio < targetAspectRatio)
        {
            vcam.m_Lens.OrthographicSize = orthoSize;
        }
        else
        {
            vcam.m_Lens.OrthographicSize = orthoSizeWideScreen;
        }
    }
}
