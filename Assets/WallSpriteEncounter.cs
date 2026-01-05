using UnityEngine;

public class SpriteEncounter : MonoBehaviour
{
    SpriteRenderer mySprite;
    [SerializeField]
    Sprite[] spritesAvailable = new Sprite[5];
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mySprite = GetComponent<SpriteRenderer>();
    }

    public void randomEncounterSprite(int num)
    {
        if (mySprite != null)
        {
            switch (num)
            {
                case 1: mySprite.sprite = spritesAvailable[0]; break;
                case 2: mySprite.sprite = spritesAvailable[1]; break;
                case 3: mySprite.sprite = spritesAvailable[2]; break;
                case 4: mySprite.sprite = spritesAvailable[3]; break;
                case 5: mySprite.sprite = spritesAvailable[4]; break;
            }
        }
        else
        {
            Debug.Log("mySprite is null in " + gameObject.name);
        }
        
    }
}
