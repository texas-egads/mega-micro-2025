using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreditsPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private TextMeshProUGUI bodyText;
    [SerializeField] private Image screenshotImage;

    public void SetMinigame(MinigameDefinition minigame) {
        SetContent(minigame.title, minigame.creditsText, minigame.minigameScreenshot);
    }

    public void SetContent(string header, string body, Sprite screenshot) {
        headerText.text = header;
        bodyText.text = body;

        if (screenshot != null) {
            screenshotImage.sprite = screenshot;
        }
    }

}
