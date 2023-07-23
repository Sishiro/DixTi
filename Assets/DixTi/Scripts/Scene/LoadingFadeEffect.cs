using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingFadeEffect : Singleton<LoadingFadeEffect>
{
    public static bool canLoad;
    [SerializeField] private Image loadingBackground;
    [SerializeField] [Range(0f, 0.5f)] private float loadingStepTime;
    [SerializeField] [Range(0f, 0.5f)] private float loadingStepValue;

    IEnumerator FadeInEffect()
    {
        Color backgroundColor = loadingBackground.color;
        backgroundColor.a = 0;
        loadingBackground.color = backgroundColor;

        while (backgroundColor.a <= 0.9f)
        {
            yield return new WaitForSeconds(loadingStepTime);

            backgroundColor.a += loadingStepValue;

            loadingBackground.color = backgroundColor;
        }

        canLoad = true;
    }

    IEnumerator FadeOutEffect()
    {
        canLoad = false;

        Color backgroundColor = loadingBackground.color;

        while (backgroundColor.a >= 0)
        {
            yield return new WaitForSeconds(loadingStepTime);

            backgroundColor.a -= loadingStepValue;

            loadingBackground.color = backgroundColor;
        }
    }

    public void FadeIn()
    {
        StartCoroutine(FadeInEffect());
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutEffect());
    }
}
