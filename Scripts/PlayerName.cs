using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerName : MonoBehaviour
{
    public Text playerNameText;

    void Awake()
    {
        playerNameText = GetComponent<Text>();
    }

    public void FadePlayerName()
    {
        StartCoroutine(Fade());
    }

    private IEnumerator Fade()
    {
        yield return new WaitForSeconds(3);
        while (playerNameText.color.a > 0.0f)
        {
            playerNameText.color = new Color(
                playerNameText.color.r,
                playerNameText.color.g,
                playerNameText.color.b,
                playerNameText.color.a - 0.01f
            );
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void EnablePlayerName()
    {
        playerNameText.color = new Color(
            playerNameText.color.r,
            playerNameText.color.g,
            playerNameText.color.b,
            1
        );
    }
}
