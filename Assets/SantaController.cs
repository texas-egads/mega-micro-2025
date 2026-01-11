using UnityEngine;

public class SantaController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject santa;
    void Start()
    {
        if (Managers.__instance?.encounterManager.encounterCount == 14) santa.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
