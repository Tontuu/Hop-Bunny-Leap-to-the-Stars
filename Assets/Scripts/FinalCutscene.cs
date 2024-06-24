using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinalCutscene : MonoBehaviour
{
    public Animator FadeAnim;
    public Image FadeImage;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            MusicManager.Instance.PlayMusic("Silence", 1f);
            StartCoroutine((FadeToCutscene()));
        }
    }


    IEnumerator FadeToCutscene()
    {
        FadeAnim.SetBool("FadeOut", true);
        yield return new WaitUntil(() => FadeImage.color.a == 1);
        SceneManager.LoadScene("FinalCutscene", LoadSceneMode.Single);
    }
}
