using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterObject : MonoBehaviour
{
    public enum Object
    {
        CANDY,
        CONSOLE,
        PRESENT,
        SOCK,
        TEDDY 
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

    private bool _isInitialized = false;
    
    void Awake()
    {
        mySprite = GetComponent<SpriteRenderer>();
        InitializeDatabase();
    }
    private void InitializeDatabase()
    {
        spriteMap = new Dictionary<(Object, bool), Sprite>();
        foreach (var mapping in spriteDatabase)
        {
            var combinedKey = (mapping.size, mapping.type);
            if (!spriteMap.ContainsKey(combinedKey))
            {
                spriteMap.Add(combinedKey, mapping.sprite);
            }
        }
    }

    void OnEnable()
    {
        if (spriteMap == null) InitializeDatabase();
        if (_isInitialized)
        {
            RefreshSprite();
        }
    }

    public void RefreshSprite()
    {
        if (mySprite == null) mySprite = GetComponent<SpriteRenderer>();
        var lookupKey = (myObject, myType);
        if (spriteMap.ContainsKey(lookupKey))
        {
            mySprite.sprite = spriteMap[lookupKey];
        }
    }

    public void changeType(bool changeType)
    {
        this.myType = changeType;
        RefreshSprite();
    }
    public void changeObject(Object changeObject)
    {
        this.myObject = changeObject;
        _isInitialized = true;
        RefreshSprite();
    }

    public Sprite returnObjectTypeSprite()
    {
        return mySprite.sprite;
    }

    public Sprite returnSpecificObjectTypeSprite(Object typeObject)
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
            //Debug.LogError("Sending Sprite for " + typeObject + " / " + true + " not found in database! (Encounter Card Sprite Object not assigned)", this.gameObject);
            spriteReturn = mySprite.sprite;
        }
        return spriteReturn;
    }


}
