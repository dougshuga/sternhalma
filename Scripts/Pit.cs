using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class Pit : MonoBehaviourPun
{
    public BoxCollider2D[] myBoxColliders;
    public Marble myMarble;
    private Marble touchingMarble;
    public ParticleSystem originalPitParticles;
    public EndZoneParticles endZoneParticles;


    // Awake is called before start.
    private void Awake()
    {
        myBoxColliders = GetComponents<BoxCollider2D>();
        originalPitParticles = GetComponent<ParticleSystem>();
        originalPitParticles.Stop();
        endZoneParticles = GetComponentInChildren<EndZoneParticles>();
        StartCoroutine(SetMarbleOnFirstTurn());
    }

    private IEnumerator SetMarbleOnFirstTurn()
    {
        yield return new WaitForSeconds(1);
        myMarble = touchingMarble;
    }

    public void ActivateParticles()
    {
        if (PlayerPrefs.GetInt(SplashPage.PLAYING_ONLINE_KEY) == 1)
        {
            photonView.RPC("SetParticlesForAll", RpcTarget.All, true);
        }
        else
        {
            originalPitParticles.Play(withChildren: false);
        }
    }

    public void DeactivateParticles()
    {
        if (PlayerPrefs.GetInt(SplashPage.PLAYING_ONLINE_KEY) == 1)
        {
            photonView.RPC("SetParticlesForAll", RpcTarget.All, false);
        }
        else
        {
            originalPitParticles.Stop(withChildren: false);
        }
    }

    [PunRPC]
    private void SetParticlesForAll(bool play)
    {
        if (play)
        {
            originalPitParticles.Play(withChildren: false);
        }
        else
        {
            originalPitParticles.Stop(withChildren: false);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        var marble = other.gameObject.GetComponent<Marble>();
        if (marble)
        {
            touchingMarble = marble;
        }
    }

    public void SetMarble()
    {
        if (PlayerPrefs.GetInt(SplashPage.PLAYING_ONLINE_KEY) == 1)
        {
            photonView.RPC("SetMarbleForAll", RpcTarget.AllBufferedViaServer);
        }
        else
        {
            myMarble = touchingMarble;
        }
    }

    [PunRPC]
    private void SetMarbleForAll()
    {
        myMarble = touchingMarble;
    }

    public void UnsetMarble()
    {
        if (PlayerPrefs.GetInt(SplashPage.PLAYING_ONLINE_KEY) == 1)
        {
            photonView.RPC("UnsetMarbleForAll", RpcTarget.AllBufferedViaServer);
        }
        else
        {
            myMarble = null;
        }
    }

    [PunRPC]
    private void UnsetMarbleForAll()
    {
        myMarble = null;
    }

}
