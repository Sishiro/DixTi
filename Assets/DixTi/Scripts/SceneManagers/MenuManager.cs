using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField playerNameIF;
    [SerializeField] private InitializeServices initializeServices;

    public async void OnClickStart()
    {
        if (playerNameIF.text.Length > 0)
        {
            LoadingPanel.Instance.Init();

            await initializeServices.Init();

            LoadingPanel.isReady = true;

            PlayerManager.Instance.playerData.name = playerNameIF.text;
            SceneSystem.Instance.LoadScene(SceneName.ConectionMenu);
        }
        else
        {
            // Show error UI
        }
    }
}
