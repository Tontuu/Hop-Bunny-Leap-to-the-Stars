using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public Animator FadeContainerAnim;
    public CanvasGroup FadeContainerAlpha;

    public Image FadeImage;
    public Animator FadeAnim;

    private void Start()
    {
        MusicManager.Instance.PlayMusic("Estou mal");
    }

    public void StartGame()
    {
        MusicManager.Instance.PlayMusic("Estou feliz");
        StartCoroutine(FadeContainer());
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    IEnumerator FadeContainer()
    {
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
