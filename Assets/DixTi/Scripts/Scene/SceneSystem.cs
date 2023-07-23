using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum SceneName : int
{
    MainMenu,
    ConectionMenu,
    Room,
    JoinMenu,
    EnterTheme,
    WaitForTheme,
    ShowTheme,
    EnterPhrase,
    ShowPhrases,
    EnterGuess,
    ShowGuesses,
    ShowScore,
    Win,
}

public class SceneSystem : Singleton<SceneSystem>
{
    [SerializeField] private List<GameObject> panels;
    public SceneName activeScene { get; private set; }

    public void LoadScene(SceneName scene)
    {
        StartCoroutine(Loading(scene));
    }

    private void ChangeScene(SceneName scene)
    {
        DeactivatePanels();
        activeScene = scene;
        panels[(int)scene].SetActive(true);
    }

    private IEnumerator Loading(SceneName scene)
    {
        LoadingFadeEffect.Instance.FadeIn();

        yield return new WaitUntil(() => LoadingFadeEffect.canLoad);

        ChangeScene(scene);

        yield return new WaitForSeconds(0.1f);

        LoadingFadeEffect.Instance.FadeOut();
    }

    private void DeactivatePanels()
    {
        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }
    }
}
