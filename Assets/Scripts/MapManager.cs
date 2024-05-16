using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public CinemachineVirtualCamera cam1;
    public CinemachineVirtualCamera cam2;
    public CinemachineVirtualCamera lookUpCam;
    float playerHeight;
    public GameObject Player;

    void Start()
    {
        playerHeight = Player.GetComponent<SpriteRenderer>().bounds.size.y;
    }

    void Update()
    {
        if ((Player.transform.position.y - playerHeight) < gameObject.transform.position.y)
        {
            CameraManager.SwitchCamera(cam1);
        }
        else
        {
            CameraManager.SwitchCamera(cam2);
        }
    }
}
