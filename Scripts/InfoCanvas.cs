using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoCanvas : MonoBehaviour
{
    private AudioSource musicAudioSource;
    private MusicPlayer musicPlayer;
    [SerializeField] AudioSource ambientAudioSource;
    [SerializeField] Slider volumeSlider;

    private Text[] myTextObjects;
    [SerializeField] Button[] myMusicButtons;
    [SerializeField] Button sparkleButton;
    [SerializeField] GameObject volumeIcon;
    [SerializeField] Image backgroundImage;
    [SerializeField] Toggle turnNotificationToggle;  // 'Game' object uses this to toggle playing sounds on nextTurn.
    [SerializeField] OwnershipButton[] ownershipButtons;
    [SerializeField] Text[] infoPlayerNames;
    [SerializeField] Text ChatInstructions;
    private BoxCollider2D myBoxCollider;
    private bool isEnabled;
    private bool playingOnline;
    public bool sparklesToggled = false;
    private float ambientVolumeScale = 0.8f;
    const string TURN_NOTIFICACTION_KEY = "NotifyMe";
    private Game game;

    // Awake is called before Start
    private void Awake()
    {
        if (PlayerPrefs.GetInt("PlayingOnline") == 1)
        {
            playingOnline = true;
        }
        else
        {
            playingOnline = false;
            turnNotificationToggle.gameObject.SetActive(false);
        }
        game = FindObjectOfType<Game>();
    }

    // Start is called before the first frame update
    void Start()
    {
        isEnabled = true;
        musicPlayer = FindObjectOfType<MusicPlayer>();
        musicAudioSource = musicPlayer.GetComponent<AudioSource>();
        volumeSlider.value = musicAudioSource.volume;
        ambientAudioSource.volume = musicAudioSource.volume * ambientVolumeScale;
        myBoxCollider = GetComponentInChildren<BoxCollider2D>();

        if (playingOnline)
        {
            ToggleOwnershipButtons();
            foreach (Text playerName in infoPlayerNames)
            {
                playerName.gameObject.SetActive(false);
            }
            if (PlayerPrefs.GetInt(TURN_NOTIFICACTION_KEY, 1) == 1)
            {
                turnNotificationToggle.isOn = true;
            }
            else
            {
                turnNotificationToggle.isOn = false;
            }
        }
        else
        {
            Destroy(ChatInstructions);

            foreach (Text playerName in infoPlayerNames)
            {
                Destroy(playerName);
            }
        }

        myTextObjects = GetComponentsInChildren<Text>();
    }

    private void Update()
    {
        musicAudioSource.volume = volumeSlider.value;
        ambientAudioSource.volume = volumeSlider.value * ambientVolumeScale;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (backgroundImage.enabled)
            {
                ToggleCanvasCollider();
                ToggleCanvasElements();
            }
        }
    }

    public void LoadNextTrack()
    {
        musicPlayer.LoadNextTrack();
    }

    public void LoadPreviousTrack()
    {
        musicPlayer.LoadPreviousTrack();
    }

    public void PlayAudio()
    {
        musicPlayer.PlayAudio();
    }

    public void StopAudio()
    {
        musicPlayer.StopAudio();
    }

    // used by toggle in unity
    public void SetNotificationPreference()
    {
        if (turnNotificationToggle.isOn)
        {
            PlayerPrefs.SetInt(TURN_NOTIFICACTION_KEY, 1);
        }
        else
        {
            PlayerPrefs.SetInt(TURN_NOTIFICACTION_KEY, 0);
        }
    }

    public void ToggleCanvasElements()
    {
        isEnabled = !isEnabled;
        backgroundImage.enabled = isEnabled;
        foreach (Text text in myTextObjects)
        {
            // in local play, player names are destroyed.
            if (text)
            {
                text.enabled = isEnabled;
            }
        }
        foreach(Button button in myMusicButtons)
        {
            button.gameObject.SetActive(isEnabled);
        }
        volumeIcon.SetActive(isEnabled);
        volumeSlider.gameObject.SetActive(isEnabled);
        if (playingOnline)
        {
            turnNotificationToggle.gameObject.SetActive(isEnabled);
            ToggleOwnershipButtons();
            foreach (Text playerName in infoPlayerNames)
            {
                playerName.gameObject.SetActive(isEnabled);
            }
        }
    }

    public void ToggleOwnershipButtons()
    {
        foreach (OwnershipButton ownershipButton in ownershipButtons)
        {
            if (ownershipButton.player.photonPlayer == null)
            {
                ownershipButton.gameObject.SetActive(!isEnabled);
            }
            else
            {
                ownershipButton.gameObject.SetActive(false);
            }
        }
    }

    public void ToggleCanvasCollider()
    {
        myBoxCollider.enabled = !myBoxCollider.enabled;
    }

    public void ToggleEndZoneEffect()
    {
        sparklesToggled = true;
        if (game.activePlayer.myZone.isPlayingParticles)
        {
            game.StopEndZoneEffect();
            DimSparkleButton();
        }
        else
        {
            game.PlayEndZoneEffect();
            BrightenSparkleButton();
        }
    }

    public void DimSparkleButton()
    {
        sparkleButton.image.color = new Color(1, 1, 1, 0.5f);
    }

    public void BrightenSparkleButton()
    {
        sparkleButton.image.color = new Color(1, 1, 1, 1);
    }
}

