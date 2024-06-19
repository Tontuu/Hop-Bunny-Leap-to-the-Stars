using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    static public int currentMap;
    private bool soundPlayed01;
    private bool soundPlayed02;
    public CinemachineVirtualCamera cam1;
    public CinemachineVirtualCamera cam2;
    public CinemachineVirtualCamera cam3;
    public CinemachineVirtualCamera lookUpCam;
    float playerHeight;
    public GameObject Player;

    public GameObject Trigger1;
    public GameObject Trigger2;

    public AudioSource ground02_AudioSource;
    public AudioSource ground03_AudioSource;

    public GameObject FadeTransition;


    void Start()
    {
        FadeTransition.SetActive(true);
        JumpCounterController.Instance.UpdateCounter();
        soundPlayed01 = false;
        soundPlayed02 = false;
        SoundManager.Instance.SetActive(true);
        MusicManager.Instance.PlayMusic("Estou feliz");
        playerHeight = Player.GetComponent<SpriteRenderer>().bounds.size.y;
    }
    private void ManageMapSounds()
    {
        if (currentMap == 2)
        {
            if (!soundPlayed01)
            {
                ground02_AudioSource.volume = 0.15f;
                ground02_AudioSource.Play();
                soundPlayed01 = true;
            }
        }
        else
        {
            float newVolume = ground02_AudioSource.volume - (0.09f * Time.deltaTime);
            if (newVolume < 0) newVolume = 0;
            ground02_AudioSource.volume = newVolume;
            soundPlayed01 = false;
        }
        if (ground02_AudioSource.volume <= 0)
        {
            ground02_AudioSource.Pause();
        }

        if (currentMap == 1 || currentMap == 3)
        {
            if (!soundPlayed02)
            {
                ground03_AudioSource.volume = 0.55f;
                ground03_AudioSource.Play();
                soundPlayed02 = true;
            }
        }
        else
        {
            float newVolume = ground03_AudioSource.volume - (0.2f * Time.deltaTime);
            if (newVolume < 0) newVolume = 0;
            ground03_AudioSource.volume = newVolume;
            soundPlayed02 = false;
        }
        if (ground03_AudioSource.volume <= 0)
        {
            ground03_AudioSource.Pause();
        }
    }

    void Update()
    {
        if ((Player.transform.position.y - playerHeight) < Trigger1.transform.position.y)
        {
            currentMap = 1;
            CameraManager.SwitchCamera(cam1);
        }
        if ((Player.transform.position.y - playerHeight) > Trigger1.transform.position.y)
        {
            currentMap = 2;
            CameraManager.SwitchCamera(cam2);
        }

        if ((Player.transform.position.y - playerHeight) > Trigger2.transform.position.y)
        {
            currentMap = 3;
            CameraManager.SwitchCamera(cam3);
        }

        ManageMapSounds();
    }

}
