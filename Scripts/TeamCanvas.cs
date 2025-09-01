using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamCanvas : MonoBehaviour
{
    private Text[] teamTexts;

    // Start is called before the first frame update
    void Start()
    {
        teamTexts = GetComponentsInChildren<Text>();
        StartCoroutine(FadeText());    
    }

    private IEnumerator FadeText()
    {
        yield return new WaitForSeconds(8);
        float opacity = 1.0f;
        while (opacity > 0)
        {
            opacity -= 0.02f;
            foreach (Text text in teamTexts)
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, opacity);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}
