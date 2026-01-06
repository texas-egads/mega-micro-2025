using UnityEngine;
using UnityEngine.UI;

public class MiniGameImageType : MonoBehaviour
{
    public Sprite[] images = new Sprite[5];
    public Image mySprite;

    public void Start()
    {
        mySprite = GetComponent<Image>();
        mySprite.sprite = images[4];
    }
    public void setSprite(Encounter.MinigameType setType)
    {
        switch (setType)
        {
            case Encounter.MinigameType.SPAM: mySprite.sprite = images[0]; break;
            case Encounter.MinigameType.PRECISION: mySprite.sprite = images[1]; break;
            case Encounter.MinigameType.TIMING: mySprite.sprite = images[2]; break;
            case Encounter.MinigameType.MOVEMENT: mySprite.sprite = images[3]; break;
            case Encounter.MinigameType.ALL: mySprite.sprite = images[4]; break;
        }
    }
}
