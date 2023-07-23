using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public struct GameSettingSerializable : INetworkSerializable
{
    public int enterInitialThemeTime;
    public int showInitialThemeTime;
    public int enterPhraseTime;
    public int showPhrasesTime;
    public int guessTime;
    public int showGuessesTime;
    public int showScoreTime;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref enterInitialThemeTime);
        serializer.SerializeValue(ref showInitialThemeTime);
        serializer.SerializeValue(ref enterPhraseTime);
        serializer.SerializeValue(ref showPhrasesTime);
        serializer.SerializeValue(ref guessTime);
        serializer.SerializeValue(ref showGuessesTime);
        serializer.SerializeValue(ref showScoreTime);
    }
}
