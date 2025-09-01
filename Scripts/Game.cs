using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviourPunCallbacks
{
    [SerializeField] List<Team> teams;
    private List<Player> players = new List<Player>();
    private int activePlayerIndex;
    const string ACTIVE_PLAYER_INDEX_KEY = "activePlayerIndex";
    public Player activePlayer;
    private GameOverScreen gameOverCanvas;
    private Room room;
    private Chat chatRoom;
    private bool playingOnline;
    public int numPlayers;
    private Team winner;
    private int turnNumber = 1;
    private InfoCanvas infoCanvas;

    [SerializeField] AudioClip bell;
    [SerializeField] Toggle turnNotificationToggle;
    [SerializeField] UnityEngine.UI.Button endTurnButton;

    // Awake is called before Start
    private void Awake()
    {
        if (PlayerPrefs.GetInt(SplashPage.PLAYING_ONLINE_KEY) == 1)
        {
            playingOnline = true;
        }
        else
        {
            playingOnline = false;
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        infoCanvas = FindObjectOfType<InfoCanvas>();
        gameOverCanvas = FindObjectOfType<GameOverScreen>();
        gameOverCanvas.gameObject.SetActive(false);
        chatRoom = FindObjectOfType<Chat>();

        foreach(Team team in teams)
        {
            foreach(Player player in team.GetPlayers())
            {
                players.Add(player);
            }
        }

        numPlayers = players.Count;
        SortPlayers();
        
        foreach (Player player in players)
        {
            // make all chevrons invisible
            player.GetChevron().GetComponent<SpriteRenderer>().material.color = new Color(1, 1, 1, 0);
        }

        if (playingOnline)
        {
            room = PhotonNetwork.CurrentRoom;
            if (!room.CustomProperties.ContainsKey(ACTIVE_PLAYER_INDEX_KEY))
            {
                room.SetCustomProperties(new Hashtable { { ACTIVE_PLAYER_INDEX_KEY, Random.Range(0, numPlayers - 1) } });
            }
        }

        FirstTurn();
        StartCoroutine(CheckWinCondition());
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private void SortPlayers()
    {
        // this reshuffles the players list to alternate turns between teams in team games.

        int teamCount = teams.Count;
        if (teamCount == numPlayers)
        {
            return;
        }
        else
        {
            List<List<Player>> teamPlayerLists = new List<List<Player>>();

            for (int i = 0; i < teamCount; i++)
            {
                teamPlayerLists.Add(new List<Player>());
            }

            int playersPerTeam = numPlayers / teamCount;
            int playerIndex = 0;

            foreach(List<Player> teamList in teamPlayerLists)
            {
                for (int i = 0; i < playersPerTeam; i++)
                {
                    teamList.Add(players[playerIndex]);
                    playerIndex++;
                }
            }

            players.Clear();

            for (int i = 0; i < playersPerTeam; i++)
            {
                foreach (List<Player> teamList in teamPlayerLists)
                {
                    players.Add(teamList[i]);
                }
            }
        }
    }

    private IEnumerator CheckWinCondition()
    {
        while (!winner)
        {
            foreach (Team team in teams)
            {
                if (team.GetPlayers().All(player => player.myZone.AllPitsOccupied()))
                {
                    winner = team;
                    GameOver();
                    yield break;
                }
            }
            yield return new WaitForSeconds(1);
        }
    }

    public void FirstTurn()
    {
        StartCoroutine(ActivateFirstTurn());
    }

    private IEnumerator ActivateFirstTurn()
    {
        foreach (Player player in players)
        {
            player.isMyTurn = false;
        }
        if (playingOnline)
        {
            while (!room.CustomProperties.ContainsKey(ACTIVE_PLAYER_INDEX_KEY))
            {
                yield return null;
            }
            activePlayerIndex = (int)room.CustomProperties[ACTIVE_PLAYER_INDEX_KEY];
        }
        else
        {
            activePlayerIndex = Random.Range(0, numPlayers - 1);
        }
        activePlayer = players[activePlayerIndex];
        activePlayer.isMyTurn = true;
        Debug.Log("It's " + activePlayer.GetColor() + "'s turn!");
        PlayEndZoneEffect();
        StartCoroutine(ChevronBlink());
    }

    // this is called by the End Turn button.
    public void TriggerNextTurn()
    {
        activePlayer.DeactivateMarbleInMotion();
        if (playingOnline)
        {
            photonView.RPC("NextTurn", RpcTarget.AllViaServer);
        }
        else
        {
            NextTurn();
        }
    }

    [PunRPC]
    private void NextTurn()
    {
        foreach (Player player in players)
        {
            player.isMyTurn = false;
            endTurnButton.gameObject.SetActive(false);
        }

        StartCoroutine(ActivateNextTurn());
    }

    private IEnumerator ActivateNextTurn()
    {
        turnNumber += 1;

        bool sparklesWereOn = activePlayer.myZone.isPlayingParticles;
        StopEndZoneEffect();

        // calculate active player
        if (playingOnline)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                var index = (int)room.CustomProperties[ACTIVE_PLAYER_INDEX_KEY];
                if (index == (numPlayers - 1))
                {
                    index = 0;
                }
                else
                {
                    index += 1;
                }
                room.SetCustomProperties(new Hashtable { { ACTIVE_PLAYER_INDEX_KEY, index } });
            }
            while (activePlayerIndex == (int)room.CustomProperties[ACTIVE_PLAYER_INDEX_KEY])
            {
                yield return null;
            }
            activePlayerIndex = (int)room.CustomProperties[ACTIVE_PLAYER_INDEX_KEY];
        }
        else
        {
            if (activePlayerIndex == (numPlayers - 1))
            {
                activePlayerIndex = 0;
            }
            else
            {
                activePlayerIndex += 1;
            }
        }

        activePlayer = players[activePlayerIndex];

        // this is game over for the player. Skip their turn.
        if (activePlayer.myZone.AllPitsOccupied())
        {
            StartCoroutine(ActivateNextTurn());
            yield break;
        }

        activePlayer.isMyTurn = true;

        if (playingOnline)
        {
            if (activePlayer.photonPlayer != null && activePlayer.photonPlayer.IsLocal)
            {
                if (turnNotificationToggle.isOn)
                {
                    AudioSource.PlayClipAtPoint(bell, Camera.main.transform.position, .3f);
                }
            }
        }
        Debug.Log("It's " + activePlayer.GetColor() + "'s turn!");

        // end-zone lights logic
        if (turnNumber <= numPlayers)
        {
            if (sparklesWereOn)
            {
                PlayEndZoneEffect();
            }
        }
        else
        {
            if (!infoCanvas.sparklesToggled)
            {
                infoCanvas.DimSparkleButton();
            }
            else if (sparklesWereOn)
            {
                PlayEndZoneEffect();
            }
        }

        StartCoroutine(ChevronBlink());
    }

    public void PlayEndZoneEffect()
    {
        activePlayer.myZone.StartParticles();
    }

    public void StopEndZoneEffect()
    {
        activePlayer.myZone.StopParticles();
    }

    private IEnumerator ChevronBlink()
    {
        int index = activePlayerIndex;
        Player player = players[index];
        float alpha = 0;
        bool increment = false;
        while (activePlayerIndex == index)
        {
            if (alpha < 0.1f)
            {
                increment = true;
            }
            else if (alpha >= 0.95f)
            {
                increment = false;
            }
            if (increment)
            {
                alpha += Time.deltaTime;
            }
            else
            {
                alpha -= Time.deltaTime;
            }
            yield return player.GetChevron().GetComponent<SpriteRenderer>().material.color = new Color(1, 1, 1, alpha);
        }
        player.GetChevron().GetComponent<SpriteRenderer>().material.color = new Color(1, 1, 1, 0);
    }

    public void GameOver()
    {
        Debug.Log("Game Over!");
        activePlayerIndex = -1;
        gameOverCanvas.ShowWinner(winner);
        gameOverCanvas.gameObject.SetActive(true);
    }

    // currently unused in Sternhalma
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }

    // currently unused in Sternhalma
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player other)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            chatRoom.SendChatMessage(other.NickName + " left the room.");
        }
    }
}
