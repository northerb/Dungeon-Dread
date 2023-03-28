using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class PersDataManager : MonoBehaviour
{
    #region Singleton

    public static PersDataManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);    
        }

    }
    #endregion

    public bool InLobby;

    public CSteamID lobbyID;
    public CSteamID hostSteamID;
}
