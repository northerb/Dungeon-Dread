using MLAPI;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;
using MLAPI.Transports.SteamP2P;
using UnityEngine.Audio;
using System.Collections.Generic;
public class MenuUI : MonoBehaviour
{

    #region Networking

    [Header("Networking")]
    public InputField nameInputField;
    public Text playerText;
    public Text connectingText;
    public GameObject connectingPanel;
    public Dropdown lobbyTypeDropdown;

    List<CSteamID> lobbyIDList = new List<CSteamID>();


    public InputField searchInputField;
    public InputField lobbyNameInputField;
    public Transform lobbyItemContainer;
    public GameObject lobbyItemPrefab;

    LobbyItem[] lobbyItems;

    private const string HostAddressKey = "HostAddress";

    CSteamID lobbyID;
    CSteamID hostSteamID;

    protected Callback<LobbyDataUpdate_t> lobbyDataUpdate;
    protected Callback<LobbyMatchList_t> lobbyMatchList;
    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> gameLobbyEntered;

    private void Start()
    {
        //Settings
        //resolutions = Screen.resolutions;
        //resolutionDropdown.ClearOptions();

        //List<string> options = new List<string>();

        //int currentResolutionIndex = 0;
        //for (int i = 0; i < resolutions.Length; i++)
        //{
        //    string option = resolutions[i].width + "x" + resolutions[i].height + " @" + resolutions[i].refreshRate + "hz";
        //    options.Add(option);

        //    if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height && Screen.currentResolution.refreshRate == resolutions[i].refreshRate)
        //    {
        //        currentResolutionIndex = i;
        //    }
        //}
        //resolutionDropdown.AddOptions(options);
        //resolutionDropdown.value = currentResolutionIndex;
        //resolutionDropdown.RefreshShownValue();



        //Networking
        if (!SteamManager.Initialized) return;

        playerText.text = SteamFriends.GetPersonaName();
        nameInputField.text = SteamFriends.GetPersonaName();
        lobbyNameInputField.text = SteamFriends.GetPersonaName() + "'s Game";

        lobbyDataUpdate = Callback<LobbyDataUpdate_t>.Create(OnLobbyDataUpdated);
        lobbyMatchList = Callback<LobbyMatchList_t>.Create(OnLobbyListObtained);
        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        gameLobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    public void RefreshLobbies()
    {
        if (!SteamManager.Initialized) return;
        SteamMatchmaking.RequestLobbyList();
    }


    public void ClientOnClick()
    {
        PlayerPrefs.SetString("PlayerName", nameInputField.text);

        ClientNetPortal.Instance.StartClient();
    } 

    public void HostOnClick()
    {
        PlayerPrefs.SetString("PlayerName", nameInputField.text);

        ELobbyType lobbyType = ELobbyType.k_ELobbyTypePublic;

        if (lobbyTypeDropdown.value == 1) lobbyType = ELobbyType.k_ELobbyTypeFriendsOnly;

        //switch (lobbyTypeDropdown.value)
        //{
        //    case 0:
        //        lobbyType = Steamworks.ELobbyType.k_ELobbyTypePublic;
        //        break;
        //    case 1:
        //        lobbyType = Steamworks.ELobbyType.k_ELobbyTypeFriendsOnly;
        //        break;
        //}

        //SceneFader.instance.FadeOut();

        SteamMatchmaking.CreateLobby(lobbyType, ServerNetPortal.Instance.maxPlayers);
    }


    void OnLobbyEntered(LobbyEnter_t callback)
    {
        if (NetworkManager.Singleton.IsHost) return;
        connectingPanel.SetActive(true);
        connectingText.text = "Establishing Connection...";

        lobbyID = (CSteamID)callback.m_ulSteamIDLobby;

        PersDataManager.instance.InLobby = true;
        PersDataManager.instance.lobbyID = lobbyID;

        if (callback.m_EChatRoomEnterResponse == 1)
            Debug.Log($"Successfully joined lobby {SteamMatchmaking.GetLobbyData((CSteamID)callback.m_ulSteamIDLobby, "name")}!");
        else
        {
            connectingPanel.SetActive(false);
            //Display "Failed to join lobby."
            Debug.Log("Failed to join lobby.");
            return;
        }
        


        PlayerPrefs.SetString("PlayerName", nameInputField.text);

        int playerCount = SteamMatchmaking.GetNumLobbyMembers((CSteamID)callback.m_ulSteamIDLobby);

        var ownerSteamID = SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)callback.m_ulSteamIDLobby, 0);
        hostSteamID = ownerSteamID;
        PersDataManager.instance.hostSteamID = hostSteamID;

        connectingText.text = "Joining" + SteamFriends.GetFriendPersonaName(hostSteamID) + "...";

        Debug.Log("Check");

        SteamMatchmaking.SetLobbyData((CSteamID)callback.m_ulSteamIDLobby, "name", lobbyNameInputField.text);

        NetworkManager.Singleton.GetComponent<SteamP2PTransport>().ConnectToSteamID = hostSteamID.m_SteamID;

        //SceneFader.instance.FadeOut();

        ClientNetPortal.Instance.StartClient();
    }
    void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }
    void OnLobbyCreated(LobbyCreated_t callback)
    {
        Debug.Log(callback.m_eResult);

        if (callback.m_eResult != EResult.k_EResultOK) return;

        GameNetPortal.Instance.StartHost();
        lobbyID = (CSteamID)callback.m_ulSteamIDLobby;

        SteamMatchmaking.SetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            HostAddressKey,
            SteamUser.GetSteamID().ToString()
        );

        PersDataManager.instance.InLobby = true;
        PersDataManager.instance.lobbyID = lobbyID;

        hostSteamID = SteamUser.GetSteamID();
        PersDataManager.instance.hostSteamID = hostSteamID;

    }


    void OnLobbyListObtained(LobbyMatchList_t result)
    {
        foreach(Transform child in lobbyItemContainer)
        {
            Destroy(child.gameObject);
        }
        
        lobbyIDList = new List<CSteamID>();
        lobbyItems = new LobbyItem[result.m_nLobbiesMatching];
        Debug.Log("Found " + result.m_nLobbiesMatching + " public lobbies!");

        for (int i = 0; i < result.m_nLobbiesMatching; i++)
        {
            CSteamID lobbyId = SteamMatchmaking.GetLobbyByIndex(i);
            lobbyIDList.Add(lobbyId);
            lobbyItems[i] = Instantiate(lobbyItemPrefab, lobbyItemContainer).GetComponent<LobbyItem>();
            SteamMatchmaking.RequestLobbyData(lobbyId);

            if (lobbyItems[i].lobbyName.ToLower().Contains(searchInputField.text.ToLower()))
            { 
                lobbyItems[i].gameObject.SetActive(true);
            }
            else
            {
                lobbyItems[i].gameObject.SetActive(false);
            }
        }

    }


    public void OpenBrowser()
    {
        RefreshLobbies();
    }

    void OnLobbyDataUpdated(LobbyDataUpdate_t result)
    {
        for (int i = 0; i < lobbyIDList.Count; i++)
        {
            if (lobbyIDList[i].m_SteamID == result.m_ulSteamIDLobby)
            {
                string lobbyName = SteamMatchmaking.GetLobbyData((CSteamID)lobbyIDList[i].m_SteamID, "name");
                lobbyItems[i].SetLobby(lobbyName, (CSteamID)result.m_ulSteamIDLobby, this);
            }
        }
    }
    #endregion


    private void OnApplicationQuit()
    {
        SteamAPI.Shutdown();
    }
    public void QuitOnClick()
    {
        //FadeOut
        Application.Quit();
    }


    #region Settings

    Resolution[] resolutions;

    [Header("Settings Menu")]
    //public WindowScript windowScript;
    public AudioMixer audioMixer;
    public Dropdown resolutionDropdown;
    public void SetFullscreen(bool isFullscreen)
    {
        if (isFullscreen)
        {
            //windowScript.OnNoBorderBtnClick();
            //windowScript.OnMaximizeBtnClick();
        }
        else
        {
            //windowScript.OnBorderBtnClick();
        }
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
    public void SetVolume(float volume)
    {
        //audioMixer.SetFloat("volume", volume);  ---------- Linear Method

        audioMixer.SetFloat("volume", Mathf.Log10(volume) * 20);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    #endregion
}
