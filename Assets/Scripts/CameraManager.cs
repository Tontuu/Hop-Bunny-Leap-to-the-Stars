using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    static List<CinemachineVirtualCamera> cameras = new List<CinemachineVirtualCamera>();
    public static CinemachineVirtualCamera ActiveCam = null;

    public static bool IsActiveCamera(CinemachineVirtualCamera cam)
    {
        return cam == ActiveCam;
    }

    public static void SwitchCamera(CinemachineVirtualCamera newCam)
    {
        newCam.Priority = 10;
        ActiveCam = newCam;

        foreach (CinemachineVirtualCamera cam in cameras)
        {
            if (cam != newCam)
            {
                cam.Priority = 0;
            }
        }
    }

    public static void Register(CinemachineVirtualCamera cam)
    {
        cameras.Add(cam);
    }

    public static void Unregister(CinemachineVirtualCamera cam)
    {
        cameras.Remove(cam);
    }
}
