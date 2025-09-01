using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashPage : MonoBehaviour
{
    public const string PLAYING_ONLINE_KEY = "PlayingOnline";

    public void LoadPhotonLauncher()
    {
        PlayerPrefs.SetInt(PLAYING_ONLINE_KEY, 1);
        SceneManager.LoadScene(1);
    }

    public void LoadBlokus()
    {
        PlayerPrefs.SetInt(PLAYING_ONLINE_KEY, 0);
        SceneManager.LoadScene(2);
    }
}
