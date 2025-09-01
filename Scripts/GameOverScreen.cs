using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.Linq;

public class GameOverScreen : MonoBehaviourPun
{
    [SerializeField] Text winningTeamText;

    public void ShowWinner(Team winningTeam)
    {
        IList<string> colors = new List<string>();
        foreach (Player player in winningTeam.GetPlayers())
        {
            colors.Add(player.GetColor());
        }
        winningTeamText.text = string.Join(" + ", colors) + " won!";
    }

    public void PlayAgain()
    {
        if (PlayerPrefs.GetInt(SplashPage.PLAYING_ONLINE_KEY) == 1)
        {
            photonView.RPC("LoadLevelForAll", RpcTarget.AllViaServer);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    [PunRPC]
    private void LoadLevelForAll()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        PhotonNetwork.LoadLevel(sceneName);
    }
}
