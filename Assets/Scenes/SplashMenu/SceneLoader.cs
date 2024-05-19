using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuLoader : MonoBehaviour
{
    Image FadeImage;
    Animator FadeAnim;
    void OnEnable()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
    IEnumerator FadeToGame()
    {
        FadeAnim.SetBool("FadeOut", true);
        yield return new WaitUntil(() => FadeImage.color.a == 1);
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
