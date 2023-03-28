using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAPI.NetworkVariable.Collections;
using MLAPI;
using System;
using MLAPI.Connection;
using MLAPI.Messaging;
using Steamworks;
public class LobbyUI : NetworkBehaviour
{
    [Header("References")]
    public MenuPlayerUI[] menuPlayers;
    public Button startButton;

    CSteamID lobbyID;
    CSteamID hostSteamID;

    private NetworkList<LobbyPlayerState> lobbyPlayers = new NetworkList<LobbyPlayerState>();

    public void InviteFriendsToLobbyOnClick()
    {
        SteamFriends.ActivateGameOverlayInviteDialog(PersDataManager.instance.lobbyID);
    }
    public override void NetworkStart()
    {
        if (IsClient)
        {
            lobbyPlayers.OnListChanged += HandleLobbyPlayersStateChanged;
        }

        if (IsServer)
        {
            startButton.gameObject.SetActive(true);

            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnect;
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;

            foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
            {
                HandleClientConnect(client.ClientId);
            }
        }
    }

    private void OnDestroy()
    {
        lobbyPlayers.OnListChanged -= HandleLobbyPlayersStateChanged;

        if (NetworkManager.Singleton)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnect;
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
        }
    }

    private bool IsEveryoneReady()
    {
        if (lobbyPlayers.Count < 1)
        {
            return false;
        }

        foreach (var player in lobbyPlayers)
        {
            if (!player.isReady)
            {
                return false;
            }
        }
        return true;
    }
    private void HandleClientConnect(ulong clientID)
    {
        var playerData = ServerNetPortal.Instance.GetPlayerData(clientID);

        if (!playerData.HasValue) return;

        lobbyPlayers.Add(new LobbyPlayerState(
                clientID,
                playerData.Value._playerName,
                false
        ));

    }
    private void HandleClientDisconnect(ulong clientID)
    {
        for (int i = 0; i < lobbyPlayers.Count; i++)
        {
            if (lobbyPlayers[i].ClientId == clientID)
            {
                lobbyPlayers.RemoveAt(i);
                break;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ToggleReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        for (int i = 0; i < lobbyPlayers.Count; i++)
        {
            if(lobbyPlayers[i].ClientId == serverRpcParams.Receive.SenderClientId)
            {
                lobbyPlayers[i] = new LobbyPlayerState
                (
                    lobbyPlayers[i].ClientId,
                    lobbyPlayers[i].playerName,
                    !lobbyPlayers[i].isReady
                );
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        if (serverRpcParams.Receive.SenderClientId != NetworkManager.Singleton.LocalClientId) return;

        if (!IsEveryoneReady()) return;

        //SceneFader.instance.FadeOut();

        StartCoroutine(StartDelay());

    }

    IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(1f);

        ServerNetPortal.Instance.StartGame();

    }

    public void LeaveOnClick()
    {
        Debug.Log("Left On Click");
        if (!PersDataManager.instance.InLobby) return;
        Debug.Log("Not in Lobby");

        lobbyID = PersDataManager.instance.lobbyID;
        hostSteamID = PersDataManager.instance.hostSteamID;

        SteamMatchmaking.LeaveLobby(lobbyID);
        SteamNetworking.CloseP2PSessionWithUser(hostSteamID);
        if (IsServer)
            NetworkManager.Singleton.StopHost();
        else
            NetworkManager.Singleton.StopClient();

        //Wipe Data
        PersDataManager.instance.InLobby = false;
        PersDataManager.instance.hostSteamID = CSteamID.Nil;
        PersDataManager.instance.lobbyID = CSteamID.Nil;

        //SceneFader.instance.FadeOut();


        GameNetPortal.Instance.RequestDisconnect();
        
    }

    public void ReadyOnClick()
    {
        ToggleReadyServerRpc();
    }

    public void StartGameOnClick()
    {
        StartGameServerRpc();
    }
    private void HandleLobbyPlayersStateChanged(NetworkListEvent<LobbyPlayerState> lobbyState)
    {
        for (int i = 0; i < menuPlayers.Length; i++)
        {
            if(lobbyPlayers.Count > i)
            {
                menuPlayers[i].playerObject.SetActive(true);
                menuPlayers[i].UpdateGFX(lobbyPlayers[i]);
            }
            else
            {
                menuPlayers[i].DisableGFX();
            }
        }

        if (IsHost)
        {
            startButton.interactable = IsEveryoneReady();
        }
    }

    
}
