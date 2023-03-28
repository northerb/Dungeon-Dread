using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConnectionPayload
{
    public string clientGUID;
    public int clientScene = -1;
    public string playerName;
}
