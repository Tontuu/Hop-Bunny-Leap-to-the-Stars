using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public Animator FadeContainerAnim;
    public CanvasGroup FadeContainerAlpha;

    public GameObject MenuScreen;
    public GameObject OptionScreen;

    public Image FadeImage;
    public Animator FadeAnim;

    private void Start()
    {
        FadeAnim.SetBool("FadeOut", false);
        FadeAnim.SetBool("FadeIn", true);
        MusicManager.Instance.PlayMusic("Estou mal");
    }

    public void StartGame()
    {
        MusicManager.Instance.PlayMusic("Estou feliz");
        StartCoroutine(FadeContainer());
    }

    static public void TriggerButtonSound()
    {
        SoundManager.Instance.PlaySound2D("Buttons");
    }

    static public void TriggerDropdownSound()
    {
        SoundManager.Instance.PlaySound2D("Dropdown");
    }

    public void ShowOptions()
    {
        MenuScreen.SetActive(false);
        OptionScreen.SetActive(true);
    }

    public void ShowMenu()
    {
        MenuScreen.SetActive(true);
        OptionScreen.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    IEnumerator FadeContainer()
    {
        SoundManager.Instance.SetActive(false);
        FadeContainerAnim.SetBool("FadeOut", true);
        yield return new WaitUntil(() => FadeContainerAlpha.alpha == 0);
        StartCoroutine(FadeToGame());
    }

    IEnumerator FadeToGame()
    {
        FadeAnim.SetBool("FadeOut", true);
        yield return new WaitUntil(() => FadeImage.color.a == 1);
        SceneManager.LoadScene("GameplayScene", LoadSceneMode.Single);
    }
}
