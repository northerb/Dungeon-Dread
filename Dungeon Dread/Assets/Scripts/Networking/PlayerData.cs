using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerData
{
    public string _playerName { get; private set; }
    public ulong clientID { get; private set; }

    public PlayerData(string playerName, ulong ClientID)
    {
        _playerName = playerName;
        clientID = ClientID;
    }
}
