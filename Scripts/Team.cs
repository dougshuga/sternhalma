using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour
{
    private Player[] players;

    // Awake is called before Start
    private void Awake()
    {
        players = GetComponentsInChildren<Player>();
    }

    public Player[] GetPlayers()
    {
        return players;
    }
}
