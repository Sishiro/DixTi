using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : SingletonNetwork<RoomManager>
{
    [SerializeField] private ConnectionSystem connectionSystem;

    [SerializeField] private GameplayManager gameplayManager;

    [Header("GUI")]
    public TMP_Text joinCodeTextUI;
    public GameObject playersPanel;
    public Button startGameButton;

    public int playerCount { get; private set; }

    public void ResetPlayersNames()
    {
        for (int i = 0; i < 8; ++i)
        {
            gameplayManager.playerStates[i].playerState = ConnectionState.disconnected;
            playersPanel.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public override void OnNetworkSpawn()
    {
        if (PlayerManager.Instance.playerData.clientId == 0)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += OnPlayerDisconnect;
        }
    }

    public void OnPlayerDisconnect(ulong clientId)
    {
        --playerCount;
        ClientDisconnectedClientRpc(GetPlayerId(clientId));

        if (clientId == 0)
        {
            connectionSystem.Disconnect();
        }
    }

    public int GetPlayerId(ulong clientId)
    {
        for (int i = 0; i < 8; i++)
        {
            if (gameplayManager.playerStates[i].clientId == clientId)
                return i;
        }

        //! This should never happen
        Debug.LogError("This should never happen");
        return -1;
    }

    public void OnClickStartGame()
    {
        GameplayManager.Instance.StartGame();
    }

    public void OnClickBack()
    {
        if (PlayerManager.Instance.playerData.clientId == 0)
        {
            StartCoroutine(HostShutdown());
        }
        else
        {
            Shutdown();
        }
    }

    IEnumerator HostShutdown()
    {
        ShutdownClientRpc();

        yield return new WaitForSeconds(0.5f);

        Shutdown();
    }

    private void Shutdown()
    {
        connectionSystem.Disconnect();
        SceneSystem.Instance.LoadScene(SceneName.ConectionMenu);
    }

    [ClientRpc]
    private void ShutdownClientRpc()
    {
        if (PlayerManager.Instance.playerData.clientId == 0) return;

        Shutdown();
    }

    public void ClientConnected(PlayerData playerData)
    {
        if (PlayerManager.Instance.playerData.clientId == 0)
        {
            ++playerCount;
            for (int i = 0; i < 8; ++i)
            {
                if (gameplayManager.playerStates[i].playerState == ConnectionState.disconnected)
                {
                    gameplayManager.playerStates[i].playerState = ConnectionState.connected;
                    gameplayManager.playerStates[i].playerName = playerData.name;
                    gameplayManager.playerStates[i].clientId = playerData.clientId;

                    SetPlayerName(i, playerData.name);

                    break;
                }
            }

            for (int i = 0; i < 8; ++i)
            {
                if (gameplayManager.playerStates[i].playerState != ConnectionState.disconnected)
                {
                    PlayerConnectsClientRpc(gameplayManager.playerStates[i].clientId, i, gameplayManager.playerStates[i].playerName, gameplayManager.playerStates[i].playerState);
                }
            }
        }
    }

    [ClientRpc]
    private void PlayerConnectsClientRpc(ulong clientId, int stateIndex, string playerName, ConnectionState state)
    {
        if (IsServer) return;

        gameplayManager.playerStates[stateIndex].playerState = state;
        gameplayManager.playerStates[stateIndex].clientId = clientId;
        gameplayManager.playerStates[stateIndex].playerName = playerName;

        SetPlayerName(stateIndex, playerName);
    }

    [ClientRpc]
    public void ClientDisconnectedClientRpc(int index)
    {
        gameplayManager.playerStates[index].playerState = ConnectionState.disconnected;
        DisablePlayerName(index);
    }

    private void SetPlayerName(int index, string playerName)
    {
        TextMeshProUGUI text = playersPanel.transform.GetChild(index).GetComponent<TextMeshProUGUI>();
        text.gameObject.SetActive(true);
        text.text = playerName;
    }

    private void DisablePlayerName(int index)
    {
        TextMeshProUGUI text = playersPanel.transform.GetChild(index).GetComponent<TextMeshProUGUI>();
        text.gameObject.SetActive(false);
    }
}
