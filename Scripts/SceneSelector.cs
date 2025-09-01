using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSelector : MonoBehaviourPun
{
    [SerializeField] Dropdown gameTypeSelect;
    private bool playingOnline;
    [SerializeField] private Button backToSplashButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectGameType()
    {
        string gameType = gameTypeSelect.captionText.text;
        if (PlayerPrefs.GetInt(SplashPage.PLAYING_ONLINE_KEY) == 1)
        {
            photonView.RPC("LoadLevelForAll", RpcTarget.AllViaServer, gameType);
        }
        else
        {
            SceneManager.LoadScene(gameType);
        }
    }

    [PunRPC]
    private void LoadLevelForAll(string gameType)
    {
        PhotonNetwork.LoadLevel(gameType);
    }

    public void BackToSplashPage()
    {
        SceneManager.LoadScene(0);
    }
}
