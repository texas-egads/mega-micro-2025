using UnityEngine;

public class TimerUI1 : MonoBehaviour
{
    void Start()
    {
        float ms = PlayerPrefs.GetFloat("minigameLength", 5143);
        if (ms == 0) ms = 16750;
        GetComponent<Animator>().speed = 4000 / ms;
    }
}
