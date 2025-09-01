using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class OwnershipButton : MonoBehaviourPunCallbacks
{
    [SerializeField] public Player player;
    [SerializeField] PlayerName playerName;
    [SerializeField] Text infoCanvasPlayerNameText;
    private const string playerPrefKey = "UserName";

    // Start is called before the first frame update
    void Start()
    {
        if (player.photonPlayer == null)
        {
            playerName.playerNameText.text = "";
            infoCanvasPlayerNameText.text = "";
        }
    }

    public void Disable()
    {
        photonView.RPC("DisableForAll", RpcTarget.AllBufferedViaServer, PlayerPrefs.GetString(playerPrefKey));
    }

    [PunRPC]
    private void DisableForAll(string name)
    {
        playerName.FadePlayerName();
        playerName.playerNameText.text = name;
        infoCanvasPlayerNameText.text = name;
        gameObject.SetActive(false);        
    }

    public void Enable()
    {
        photonView.RPC("EnableForAll", RpcTarget.AllBufferedViaServer);
    }

    [PunRPC]
    private void EnableForAll()
    {
        player.photonPlayer = null;
        gameObject.SetActive(true);
        playerName.playerNameText.text = "";
        infoCanvasPlayerNameText.text = "";
        playerName.EnablePlayerName();
    }
}
