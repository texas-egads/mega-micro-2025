using UnityEngine;

public class MenuScreens : MonoBehaviour
{

    [SerializeField] private GameObject won;
    [SerializeField] private GameObject lost;
   
    public void ShowWinScreen()
    {
        won.SetActive(true);
        lost.SetActive(false);
    }

    public void ShowLoseScreen()
    {
        won.SetActive(false);
        lost.SetActive(true);
    }
    private bool beenPressed;
    public void StartGame()
    {
        if (beenPressed) return;
        beenPressed = true;
        if (Managers.__instance)
        {
            Managers.__instance.scenesManager.LoadSceneTransition("Main");
        }
        else
        {
            Debug.LogError("No Managers instance found! Cannot load Main scene.");
        }
    }

    public void Quit()
    {
        if (beenPressed) return;
        beenPressed = true;
        if (Managers.__instance)
        {
            Managers.__instance.scenesManager.LoadSceneTransition("TitleScreen");
        }
        else
        {
            Debug.LogError("No Managers instance found! Cannot load TitleScreen scene.");
        }
    }
}
