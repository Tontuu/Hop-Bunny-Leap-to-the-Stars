using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DetectEnableLoad : MonoBehaviour
{
    public GameObject SceneLoader;
    public Image FadeImage;
    public Animator FadeAnim;
    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey)
        {
            StartCoroutine(FadeToGame());
        }
    }

    IEnumerator FadeToGame()
    {
        FadeAnim.SetBool("FadeOut", true);
        yield return new WaitUntil(() => FadeImage.color.a == 1);
        SceneLoader.SetActive(true);
    }
}
