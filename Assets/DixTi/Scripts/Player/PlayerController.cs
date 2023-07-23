using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{
    private string phrase = "";

    public bool isReady { get; private set; }

    public int score { get; private set; }

    public void SavePhrase(string text)
    {
        phrase = text;
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            PlayerManager.Instance.controller = this;
            PlayerManager.Instance.playerData.clientId = OwnerClientId;
            ClientConnectedServerRpc(PlayerManager.Instance.playerData);

            GameplayManager.Instance.phraseInputField.onValueChanged.AddListener(SavePhrase);
        }
    }

    [ServerRpc]
    private void ClientConnectedServerRpc(PlayerData playerData)
    {
        RoomManager.Instance.ClientConnected(playerData);
    }

    public void ChooseTheme(ImageData theme)
    {
        ChooseThemeServerRpc(theme.type);
    }

    [ServerRpc]
    private void ChooseThemeServerRpc(int theme)
    {
        GameplayManager.Instance.SetTheme(theme);
    }

    public void Ready()
    {
        isReady = !isReady;

        if (isReady)
        {
            switch (SceneSystem.Instance.activeScene)
            {
                case SceneName.EnterPhrase:
                    SendPhraseServerRpc(OwnerClientId, phrase);
                    break;
            }
            ReadyServerRpc(OwnerClientId);
        }
        else
        {
            NotReadyServerRpc(OwnerClientId);
        }
    }

    public void SetNotReady()
    {
        isReady = false;
    }

    [ServerRpc]
    private void SendPhraseServerRpc(ulong clientId, string phrase)
    {
        GameplayManager.Instance.SavePhrase(clientId, phrase);
    }

    public void SendScore()
    {
        SendScoreServerRpc(OwnerClientId, score);
    }

    [ServerRpc]
    public void SendScoreServerRpc(ulong clientId, int score)
    {
        GameplayManager.Instance.SetScore(clientId, score);
    }

    [ServerRpc]
    public void ReadyServerRpc(ulong clientId)
    {
        GameplayManager.Instance.PlayerReady(clientId);
    }

    [ServerRpc]
    public void NotReadyServerRpc(ulong clientId)
    {
        GameplayManager.Instance.PlayerNotReady(clientId);
    }

    public void AddScore(int addedScore)
    {
        score += addedScore;
    }

    public void ResetScore()
    {
        score = 0;
    }
}
