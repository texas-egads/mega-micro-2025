using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;

public class EncounterObject : MonoBehaviour
{
    public enum Object
    {
        CANDY,
        CONSOLE,
        PRESENT,
        SOCK,
        TEDYY 
    }
    [System.Serializable]
    public struct SpriteMapping
    {
        public bool type;
        public Object size;
        public Sprite sprite;
    }

    public Object myObject;
    public bool myType;
    public SpriteRenderer mySprite;

    public List<SpriteMapping> spriteDatabase;
    private static Dictionary<(Object, bool), Sprite> spriteMap;

    void Awake()
    {
        mySprite = GetComponent<SpriteRenderer>();

        if (spriteMap == null)
        {
            spriteMap = new Dictionary<(Object, bool), Sprite>();

            foreach (var mapping in spriteDatabase)
            {
                var combinedKey = (mapping.size, mapping.type);

                if (!spriteMap.ContainsKey(combinedKey))
                {
                    spriteMap.Add(combinedKey, mapping.sprite);
                }
                else
                {
                    Debug.LogWarning("Duplicate database entry for: " + mapping.size + " / " + mapping.type);
                }
            }
        }
   
    }

    public void Update()
    {
        Debug.Log("Assigned sprite: " + mySprite.sprite + " based on object: " + myObject + " and type: " + myType);
    }
    /*
    private void Start()
    {
        mySprite = GetComponent<SpriteRenderer>();
        var lookupKey = (myObject, myType);

        if (spriteMap.ContainsKey(lookupKey))
        {
            mySprite.sprite = spriteMap[lookupKey];
            //Debug.Log("Assigned sprite: " +  mySprite.sprite + " based on object: " + myObject + " and type: " + myType);

        }
        else
        {
            // Debug.LogError("Sprite for " + myObject + " / " + myType + " not found in database!", this.gameObject);
        }
    }
    */
    public void changeType(bool changeType)
    {
        myType = changeType;
        var lookupKey = (myObject, myType);
        if (spriteMap.ContainsKey(lookupKey))
        {
            mySprite.sprite = spriteMap[lookupKey];
            //Debug.Log("Assigned sprite: " +  mySprite.sprite + " based on object: " + myObject + " and type: " + myType);

        }
        else
        {
            // Debug.LogError("Sprite for " + myObject + " / " + myType + " not found in database!", this.gameObject);
        }
    }
    public void changeObject(Object changeObject)
    {
        myObject = changeObject;
        var lookupKey = (myObject, myType);
        if (spriteMap.ContainsKey(lookupKey))
        {
            mySprite.sprite = spriteMap[lookupKey];
            Debug.Log("Assigned sprite: " + mySprite.sprite + " based on object: " + myObject + " and type: " + myType);

        }
        else
        {
             Debug.LogError("Sprite for " + myObject + " / " + myType + " not found in database!", this.gameObject);
        }
    }

    public Sprite returnObjectTypeSprite()
    {
        return mySprite.sprite;
    }

    public Sprite returnSpecificObjectType(Object typeObject)
    {
        Sprite spriteReturn;
        var lookupKey = (typeObject, true);

        if (spriteMap.ContainsKey(lookupKey))
        {
            spriteReturn = spriteMap[lookupKey];
            //Debug.Log("Assigned sprite: " +  mySprite.sprite + " based on object: " + myObject + " and type: " + myType);
            
        }
        else
        {
            Debug.LogError("Sending Sprite for " + typeObject + " / " + true + " not found in database! (Encounter Card Sprite Object not assigned)", this.gameObject);
            spriteReturn = mySprite.sprite;
        }
        return spriteReturn;
    }


}
