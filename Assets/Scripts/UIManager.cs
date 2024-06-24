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
    public GameObject HudCanvas;
    public GameObject PauseCanvas;
    public GameObject MainScreen;
    public GameObject OptionScreen;
    public AudioMixer audioMixer;
    public Animator tutorialOverlay;
    public static float elapsedTimeTutorial;
    public static bool finishedTutorial;

    void Awake()
    {
        Cursor.visible = false;
    }

    void Start()
    {
        isPaused = false;
        elapsedTimeTutorial = 0f;
        finishedTutorial = false;
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


        // Tutorial overlay
        if (DialogueManager.isDialogueActive)
        {
            if (finishedTutorial == false)
            {
                tutorialOverlay.SetTrigger("show");
            }
            else
            {
                tutorialOverlay.SetTrigger("hide");
                elapsedTimeTutorial += Time.deltaTime;
            }

            if (elapsedTimeTutorial >= 15.0f && finishedTutorial)
            {
                finishedTutorial = false;
                elapsedTimeTutorial = 0f;
            }
        }
    }

    public void ShowPause()
    {
        Cursor.visible = true;
        Time.timeScale = 0;
        isPaused = true;
        if (HudCanvas) HudCanvas.SetActive(false);
        PauseCanvas.SetActive(true);
        MainScreen.SetActive(true);
    }

    public void HidePause()
    {
        Cursor.visible = false;
        Time.timeScale = 1;
        isPaused = false;
        if (HudCanvas) HudCanvas.SetActive(true);
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
