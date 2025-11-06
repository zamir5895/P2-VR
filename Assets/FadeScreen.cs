using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeScreen : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private float duracionFade = 1f;

    void Start()
    {
        if (fadeImage != null)
            StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float tiempo = 0;
        Color color = fadeImage.color;

        while (tiempo < duracionFade)
        {
            tiempo += Time.deltaTime;
            color.a = 1 - (tiempo / duracionFade);
            fadeImage.color = color;
            yield return null;
        }

        color.a = 0;
        fadeImage.color = color;
        fadeImage.gameObject.SetActive(false);
    }
}