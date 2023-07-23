using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/GameSetting")]
public class GameSetting : ScriptableObject
{
    [Range(15, 30)] public int enterInitialThemeTime;
    [Range(5, 30)]  public int showInitialThemeTime;
    [Range(15, 30)] public int enterPhraseTime;
    [Range(5, 30)]  public int showPhrasesTime;
    [Range(15, 30)] public int guessTime;
    [Range(5, 30)]  public int showGuessesTime;
    [Range(5, 30)]  public int showScoreTime;

    public GameSettingSerializable GetSerializable()
    {
        GameSettingSerializable setting = new GameSettingSerializable();

        setting.enterInitialThemeTime = enterInitialThemeTime;
        setting.showInitialThemeTime = showInitialThemeTime;
        setting.enterPhraseTime = enterPhraseTime;
        setting.showPhrasesTime = showPhrasesTime;
        setting.guessTime = guessTime;
        setting.showGuessesTime = showGuessesTime;
        setting.showScoreTime = showScoreTime;

        return setting;
    }
}
