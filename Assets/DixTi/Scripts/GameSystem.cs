using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
    EnterTheme,
    ShowTheme,
    EnterPhrase,
    ShowPhrases,
    Guess
}

public class GameSystem : Singleton<GameSystem>
{
    public GameState state;

    public PlayerData[] playerDatas = new PlayerData[8];

    public GameSetting gameSetting;

    [Header("Room")]
    public GameObject playersPanel;
    public Button startGameButton;

    [Header("Theme")]
    public Button okButtonTheme;

    [Header("Phrase")]
    public TMP_Text phraseTimeText;
    public TMP_InputField phraseInputField;
    public Button phraseOkButton;

    [Header("Guess")]
    public TMP_Text guessTimeText;
    public GameObject guessPanel;
    public Button guessOkButton;
}
