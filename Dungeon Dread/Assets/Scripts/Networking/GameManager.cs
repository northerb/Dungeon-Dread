using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.Connection;
using MLAPI.NetworkVariable;
using MLAPI.NetworkVariable.Collections;
using System;

public class GameManager : NetworkBehaviour
{
    #region Singleton

    public static GameManager instance;



    #endregion

    public NetworkObject playerPrefab;

    public Transform[] playerSpawnPoints;

    public NetworkDictionary<ulong, GameObject> players = new NetworkDictionary<ulong, GameObject>();


    bool hasSpawned;
    private void Awake()
    {
        instance = this;

    }

    private void Start()
    {
        if (!IsOwner) return;

        SpawnPlayersServerRpc();

    }

    [ServerRpc]
    void SpawnPlayersServerRpc()
    {

        //Get player count
        Dictionary<ulong, NetworkClient>.KeyCollection keys = NetworkManager.Singleton.ConnectedClients.Keys;

        //For each player spawn a network object at a spawnpoint.
        int counter = 0;
        foreach (ulong clientID in keys)
        {
            //Debug.Log("HasClientKey");
            Transform spawnPoint = playerSpawnPoints[counter];

            NetworkObject playerInstance = Instantiate(playerPrefab, spawnPoint.position, Quaternion.Euler(spawnPoint.eulerAngles));

            playerInstance.SpawnAsPlayerObject(clientID, null, true);
            players.Add(clientID, playerInstance.gameObject);

            counter++;
        }
    }


    [ServerRpc]
    public void UnregisterPlayerServerRpc(ulong clientID)
    {
        Debug.Log("Remove Player");
        players.Remove(clientID);
    }

    private void OnEnable()
    {
        players.OnDictionaryChanged += OnPlayersChanged;
    }

    private void OnDisable()
    {
        players.OnDictionaryChanged -= OnPlayersChanged;
    }


    public GameObject[] GetOtherPlayers(ulong ownClientID)
    {
        if (players.Count <= 1) return null;

        GameObject[] playerArray = new GameObject[players.Count - 1];
        int counter = 0;
        foreach (ulong key in players.Keys)
        {
            if (key != ownClientID)
            {
                playerArray[counter] = players[key];
                counter++;
            }
        }
        return playerArray;
    }

    private void OnPlayersChanged(NetworkDictionaryEvent<ulong, GameObject> changeEvent)
    {
        if (!IsClient) return;
    }
}
