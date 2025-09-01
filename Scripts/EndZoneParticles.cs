using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndZoneParticles : MonoBehaviour
{
    private ParticleSystem particles;

    // Awake is called before Start
    void Awake()
    {
        particles = GetComponent<ParticleSystem>();
    }

    public void Play()
    {
        particles.Play();
    }

    public void Stop()
    {
        particles.Stop();
    }

    public void SetParticleColor(Color color)
    {
        // particles should be mostly transparent
        particles.startColor = new Color(color.r, color.g, color.b, 0.05f);
    }
}
