using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatCanvas : MonoBehaviour
{
    public GameObject scrollView;
    public GameObject inputField;
    [SerializeField] GameObject chatButton;
    private Image chatIcon;
    private bool flash;

    // Awake is called before Start
    private void Awake()
    {
        if(PlayerPrefs.GetInt("PlayingOnline") == 0)
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        chatIcon = chatButton.GetComponent<Image>();
        ToggleChat();
        TurnIconWhite();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (inputField.gameObject.activeSelf)
            {
                ToggleChat();
            }
        }
    }

    public void ToggleChat()
    {
        scrollView.gameObject.SetActive(!scrollView.gameObject.activeSelf);
        inputField.gameObject.SetActive(!inputField.gameObject.activeSelf);
        TurnIconWhite();
    }

    public bool IsActive()
    {
        return scrollView.gameObject.activeSelf;
    }

    public void TurnIconWhite()
    {
        flash = false;
        chatIcon.color = new Color(1, 1, 1, 1);
    }

    public void TurnIconBlue()
    {
        chatIcon.color = new Color(0.45f, 0.85f, 1, 1);
        StartCoroutine(FlashIcon());
    }

    public IEnumerator FlashIcon()
    {
        flash = true;
        float a = 1;
        bool decrement = true;
        while (flash)
        {
            if (a >= 1)
            {
                decrement = true;
            }
            else if (a < 0.5f)
            {
                decrement = false;
            }
            if (decrement)
            {
                a -= 0.04f;
            }
            else
            {
                a += 0.04f;
            }
            chatIcon.color = new Color(chatIcon.color.r, chatIcon.color.g, chatIcon.color.b, a);
            yield return new WaitForSeconds(0.05f);
        }
    }

}
