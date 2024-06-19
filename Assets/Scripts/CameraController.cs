using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    private CinemachineVirtualCamera vcam;

    private float startScreenWidth;
    const int orthoSizeNormal = 15;
    const int orthoSizeWideScreen = 13;

    int zoomOffset = 0;

    // LookUp Funct
    public float yOffset;

    // Start is called before the first frame update
    void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
        FitCamToScreen();
        startScreenWidth = Screen.width;
    }

    void Update()
    {
        zoomOffset = 0;
        if (CarrotController.reachedCarrot)
        {
            zoomOffset = 1;
            FitCamToScreen();
        }
        // Only Fit on screen change.
        if (Screen.width != startScreenWidth)
        {
            FitCamToScreen();
            startScreenWidth = Screen.width;
        }
    }

    private void FitCamToScreen()
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

        vcam.m_Lens.OrthographicSize = newOrthoSize - zoomOffset;

    }
}
