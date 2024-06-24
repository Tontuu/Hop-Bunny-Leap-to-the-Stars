using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class CutsceneManager : MonoBehaviour
{
    private double time;
    public GameObject videoPlayerObj;
    public VideoPlayer videoPlayer;

    void Start()
    {
        time = videoPlayer.GetComponent<VideoPlayer>().clip.length;
        videoPlayer = videoPlayerObj.GetComponent<VideoPlayer>();
        videoPlayer.Prepare();
        videoPlayer.loopPointReached += EndReached;
    }

    void Update()
    {
        if (UI.isPaused)
        {
            videoPlayer.Pause();
        }
        else
        {
            videoPlayer.Play();
        }
        if (Input.anyKeyDown)
        {
            if (videoPlayer.time > 41.0)
            {
                EndReached(videoPlayer);
            }

            if (Input.GetMouseButtonDown(0)
            || Input.GetMouseButtonDown(1)
            || Input.GetMouseButtonDown(2))
                return;

            if (!Input.GetKeyDown(KeyCode.Escape))
            {
                SoundManager.Instance.PlaySound2D("Error");
            }
        }
    }

    void EndReached(VideoPlayer vp)
    {
        SceneManager.LoadScene("MainMenu");
    }
}
