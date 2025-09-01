using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UIElements;


public class Marble : MonoBehaviourPunCallbacks
{
    public Player player;

    // state variables
    public CircleCollider2D myCircleCollider;
    public BoxCollider2D[] myBoxColliders;
    public EdgeCollider2D[] myEdgeColliders;
    public ParticleSystem particles;
    public bool singleHopping = false;
    public bool doubleHopping = false;
    public bool singleHopped = false;
    public bool doubleHopped = false;
    public bool playable = false;
    public bool active = false;  // whether the marble position will track with the mouse position.
    public Pit originalPit; // pit the marble occupied when the turn began.
    private Pit priorPit; // previous pit the marble occupied while moving in an particular turn.
    private Pit touchingPit; // pit the marble is in contact with.
    private int marbleLayer;

    public byte Id { get; set; }


    private void Awake()
    {
        if (PlayerPrefs.GetInt("PlayingOnline") == 1)
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        player = GetComponentInParent<Player>();
        myCircleCollider = GetComponent<CircleCollider2D>();
        myBoxColliders = GetComponentsInParent<BoxCollider2D>();
        myEdgeColliders = GetComponentsInParent<EdgeCollider2D>();
        particles = GetComponentInParent<ParticleSystem>();
    }

    // Start is called before the first frame update
    void Start()
    {   
        particles.Stop();
        marbleLayer = LayerMask.GetMask("Marbles");
        Debug.Log("Marble layer is: " + marbleLayer);
    }

    private void Update()
    {
        
    }

    private void OnMouseDown()
    {
        Debug.Log("OnMouseDown on marble called.");
        if (PlayerPrefs.GetInt("PlayingOnline") == 1)
        {
            if (!player.photonView.AmOwner)
            {
                Debug.Log("!player.photonView.AmOwner.");
                return;
            }
        }

        if (!player.isMyTurn || !touchingPit)
        {
            Debug.Log("Not my turn or not touching pit.");
            return;
        }

        if (!player.marbleInMotion)
        {
            SetMarbleInMotion();
        }

        if (player.marbleInMotion == this && !active)
        {
            Debug.Log("This is the marble in motion; setting as active.");
            active = true;
            priorPit = touchingPit;
            particles.Play();
            return;
        }

        else if (player.marbleInMotion == this && active)
        {
            Debug.Log("Trying to place marble.");
            if (playable)
            {
                Debug.Log("Marble is playable.");
                active = false;

                // back in original pit
                if (touchingPit == originalPit)
                {
                    Debug.Log("Setting marble in original pit.");
                    player.DeactivateMarbleInMotion();
                    if (priorPit)
                    {
                        priorPit.UnsetMarble();
                    }
                    originalPit.SetMarble();
                }

                // in a new pit
                else
                {
                    Debug.Log("Setting marble in a new pit.");
                    player.isTurnLegal = true;
                    originalPit.UnsetMarble();
                    if (priorPit && touchingPit != priorPit)
                    {
                        priorPit.UnsetMarble();
                    }
                    touchingPit.SetMarble();
                }
                
                priorPit = touchingPit;

                transform.parent.position = new Vector3(touchingPit.transform.position.x, touchingPit.transform.position.y, -1);

                if (singleHopping)
                {
                    Debug.Log("Setting singleHopped = true");
                    singleHopped = true;
                }
                if (doubleHopping)
                {
                    Debug.Log("Setting doubleHopped = true");
                    doubleHopped = true;
                }
            }
            else
            {
                Debug.Log("Marble is not playable.");
            }
        }
    }

    private void SetMarbleInMotion()
    {
        Debug.Log("No marble in motion: setting this as the marble in motion.");
        if (PlayerPrefs.GetInt("PlayingOnline") == 1)
        {
            photonView.RPC("SetMarbleInMotionForAll", RpcTarget.All);
        }
        else
        {
            SetMarbleInMotionForAll();
        }
        originalPit.ActivateParticles();
    }

    [PunRPC]
    private void SetMarbleInMotionForAll()
    {
        player.marbleInMotion = this;
        originalPit = touchingPit;
    }

    // detecting collisions with pits
    private void OnTriggerStay2D(Collider2D other)
    {
        var contactPit = other.gameObject.GetComponent<Pit>();
        if (contactPit && contactPit.myBoxColliders.All(boxCollider => boxCollider.IsTouching(myCircleCollider)))
        {
            touchingPit = contactPit;
        }

        if (!active)
        {
            return;
        }

        if (!player.marbleInMotion || player.marbleInMotion != this)
        {
            return;
        }

        if (!other.IsType<BoxCollider2D>())
        {
            Debug.Log("Other type was not BoxCollider.");
            return;
        }

        var pit = other.gameObject.GetComponent<Pit>();
        if (pit)
        {
            SetPlayableAndColor(false);
            // back in original pit?
            if (pit == originalPit)
            {
                Debug.Log("Touching original pit.");
                touchingPit = originalPit;
                SetPlayableAndColor(true);
                return;
            }
            // back in prior pit?
            else if (pit == priorPit)
            {
                Debug.Log("Touching prior pit.");
                touchingPit = priorPit;
                SetPlayableAndColor(true);
                return;
            }

            // unoccupied pit?
            if (!pit.myMarble)
            {
                singleHopping = false;
                doubleHopping = false;
                if (pit.myBoxColliders.All(boxCollider => boxCollider.IsTouching(myCircleCollider)))
                {
                    Debug.Log("Fully touching an unoccupied pit with my circle collider.");
                    touchingPit = pit;
                }
                else
                {
                    Debug.Log("Not fully touching an unoccupied pit with my circle collider.");
                    return;
                }
                // single hop?
                if (!singleHopped && !doubleHopped && myBoxColliders.Any(boxCollider => boxCollider.IsTouching(originalPit.GetComponent<CircleCollider2D>())))
                {
                    Debug.Log("Single hop possible.");
                    singleHopping = true;
                    doubleHopping = false;
                    SetPlayableAndColor(true);
                }
                // double hop?
                else if (!singleHopped)
                {
                    foreach (EdgeCollider2D edgeCollider in myEdgeColliders)
                    {
                        if (
                            edgeCollider.IsTouching(priorPit.GetComponent<CircleCollider2D>())
                            && edgeCollider.IsTouchingLayers(marbleLayer)
                            && !myBoxColliders.Any(boxCollider => boxCollider.IsTouching(priorPit.GetComponent<CircleCollider2D>()))
                        )
                        {
                            Debug.Log("Double hop possible.");
                            singleHopping = false;
                            doubleHopping = true;
                            SetPlayableAndColor(true);
                            break;
                        }
                    }
                }
            }
        }
    }

    public void TurnGray()
    {
        GetComponent<SpriteRenderer>().material.color = new Color(0.6f, 0.5f, 0.15f, 1);
        var p = particles.main;
        p.startColor = new Color(0.72f, 0, 0, 0.16f);
    }

    public void TurnWhite()
    {
        GetComponent<SpriteRenderer>().material.color = new Color(1, 1, 1, 1);
        var p = particles.main;
        p.startColor = new Color(0, 0.72f, 0, 0.1f);
    }

    public void SetPlayableAndColor(bool status)
    {
        playable = status;
        if (playable)
        {
            TurnWhite();
        }
        else
        {
            TurnGray();
        }
    }
}
