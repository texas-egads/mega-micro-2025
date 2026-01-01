using UnityEngine;

public class MenuScreens : MonoBehaviour
{

    [SerializeField] private GameObject won;
    [SerializeField] private GameObject lost;
    private bool gameWon;

    private void Awake()
    {
        gameWon = true;

        if (gameWon)
        {
            ShowWinScreen();
        }
        else
        {
            ShowLoseScreen();
        }
    }

    private void ShowWinScreen()
    {
        won.SetActive(true);
        lost.SetActive(false);

        gameObject.SetActive(true);
    }

    private void ShowLoseScreen()
    {
        won.SetActive(false);
        lost.SetActive(true);

        gameObject.SetActive(true);
    }

    public void StartGame()
    {
        if (Managers.__instance)
        {
            Managers.__instance.scenesManager.LoadSceneImmediate("Main");
        }
        else
        {
            Debug.LogError("No Managers instance found! Cannot load Main scene.");
        }
    }

    public void Quit()
    {
        if (Managers.__instance)
        {
            Managers.__instance.scenesManager.LoadSceneImmediate("TitleScreen");
        }
        else
        {
            Debug.LogError("No Managers instance found! Cannot load TitleScreen scene.");
        }
    }
}
