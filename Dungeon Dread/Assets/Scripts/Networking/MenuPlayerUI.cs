using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAPI;
public class MenuPlayerUI : MonoBehaviour
{
    [Header("References")]
    public GameObject playerObject;

    [Header("UI")]
    public Text playerNameText;
    public Text playerReadyText;

    public void UpdateGFX(LobbyPlayerState lobbyPlayer)
    {
        playerNameText.text = lobbyPlayer.playerName;


        switch (lobbyPlayer.isReady) 
        {
            case true:
                playerReadyText.text = "Ready";
                break;
            case false:
                playerReadyText.text = "Not Ready";
                break;
        }
    }

    public void DisableGFX()
    {
        playerObject.SetActive(false);
    }

}
