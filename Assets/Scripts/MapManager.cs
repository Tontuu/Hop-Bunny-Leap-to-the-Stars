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
        MusicManager.Instance.PlayMusic("Estou feliz");
        playerHeight = Player.GetComponent<SpriteRenderer>().bounds.size.y;
    }

    void Update()
    {
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
