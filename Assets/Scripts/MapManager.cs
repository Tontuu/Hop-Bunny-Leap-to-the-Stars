using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public CinemachineVirtualCamera cam1;
    public CinemachineVirtualCamera cam2;
    public CinemachineVirtualCamera cam3;
    public CinemachineVirtualCamera lookUpCam;
    float playerHeight;
    public GameObject Player;

    public GameObject Trigger1;
    public GameObject Trigger2;

    void Start()
    {
        playerHeight = Player.GetComponent<SpriteRenderer>().bounds.size.y;
    }

    void Update()
    {
        Debug.Log("Hop: " + (Player.transform.position.y - playerHeight));
        Debug.Log("Trigger1: " + Trigger1.transform.position.y);
        if ((Player.transform.position.y - playerHeight) < Trigger1.transform.position.y)
        {
            CameraManager.SwitchCamera(cam1);
        }
        if ((Player.transform.position.y - playerHeight) > Trigger1.transform.position.y)
        {
            CameraManager.SwitchCamera(cam2);
        }

        if ((Player.transform.position.y - playerHeight) > Trigger2.transform.position.y)
        {
            CameraManager.SwitchCamera(cam3);
        }
    }

}
