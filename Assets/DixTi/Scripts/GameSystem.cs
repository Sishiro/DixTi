using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameSystem : MonoBehaviour
{
    public enum State
    {
        Theme,
        Phrase,
        Guess
    }

    public State state;

    public PlayerData[] playerDatas = new PlayerData[8];

    [Header("Room")]
    public GameObject playersPanel;
    public Button startGameButton;

    [Header("Theme")]
    public Button okButtonTheme;

    [Header("Phrase")]
    public TMP_InputField inputField;
    public Button okButtonPhrase;

    [Header("Guess")]
    public GameObject guessPanel;
    public Button okButtonGuess;
}
