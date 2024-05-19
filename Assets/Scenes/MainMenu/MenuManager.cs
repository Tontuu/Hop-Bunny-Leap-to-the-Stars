using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public Image FadeImage;
    public Animator FadeAnim;

    private void Start()
    {
        MusicManager.Instance.PlayMusic("Estou mal");
    }

    public void StartGame()
    {
        MusicManager.Instance.PlayMusic("Estou feliz");
        StartCoroutine(FadeToGame());
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    IEnumerator FadeToGame()
    {
        FadeAnim.SetBool("FadeOut", true);
        yield return new WaitUntil(() => FadeImage.color.a == 1);
        SceneManager.LoadScene("GameplayScene", LoadSceneMode.Single);
    }
}
