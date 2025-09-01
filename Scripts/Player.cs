using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Player : MonoBehaviourPunCallbacks, IPunOwnershipCallbacks
{
    private Camera myCamera;
    [SerializeField] GameObject myChevron;
    [SerializeField] OwnershipButton ownershipButton;
    public Marble[] myMarbles;
    public Marble marbleInMotion;
    public Photon.Realtime.Player photonPlayer;

    [SerializeField] string color;
    public bool isMyTurn = false;
    public bool isTurnLegal = false;
    [SerializeField] UnityEngine.UI.Button endTurnButton;
    [SerializeField] public Zone myZone;

    private void Awake()
    {
        if (PlayerPrefs.GetInt("PlayingOnline") == 1)
        {
            PhotonNetwork.AddCallbackTarget(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        myMarbles = GetComponentsInChildren<Marble>();
        myCamera = Camera.main;
        endTurnButton.gameObject.SetActive(false);
        myZone.SetEndZoneColor(myChevron.GetComponent<SpriteRenderer>().color);
    }

    void Update()
    {
        if (isMyTurn)
        {
            if (marbleInMotion && marbleInMotion.active)
            {
                marbleInMotion.transform.parent.position = GetMousePosition();
            }
            if (marbleInMotion.active)
            {
                endTurnButton.gameObject.SetActive(false);
            }
            else
            {
                endTurnButton.gameObject.SetActive(isTurnLegal);
            }
        }
    }

    private Vector3 GetMousePosition()
    {
        var mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        var worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        return new Vector3(worldPos.x, worldPos.y, -1);
    }

    public void OnOwnershipRequest(PhotonView targetView, Photon.Realtime.Player requestingPlayer)
    {
        Debug.Log("Ownership request from: " + requestingPlayer.NickName);
        if (!this)
        {
            return;
        }
        // this check is supposedly necessary because OnOwnershipRequest gets called for all objects implementing IPunOwnershipCallbacks interface.
        if (targetView != photonView)
        {
            return;
        }
        if (targetView.Owner == null)
        {
            Debug.Log("Transferring ownership to: " + requestingPlayer.NickName);
            photonView.TransferOwnership(requestingPlayer);
            foreach(Marble marble in myMarbles)
            {
                marble.photonView.TransferOwnership(requestingPlayer);
            }
            SetPhotonPlayer(requestingPlayer);
        }
    }

    public void OnOwnershipTransfered(PhotonView targetView, Photon.Realtime.Player previousOwner)
    {
        Debug.Log("targetView AmOwner is now " + targetView.AmOwner);
        Debug.Log("targetView IsMine is now " + targetView.IsMine);
        Debug.Log("targetView owner is now " + targetView.Owner);
    }

    public void RequestOwnership()
    {
        if (photonView.Owner == null)
        {
            photonView.RequestOwnership();
            ownershipButton.Disable();
        }
    }

    private void SetPhotonPlayer(Photon.Realtime.Player player)
    {
        photonView.RPC("SetPhotonPlayerForAll", RpcTarget.AllBufferedViaServer, player);
    }

    [PunRPC]
    private void SetPhotonPlayerForAll(Photon.Realtime.Player player)
    {
        photonPlayer = player;
    }

    public string GetColor()
    {
        return color;
    }

    public GameObject GetChevron()
    {
        return myChevron;
    }

    public void DeactivateMarbleInMotion()
    {
        if (PlayerPrefs.GetInt("PlayingOnline") == 1)
        {
            photonView.RPC("DeactivateMarbleInMotionForAll", RpcTarget.All);
        }
        else
        {
            DeactivateMarbleInMotionForAll();
        }
    }

    [PunRPC]
    public void DeactivateMarbleInMotionForAll()
    {
        marbleInMotion.singleHopping = false;
        marbleInMotion.singleHopped = false;
        marbleInMotion.doubleHopping = false;
        marbleInMotion.doubleHopped = false;
        marbleInMotion.transform.parent.position = new Vector3(marbleInMotion.transform.parent.position.x, marbleInMotion.transform.parent.position.y, 0);
        marbleInMotion.particles.Stop();
        marbleInMotion.originalPit.DeactivateParticles();
        marbleInMotion = null;
        isTurnLegal = false;
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        if (otherPlayer == photonPlayer)
        {
            ownershipButton.Enable();
            // need to deactivate and reset the position of marbleInMotion of the player who left.
            if (PhotonNetwork.IsMasterClient)
            {
                if (marbleInMotion && marbleInMotion.originalPit)
                {
                    marbleInMotion.transform.position = new Vector3(
                        marbleInMotion.originalPit.transform.position.x,
                        marbleInMotion.originalPit.transform.position.y,
                        0
                    );
                    DeactivateMarbleInMotion();
                }
            }
        }
    }
}
