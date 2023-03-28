using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using UnityEngine.UI;

public class LobbyItem : MonoBehaviour
{

    public string lobbyName;
    public Text lobbyNameText;
    public Text playerCountText;
    public int currentPlayerCount;
    public CSteamID lobbyID;

    public MenuUI menuUI;


    public void SetLobby(string _lobbyName, CSteamID _lobbyID, MenuUI _menuUI)
    {
        lobbyName = _lobbyName;
        lobbyID = _lobbyID;
        menuUI = _menuUI;
        currentPlayerCount = SteamMatchmaking.GetNumLobbyMembers(lobbyID);

        lobbyNameText.text = lobbyName;
        playerCountText.text = currentPlayerCount.ToString() + "/4";
    }

    public void JoinLobbyOnclick()
    {
        SteamMatchmaking.JoinLobby(lobbyID);
    }
}