using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    [SerializeField] private ConnectionSystem connectionSystem;
    public async void OnClickHost()
    {
        RoomManager.Instance.ResetPlayersNames();
        LoadingPanel.Instance.Init();

        bool isOk = await connectionSystem.CreateRelay();

        LoadingPanel.isReady = true;

        if (isOk)
        {
            SceneSystem.Instance.LoadScene(SceneName.Room);
        }
        else
        {
            // Error
        }
    }

    public void OnClickClient()
    {
        RoomManager.Instance.ResetPlayersNames();
        SceneSystem.Instance.LoadScene(SceneName.JoinMenu);
    }

    public void OnClickBack()
    {
        SceneSystem.Instance.LoadScene(SceneName.MainMenu);
    }
}
