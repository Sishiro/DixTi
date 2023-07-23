using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingPanel : Singleton<LoadingPanel>
{
    [SerializeField] private float rotationStepValue;
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject loadingIcon;

    public static bool isReady;

    public void Init()
    {
        StartCoroutine(Rotate());
    }

    private IEnumerator Rotate()
    {
        isReady = false;
        panel.SetActive(true);
        while (!isReady)
        {
            loadingIcon.transform.Rotate(0, 0, rotationStepValue);

            yield return null;
        }
        panel.SetActive(false);
    }
}
