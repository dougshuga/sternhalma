using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Zone : MonoBehaviour
{
    [SerializeField] Pit[] myPits;
    [SerializeField] Player myPlayer;
    public bool isPlayingParticles = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    public bool AllPitsOccupied()
    {
        return myPits.All(pit => pit.myMarble && pit.myMarble.player == myPlayer);
    }

    public void SetEndZoneColor(Color color)
    {
        foreach (Pit pit in myPits)
        {
            pit.endZoneParticles.SetParticleColor(color);
        }
    }

    public void StartParticles()
    {
        isPlayingParticles = true;
        foreach(Pit pit in myPits)
        {
            pit.endZoneParticles.Play();
        }
    }

    public void StopParticles()
    {
        isPlayingParticles = false;
        foreach (Pit pit in myPits)
        {
            pit.endZoneParticles.Stop();
        }
    }
}