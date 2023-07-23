using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JoinManager : Singleton<JoinManager>
{
    [SerializeField] private ConnectionSystem connectionSystem;

    public TMP_InputField joinCodeInputField;

    public async void OnClickJoin()
    {
        LoadingPanel.Instance.Init();

        bool isOk = await connectionSystem.JoinRelay();

        LoadingPanel.isReady = true;

        if (isOk)
        {
            RoomManager.Instance.joinCodeTextUI.text = "Código: " + joinCodeInputField.text.ToUpper();
            SceneSystem.Instance.LoadScene(SceneName.Room);
        }
        else
        {

        }
    }

    public void OnClickBack()
    {
        SceneSystem.Instance.LoadScene(SceneName.ConectionMenu);
    }
}
