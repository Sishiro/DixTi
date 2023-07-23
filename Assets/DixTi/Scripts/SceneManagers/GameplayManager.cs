using System.Collections.Generic;
using System.Threading;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public enum ConnectionState : byte
{
    disconnected,
    connected,
}

public enum Role : byte
{
    none,
    cuentaCuentos,
    adivinador,
}

[System.Serializable]
public struct PlayerConnectionState
{
    public ConnectionState playerState;
    public string playerName;
    public ulong clientId;
    public Role role;
    public int score;
}

public class GameplayManager : SingletonNetwork<GameplayManager>
{
    [SerializeField] private List<ImageData> images;

    public PlayerConnectionState[] playerStates;

    public int winnerPlayer;

    public string[] phrases;

    private Dictionary<string, string> playerPhrases;

    [SerializeField] private GameSetting gameSetting;

    private float timer;
    [SerializeField] private bool[] isReady;

    [Header("EnterTheme")]
    public List<Button> themeButtons;
    public ImageData selectedTheme;

    [Header("ShowTheme")]
    public Image selectedThemeImage;

    [Header("EnterPhrase")]
    public TMP_InputField phraseInputField;

    [Header("ShowPhrases")]
    public GameObject phrasesPanel;

    [Header("EnterGuess")]
    public GameObject guessPanel;

    [Header("ShowGuesses")]
    public GameObject showGuessesPanel;
    public GameObject symbolsPanel;
    public Sprite tick;
    public Sprite cross;

    [Header("Score")]
    public GameObject playerNames;
    public GameObject playerScores;

    [Header("Win")]
    public TMP_Text winnerPlayerName;

    public void StartGame()
    {
        ResetStateClientRpc();

        for (int i = 0; i < RoomManager.Instance.playerCount; ++i)
        {
            FillDropdownClientRpc(playerStates[i].playerName);
            FillPlayerScoreNamesClientRpc(i, playerStates[i].playerName);
        }

        int cuentaCuentos = Random.Range(0, RoomManager.Instance.playerCount);

        for (int i = 0; i < RoomManager.Instance.playerCount; ++i)
        {
            if (i == cuentaCuentos)
            {
                playerStates[i].role = Role.cuentaCuentos;
            }
            else
            {
                playerStates[i].role = Role.adivinador;
            }

            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { playerStates[i].clientId }
                }
            };

            StartGameClientRpc(playerStates[i].role, clientRpcParams);
        }
    }

    [ClientRpc]
    private void ResetStateClientRpc()
    {
        GuessField[] dropdownUI = guessPanel.GetComponentsInChildren<GuessField>(true);
        foreach (var dropdown in dropdownUI)
        {
            dropdown.dropdownUI.ClearOptions();
        }

        GuessField[] showDropdownUI = showGuessesPanel.GetComponentsInChildren<GuessField>(true);
        foreach (var dropdown in showDropdownUI)
        {
            dropdown.dropdownUI.ClearOptions();
        }
    }

    [ClientRpc]
    private void FillDropdownClientRpc(string playerName)
    {
        GuessField[] dropdownUI = guessPanel.GetComponentsInChildren<GuessField>(true);
        foreach (var dropdown in dropdownUI)
        {
            dropdown.dropdownUI.options.Add(new TMP_Dropdown.OptionData(playerName));
        }
        GuessField[] showDropdownUI = showGuessesPanel.GetComponentsInChildren<GuessField>(true);
        foreach (var dropdown in showDropdownUI)
        {
            dropdown.dropdownUI.options.Add(new TMP_Dropdown.OptionData(playerName));
        }
    }

    [ClientRpc]
    private void FillPlayerScoreNamesClientRpc(int index, string playerName)
    {
        playerNames.transform.GetChild(index).gameObject.SetActive(true);
        playerNames.transform.GetChild(index).GetComponent<TMP_Text>().text = playerName;
        playerScores.transform.GetChild(index).gameObject.SetActive(true);
    }

    [ClientRpc]
    private void StartGameClientRpc(Role role, ClientRpcParams clientRpcParams = default)
    {
        PlayerManager.Instance.controller.ResetScore();

        playerPhrases = new Dictionary<string, string>();

        if (role == Role.cuentaCuentos)
        {
            SceneSystem.Instance.LoadScene(SceneName.EnterTheme);
        }
        else
        {
            SceneSystem.Instance.LoadScene(SceneName.WaitForTheme);
        }
    }

    [ClientRpc]
    private void NextRoundClientRpc(Role role, ClientRpcParams clientRpcParams = default)
    {
        playerPhrases = new Dictionary<string, string>();

        if (role == Role.cuentaCuentos)
        {
            SceneSystem.Instance.LoadScene(SceneName.EnterTheme);
        }
        else
        {
            SceneSystem.Instance.LoadScene(SceneName.WaitForTheme);
        }
    }

    private void NextRound()
    {
        for (int i = 0; i < RoomManager.Instance.playerCount; ++i)
        {
            if (playerStates[i].role == Role.cuentaCuentos)
            {
                playerStates[i].role = Role.adivinador;

                if (i == RoomManager.Instance.playerCount - 1)
                {
                    playerStates[0].role = Role.cuentaCuentos;
                }
                else
                {
                    playerStates[i + 1].role = Role.cuentaCuentos;
                }
                break;
            }
        }

        for (int i = 0; i < RoomManager.Instance.playerCount; ++i)
        {
            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { playerStates[i].clientId }
                }
            };

            NextRoundClientRpc(playerStates[i].role, clientRpcParams);
        }
    }

    public void SetTheme(int type)
    {
        SetThemeClientRpc(type);

        ChangeSceneClientRpc(SceneName.ShowTheme);
    }

    [ClientRpc]
    private void SetThemeClientRpc(int type)
    {
        selectedTheme = images[type];
        selectedThemeImage.sprite = selectedTheme.sprite;
    }

    [ClientRpc]
    private void ChangeSceneClientRpc(SceneName scene)
    {
        SceneSystem.Instance.LoadScene(scene);
    }

    public void SavePhrase(ulong clientId, string phrase)
    {
        int index = RoomManager.Instance.GetPlayerId(clientId);
        phrases[index] = phrase;
    }

    public void PlayerReady(ulong clientId)
    {
        int index = RoomManager.Instance.GetPlayerId(clientId);
        isReady[index] = true;

        for (int i = 0; i < RoomManager.Instance.playerCount; ++i)
        {
            if (!isReady[i])
            {
                return;
            }
        }

        for (int i = 0; i < RoomManager.Instance.playerCount; ++i)
        {
            isReady[i] = false;
        }
        SetPlayerNotReadyClientRpc();

        if (SceneSystem.Instance.activeScene == SceneName.EnterPhrase)
        {
            for (int i = 0; i < RoomManager.Instance.playerCount; ++i)
            {
                SendPhraseClientRpc(playerStates[i].clientId, phrases[i]);
            }
        }
        else if (SceneSystem.Instance.activeScene == SceneName.EnterGuess)
        {
            for (int i = 0; i < RoomManager.Instance.playerCount; ++i)
            {
                PassDropdownTextsClientRpc(i);
            }
            for (int i = 0; i < RoomManager.Instance.playerCount; ++i)
            {
                CheckGuessesClientRpc(i);
            }
            CheckScoreClientRpc();
        }
        else if (SceneSystem.Instance.activeScene == SceneName.Win)
        {
            ChangeSceneClientRpc(SceneName.Room);
            return;
        }

        if (SceneSystem.Instance.activeScene != SceneName.ShowScore)
        {
            ChangeSceneClientRpc(SceneSystem.Instance.activeScene + 1);
        }
        else
        {
            if (CheckWinners())
            {
                SetWinnerClientRpc(playerStates[winnerPlayer].playerName);
                ChangeSceneClientRpc(SceneName.Win);
            }
            else
            {
                NextRound();
            }
        }
    }

    [ClientRpc]
    private void PassDropdownTextsClientRpc(int index)
    {
        GuessField[] guesses = guessPanel.GetComponentsInChildren<GuessField>(true);
        GuessField[] showed = showGuessesPanel.GetComponentsInChildren<GuessField>(true);

        showed[index].textUI.text = guesses[index].textUI.text;
        showed[index].dropdownUI.value = guesses[index].dropdownUI.value;
    }

    private bool CheckWinners()
    {
        for (int i = 0; i < RoomManager.Instance.playerCount; ++i)
        {
            if (playerStates[i].score >= 10)
            {
                winnerPlayer = i;
                return true;
            }
        }
        return false;
    }

    [ClientRpc]
    private void SetWinnerClientRpc(string playerName)
    {
        winnerPlayerName.text = playerName;
    }

    [ClientRpc]
    private void SendPhraseClientRpc(ulong clientId, string phrase)
    {
        int index = RoomManager.Instance.GetPlayerId(clientId);
        playerPhrases.Add(playerStates[index].playerName, phrase);
        SetPhrase(index, phrase);
        SetPhraseInGuessPanel(index, phrase);
    }

    private void SetPhrase(int player, string phrase)
    {
        TextMeshProUGUI text = phrasesPanel.transform.GetChild(player).GetComponent<TextMeshProUGUI>();
        text.gameObject.SetActive(true);
        text.text = phrase;
    }

    private void SetPhraseInGuessPanel(int player, string phrase)
    {
        GuessField container = guessPanel.transform.GetChild(player).GetComponent<GuessField>();
        container.textUI.text = phrase;
        container.gameObject.SetActive(true);

        GuessField container2 = showGuessesPanel.transform.GetChild(player).GetComponent<GuessField>();
        container2.textUI.text = phrase;
        container2.gameObject.SetActive(true);
    }

    [ClientRpc]
    private void SetPlayerNotReadyClientRpc()
    {
        PlayerManager.Instance.controller.SetNotReady();
    }

    public void PlayerNotReady(ulong clientId)
    {
        int index = RoomManager.Instance.GetPlayerId(clientId);
        isReady[index] = false;
    }

    public void OnClickReadyEvent(Button button)
    {
        PlayerManager.Instance.controller.Ready();
        if (PlayerManager.Instance.controller.isReady)
        {
            button.GetComponentInChildren<TMP_Text>().text = "Editar";
        }
        else
        {
            button.GetComponentInChildren<TMP_Text>().text = "Listo";
        }
    }

    [ClientRpc]
    private void CheckGuessesClientRpc(int index)
    {
        GuessField[] containers = showGuessesPanel.GetComponentsInChildren<GuessField>(true);
        foreach (var pair in playerPhrases)
        {
            if (containers[index].textUI.text.Equals(pair.Value))
            {
                Image symbol = symbolsPanel.transform.GetChild(index).GetComponent<Image>();
                if (containers[index].dropdownUI.options[containers[index].dropdownUI.value].text.Equals(pair.Key))
                {
                    symbol.sprite = tick;
                    PlayerManager.Instance.controller.AddScore(5);
                }
                else
                {
                    symbol.sprite = cross;
                }
                symbol.gameObject.SetActive(true);
            }
        }
    }

    [ClientRpc]
    private void CheckScoreClientRpc()
    {
        PlayerManager.Instance.controller.SendScore();
    }

    public void SetScore(ulong clientId, int score)
    {
        int index = RoomManager.Instance.GetPlayerId(clientId);
        playerStates[index].score = score;

        for (int i = 0; i < RoomManager.Instance.playerCount; ++i)
        {
            SetScoreClientRpc(i, playerStates[i].score);
        }
    }

    [ClientRpc]
    private void SetScoreClientRpc(int player, int score)
    {
        Transform playerScore = playerScores.transform.GetChild(player);
        playerScore.GetComponent<TMP_Text>().text = score.ToString();
        playerScore.gameObject.SetActive(true);
    }
}
