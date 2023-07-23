using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public struct PlayerData : INetworkSerializable
{
    public string name;
    public int score;
    public ulong clientId;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref name);
        serializer.SerializeValue(ref score);
        serializer.SerializeValue(ref clientId);
    }
}
