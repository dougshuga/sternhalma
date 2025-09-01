using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class PhotonLauncher : MonoBehaviourPunCallbacks
{

    [Tooltip("The maximum number of players per room. When a room is full it can't be joined by new players, so a new room will be created.")]
    [SerializeField]
    private byte maxPlayersPerRoom = 8;

    [SerializeField] private Text connectingText;
    [SerializeField] private Text errorText;
    [SerializeField] private Button createRoomButton;
    [SerializeField] private InputField createRoomInput;
    [SerializeField] private Button createRoomSubmit;
    [SerializeField] private Button joinRoomButton;
    [SerializeField] private Button joinRoomSubmit;
    [SerializeField] private Dropdown joinRoomDropdown;
    private List<string> roomNames = new List<string>();
    [SerializeField] private Button backButton;
    [SerializeField] private Button backToSplashButton;
    [SerializeField] private InputField userNameInput;
    private const string PLAYER_PREF_KEY = "UserName";

    string gameVersion = "1";


    void Awake()
    {
        // this makes sure we can use PhotonNetwork.LoadLevel() on the ma gster client and all clients in the same room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        SetInitialState();

        userNameInput.text = PlayerPrefs.GetString(PLAYER_PREF_KEY);

        if (PhotonNetwork.IsConnected)
        {
            userNameInput.gameObject.SetActive(true);
            createRoomButton.gameObject.SetActive(true);
            joinRoomButton.gameObject.SetActive(true);
        }
        else
        {
            Connect();
        }

        
    }

    private void Update()
    {
        if (createRoomInput.text.Length == 0)
        {
            createRoomSubmit.interactable = false;
        }
        else
        {
            createRoomSubmit.interactable = true;
        }
        if (userNameInput.text.Length == 0)
        {
            createRoomButton.interactable = false;
            joinRoomButton.interactable = false;
        }
        else
        {
            createRoomButton.interactable = true;
            joinRoomButton.interactable = true;
        }
    }

    #region Public Methods

    // used by Start and after connection errors.
    public void SetInitialState()
    {
        backToSplashButton.gameObject.SetActive(true);
        userNameInput.gameObject.SetActive(false);
        connectingText.gameObject.SetActive(false);
        errorText.gameObject.SetActive(false);

        createRoomButton.gameObject.SetActive(false);
        joinRoomButton.gameObject.SetActive(false);

        createRoomInput.gameObject.SetActive(false);
        createRoomSubmit.gameObject.SetActive(false);

        joinRoomSubmit.gameObject.SetActive(false);
        joinRoomDropdown.gameObject.SetActive(false);

        backButton.gameObject.SetActive(false);
    }

    /* Start the connection process.
    - If already connected, we attempt joining a random room
    - if not yet connected, Connect this application instance to Photon Cloud Network. */
    public void Connect()
    {
        connectingText.gameObject.SetActive(true);
        // #Critical, we must first and foremost connect to Photon Online Server.
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = gameVersion;
    }

    public void JoinRoomOption()
    {
        PlayerPrefs.SetString(PLAYER_PREF_KEY, userNameInput.text);
        PhotonNetwork.LocalPlayer.NickName = userNameInput.text;
        PhotonNetwork.JoinLobby();

        errorText.gameObject.SetActive(false);
        userNameInput.gameObject.SetActive(false);
        createRoomButton.gameObject.SetActive(false);
        joinRoomButton.gameObject.SetActive(false);
        joinRoomSubmit.gameObject.SetActive(true);
        joinRoomDropdown.gameObject.SetActive(true);
        backToSplashButton.gameObject.SetActive(false);
        backButton.gameObject.SetActive(true);
    }

    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            RoomInfo roomInfo = roomList[i];
            if (roomInfo.RemovedFromList)
            {
                roomNames.Remove(roomInfo.Name);
            }
            else
            {
                if (!roomNames.Contains(roomInfo.Name))
                {
                    roomNames.Add(roomInfo.Name);
                }
            }
        }

        joinRoomDropdown.ClearOptions();
        joinRoomDropdown.AddOptions(roomNames);
        if (roomNames.Count == 0)
        {
            joinRoomDropdown.interactable = false;
            joinRoomSubmit.interactable = false;
        }
        else
        {
            joinRoomDropdown.interactable = true;
            joinRoomSubmit.interactable = true;
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        UpdateCachedRoomList(roomList);
    }

    public void JoinRoomSubmit()
    {
        joinRoomSubmit.interactable = false;
        PhotonNetwork.JoinRoom(joinRoomDropdown.options[joinRoomDropdown.value].text);
    }

    public void CreateRoomOption()
    {
        PlayerPrefs.SetString(PLAYER_PREF_KEY, userNameInput.text);
        PhotonNetwork.LocalPlayer.NickName = userNameInput.text;

        errorText.gameObject.SetActive(false);
        userNameInput.gameObject.SetActive(false);
        createRoomButton.gameObject.SetActive(false);
        joinRoomButton.gameObject.SetActive(false);
        createRoomInput.gameObject.SetActive(true);
        createRoomInput.ActivateInputField();
        createRoomSubmit.gameObject.SetActive(true);
        backToSplashButton.gameObject.SetActive(false);
        backButton.gameObject.SetActive(true);
    }

    public void CreateRoomSubmit()
    {
        createRoomSubmit.interactable = false;
        var roomName = createRoomInput.text;
        PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
    }

    public void Back()
    {
        userNameInput.gameObject.SetActive(true);
        createRoomButton.gameObject.SetActive(true);
        joinRoomButton.gameObject.SetActive(true);
        createRoomInput.gameObject.SetActive(false);
        createRoomSubmit.gameObject.SetActive(false);
        joinRoomSubmit.gameObject.SetActive(false);
        joinRoomDropdown.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);
        backToSplashButton.gameObject.SetActive(true);
    }

    public void BackToSplashPage()
    {
        SceneManager.LoadScene(0);
    }

    #endregion



    #region MonoBehaviourPunCallbacks Callbacks

    public override void OnConnectedToMaster()
    {
        connectingText.gameObject.SetActive(false);
        userNameInput.gameObject.SetActive(true);
        if (userNameInput.text.Length == 0)
        {
            userNameInput.ActivateInputField();
        }
        createRoomButton.gameObject.SetActive(true);
        joinRoomButton.gameObject.SetActive(true);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room.");
        // #Critical: We only load if we are the first player, else we rely on `PhotonNetwork.AutomaticallySyncScene` to sync our instance scene.
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            Debug.Log("Loading Scene Selector");
            PhotonNetwork.LoadLevel("Scene Selector");
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        SetInitialState();
        errorText.gameObject.SetActive(true);
        errorText.text = "Error: " + message;
        createRoomButton.gameObject.SetActive(true);
        joinRoomButton.gameObject.SetActive(true);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        SetInitialState();
        errorText.gameObject.SetActive(true);
        errorText.text = "Error: " + message;
        createRoomButton.gameObject.SetActive(true);
        joinRoomButton.gameObject.SetActive(true);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        errorText.gameObject.SetActive(true);
        errorText.text = cause.ToString();
        createRoomButton.gameObject.SetActive(false);
        joinRoomButton.gameObject.SetActive(false);
        createRoomInput.gameObject.SetActive(false);
        createRoomSubmit.gameObject.SetActive(false);
        connectingText.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);
        Connect();
    }

    #endregion

}