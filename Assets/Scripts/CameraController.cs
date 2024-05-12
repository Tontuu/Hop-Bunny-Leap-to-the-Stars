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
        // Only Fit on screen change.
        if (Screen.width != startScreenWidth)
        {
            FitCamToScreen();
            startScreenWidth = Screen.width;
        }

        CheckLookUp();
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
        vcam.m_Lens.OrthographicSize = newOrthoSize;
    }

    private void CheckLookUp()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            Vector2 newPos = vcam.transform.position;
            yOffset = Mathf.Lerp(0.5f, 1.5f, 0.1f);
            vcam.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenY = yOffset;
        }
    }
}
