using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class UI : MonoBehaviour
{
    public TextMeshProUGUI resolutionToggle;
    public TextMeshProUGUI resolutionLabel;
    public static bool isPaused;
    public GameObject PauseCanvas;
    public GameObject MainScreen;
    public GameObject OptionScreen;
    public AudioMixer audioMixer;

    void Start()
    {
        isPaused = false;
    }

    void Update()
    {
        FlushText();
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
                ShowPause();
            else
                HidePause();
        }
    }

    public void ShowPause()
    {
        Time.timeScale = 0;
        isPaused = true;
        PauseCanvas.SetActive(true);
        MainScreen.SetActive(true);
    }

    public void HidePause()
    {
        Time.timeScale = 1;
        isPaused = false;
        PauseCanvas.SetActive(false);
        MainScreen.SetActive(false);
        OptionScreen.SetActive(false);
    }

    public void ShowOptions()
    {
        MainScreen.SetActive(false);
        OptionScreen.SetActive(true);
        FlushText();
    }

    public void HideOptions()
    {
        MainScreen.SetActive(true);
        OptionScreen.SetActive(false);
    }

    public void OnContinueButton()
    {
        HidePause();
    }


    public void OnOptionsButton()
    {
        ShowOptions();
    }

    public void OnOptionsGoBackButton()
    {
        HideOptions();
    }

    public void OnQuitButton()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    static public void TriggerDropdownSound()
    {
        SoundManager.Instance.PlaySound2D("Dropdown");
    }

    static public void TriggerButtonSound()
    {
        SoundManager.Instance.PlaySound2D("Buttons");
    }

    public void FlushText()
    {
        // Fullscreen toggle
        if (Screen.fullScreen)
        {
            resolutionToggle.text = "on >";
        }
        else
        {
            resolutionToggle.text = "< off";
        }

        //Resolution Dropdown
        if (!Screen.fullScreen)
        {
            resolutionLabel.text = Screen.width +
            " x " + Screen.height;
        }
    }
}
