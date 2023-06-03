using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{
    private MenuSystem menuSystem;
    private ConectionSystem conectionSystem;
    private GameSystem gameSystem;

    private bool isReadyPhrase = false;
    private string phrase = "";
    private bool isReadyGuess = false;

    public void OnClickPhraseButton()
    {
        if (gameSystem.inputField.text.Length > 0)
        {
            phrase = gameSystem.inputField.text;
            isReadyPhrase = !isReadyPhrase;
            gameSystem.inputField.interactable = !isReadyPhrase;
            CheckPlayersReadyServerRpc(isReadyPhrase, phrase);
        }
    }

    public void OnClickGuessButton()
    {
        isReadyGuess = !isReadyGuess;
        foreach (var a in gameSystem.guessPanel.GetComponentsInChildren<TMP_Dropdown>())
        {
            a.interactable = !isReadyGuess;
        }
        CheckPlayersReadyGuessServerRpc(isReadyGuess);
    }

    public override void OnNetworkSpawn()
    {
        GameObject globalSystem = GameObject.Find("GlobalSystem");
        menuSystem = globalSystem.GetComponent<MenuSystem>();
        conectionSystem = globalSystem.GetComponent<ConectionSystem>();
        gameSystem = globalSystem.GetComponent<GameSystem>();

        if (IsOwner)
        {
            gameSystem.okButtonPhrase.onClick.AddListener(OnClickPhraseButton);
            gameSystem.startGameButton.onClick.AddListener(OnClickStartGameButton);
            gameSystem.okButtonGuess.onClick.AddListener(OnClickGuessButton);
            SendNameServerRpc(OwnerClientId, conectionSystem.playerData);
            if (!IsHost) {
                gameSystem.startGameButton.interactable = false;
            }

            GuessField[] dropdownUI = gameSystem.guessPanel.GetComponentsInChildren<GuessField>(true);
            foreach (var dropdown in dropdownUI)
            {
                dropdown.dropdownUI.ClearOptions();
            }
        }
    }

    public void OnClickStartGameButton()
    {
        ChangePlayerSceneClientRpc(4);
    }

    [ServerRpc]
    private void SendNameServerRpc(ulong id, PlayerData data)
    {
        gameSystem.playerDatas[(int)id].name = data.name;

        foreach (var client in NetworkManager.ConnectedClientsIds)
        {
            UpdateNamesClientRpc((int)client, gameSystem.playerDatas[(int)client].name);
        }
    }

    [ClientRpc]
    private void UpdateNamesClientRpc(int player, string playerName)
    {
        TextMeshProUGUI text = gameSystem.playersPanel.transform.GetChild(player).GetComponent<TextMeshProUGUI>();
        text.gameObject.SetActive(true);
        text.text = playerName;
    }

    [ServerRpc]
    private void CheckPlayersReadyServerRpc(bool isReady, string phrase)
    {
        isReadyPhrase = isReady;
        this.phrase = phrase;
        foreach (var client in NetworkManager.ConnectedClientsList)
        {
            if (!client.PlayerObject.GetComponent<PlayerController>().isReadyPhrase) return;
        }
        foreach (var client in NetworkManager.ConnectedClients)
        {
            PlayerController player = client.Value.PlayerObject.GetComponent<PlayerController>();
            UpdateGuessClientRpc((int)client.Key, player.phrase, gameSystem.playerDatas[(int)client.Key].name);
        }
        ChangePlayerSceneClientRpc(5);
    }

    [ClientRpc]
    private void ChangePlayerSceneClientRpc(int num)
    {
        menuSystem.ChangeScene(num);
    }

    [ClientRpc]
    private void UpdateGuessClientRpc(int player, string text, string playerName)
    {
        GuessField container = gameSystem.guessPanel.transform.GetChild(player).GetComponent<GuessField>();
        container.textUI.text = text;

        GuessField[] dropdownUI = gameSystem.guessPanel.GetComponentsInChildren<GuessField>(true);
        foreach (var dropdown in dropdownUI)
        {
            dropdown.dropdownUI.options.Add(new TMP_Dropdown.OptionData(playerName));
        }
        container.gameObject.SetActive(true);
    }

    [ServerRpc]
    private void CheckPlayersReadyGuessServerRpc(bool isReady)
    {
        isReadyGuess = isReady;
        foreach (var client in NetworkManager.ConnectedClientsList)
        {
            if (!client.PlayerObject.GetComponent<PlayerController>().isReadyGuess) return;
        }
        ChangePlayerSceneClientRpc(2);
    }
}
