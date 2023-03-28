using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI.Serialization;
public struct LobbyPlayerState : INetworkSerializable
{
    public ulong ClientId;
    public string playerName;
    public bool isReady;

    public LobbyPlayerState(ulong _clientId, string playername, bool isready)
    {
        ClientId = _clientId;
        playerName = playername;
        isReady = isready;
    }
    public void NetworkSerialize(NetworkSerializer serializer)
    {
        serializer.Serialize(ref ClientId);
        serializer.Serialize(ref playerName);
        serializer.Serialize(ref isReady);
    }
}
