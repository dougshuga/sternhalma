using System;

using UnityEngine;
using UnityEngine.UI;

using Photon.Chat;
using Photon.Realtime;
using AuthenticationValues = Photon.Chat.AuthenticationValues;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Collections;

public class Chat : MonoBehaviour, IChatClientListener
{
    private Room room;
    public string userName;
    public ChatClient chatClient;
	private ChatChannel thisChannel;
	private string channelName;
	[SerializeField] InputField inputFieldChat;
	[SerializeField] Text currentChannelText;
    [SerializeField] ChatCanvas chatCanvas;
    private const string PLAYER_PREF_KEY = "UserName";
    private const string CHANNEL_NAME_KEY = "ChannelName";
    protected internal ChatAppSettings chatAppSettings;

    // Awake is called before Start
    private void Awake()
    {
        chatAppSettings = new ChatAppSettings
        {
            Server = PhotonNetwork.PhotonServerSettings.AppSettings.Server,
            Protocol = PhotonNetwork.PhotonServerSettings.AppSettings.Protocol,
            AppIdChat = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat,
            AppVersion = PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion,
            FixedRegion = PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion,
            NetworkLogging = PhotonNetwork.PhotonServerSettings.AppSettings.NetworkLogging,
        };
        if (PlayerPrefs.GetInt("PlayingOnline") == 0)
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        room = PhotonNetwork.CurrentRoom;
        if (!room.CustomProperties.ContainsKey(CHANNEL_NAME_KEY))
        {
            room.SetCustomProperties(new Hashtable { { CHANNEL_NAME_KEY, Guid.NewGuid().ToString() } });
        }
        chatClient = new ChatClient(this);

        #if !UNITY_WEBGL
        chatClient.UseBackgroundWorkerForSending = true;
        #endif
        userName = PlayerPrefs.GetString(PLAYER_PREF_KEY);
        chatClient.AuthValues = new AuthenticationValues(userName);
        chatClient.ConnectUsingSettings(chatAppSettings);
	}

    // Clients have to be connected before they can send their state, subscribe to channels and send any messages.
    public void OnConnected()
    {
        StartCoroutine(ConnectToChat());
    }

    private IEnumerator ConnectToChat()
    {
        while (!room.CustomProperties.ContainsKey(CHANNEL_NAME_KEY))
        {
            yield return null;
        }
        channelName = (string)room.CustomProperties[CHANNEL_NAME_KEY];
        chatClient.Subscribe(channelName, 0, 20);
        while (!chatClient.TryGetChannel(channelName, out thisChannel))
        {
            yield return null;
        }
    }

    // Update is called once per frame
    public void Update()
    {
        this.chatClient.Service();
    }

    public void OnEnterSend()
	{
		if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
		{
		    SendChatMessage(this.inputFieldChat.text);
			inputFieldChat.text = "";
            inputFieldChat.ActivateInputField();
		}
	}

	public void SendChatMessage(string inputLine)
	{
		if (string.IsNullOrEmpty(inputLine))
		{
			return;
		}
		chatClient.PublishMessage(this.channelName, inputLine);
	}

	public void OnGetMessages(string channelName, string[] senders, object[] messages)
	{
		if (this.channelName.Equals(channelName))
		{
            if (thisChannel != null)
            {
                // update text
                currentChannelText.text = thisChannel.ToStringMessages();

                // color chat icon for new unseen updates
                if (!chatCanvas.scrollView.activeSelf)
                {
                    chatCanvas.TurnIconBlue();
                }
            }
		}
	}

    public void OnSubscribed(string[] channels, bool[] results)
    {
        chatClient.PublishMessage(channels[0], "has joined the room.");
    }

    public void DebugReturn(ExitGames.Client.Photon.DebugLevel level, string message)
    {
        if (level == ExitGames.Client.Photon.DebugLevel.ERROR)
        {
            Debug.LogError(message);
        }
        else if (level == ExitGames.Client.Photon.DebugLevel.WARNING)
        {
            Debug.LogWarning(message);
        }
        else
        {
            Debug.Log(message);
        }
    }




    #region unused IChatClientListener overrides

    /// <summary>
    /// Disconnection happened.
    /// </summary>
    public void OnDisconnected()
    {

    }

    /// <summary>The ChatClient's state changed. Usually, OnConnected and OnDisconnected are the callbacks to react to.</summary>
    /// <param name="state">The new state.</param>
    public void OnChatStateChange(ChatState state)
    {

    }


    public void OnPrivateMessage(string sender, object message, string channelName)
    {

    }

    public void OnUnsubscribed(string[] channels)
    {

    }

    /// <summary>
    /// New status of another user (you get updates for users set in your friends list).
    /// </summary>
    /// <param name="user">Name of the user.</param>
    /// <param name="status">New status of that user.</param>
    /// <param name="gotMessage">True if the status contains a message you should cache locally. False: This status update does not include a message (keep any you have).</param>
    /// <param name="message">Message that user set.</param>
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {

    }

    /// <summary>
    /// A user has subscribed to a public chat channel
    /// </summary>
    /// <param name="channel">Name of the chat channel</param>
    /// <param name="user">UserId of the user who subscribed</param>
    public void OnUserSubscribed(string channel, string user)
    {

    }

    /// <summary>
    /// A user has unsubscribed from a public chat channel
    /// </summary>
    /// <param name="channel">Name of the chat channel</param>
    /// <param name="user">UserId of the user who unsubscribed</param>
    public void OnUserUnsubscribed(string channel, string user)
    {

    }

    #endregion
}
