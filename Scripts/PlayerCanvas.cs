using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCanvas : MonoBehaviour
{
    private void Awake()
    {
        if (PlayerPrefs.GetInt("PlayingOnline") == 0)
        {
            Destroy(gameObject);
        }
    }
}
