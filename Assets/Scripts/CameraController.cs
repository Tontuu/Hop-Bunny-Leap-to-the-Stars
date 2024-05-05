using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    private CinemachineVirtualCamera vcam;

    const int orthoSizeNormal = 15;
    const int orthoSizeWideScreen = 13;

    // Start is called before the first frame update
    void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
    }

    void Update()
    {
        float newOrthoSize;
        float widthToHeightRatio = (float)Screen.width / Screen.height;
        float thresholdRatio = 1.5f;
        if (widthToHeightRatio > thresholdRatio)
        {
            newOrthoSize = orthoSizeWideScreen;
        }
        else
        {
            newOrthoSize = orthoSizeNormal;
        }

        vcam.m_Lens.OrthographicSize = newOrthoSize;
    }
}
