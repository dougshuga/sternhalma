using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicPlayer : MonoBehaviour
{
    AudioSource myAudioSource;
    [SerializeField] AudioClip[] audioClips;
    private int currentIndex;
    public bool play = true;

    // Awake is called before Start
    void Awake()
    {
        int musicPlayerCount = FindObjectsOfType<MusicPlayer>().Length;
        if (musicPlayerCount > 1)
        {
            gameObject.SetActive(false);  // onDestroy occurs after update.
            Destroy(gameObject);

        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        myAudioSource = GetComponent<AudioSource>();
        currentIndex = Random.Range(0, audioClips.Length);
        LoadNextTrack();
        PlayAudio();
    }

    private void Update()
    {
        if (play && !myAudioSource.isPlaying)
        {
            LoadNextTrack();
        }
    }

    public void LoadNextTrack()
    {
        if (currentIndex + 1 == audioClips.Length)
        {
            currentIndex = 0;
        }
        else
        {
            currentIndex += 1;
        }
        myAudioSource.clip = audioClips[currentIndex];
        PlayAudio();
    }

    public void LoadPreviousTrack()
    {
        if (currentIndex == 0)
        {
            currentIndex = audioClips.Length - 1;
        }
        else
        {
            currentIndex -= 1;
        }
        myAudioSource.clip = audioClips[currentIndex];
        PlayAudio();
    }

    public void PlayAudio()
    {
        play = true;
        myAudioSource.clip = audioClips[currentIndex];
        myAudioSource.Play();
    }

    public void StopAudio()
    {
        play = false;
        myAudioSource.Stop();
    }
}
