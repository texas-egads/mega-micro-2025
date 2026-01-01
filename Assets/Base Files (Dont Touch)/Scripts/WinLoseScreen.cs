using UnityEngine;

public class WinLoseScreen : MonoBehaviour
{

    [SerializeField] private GameObject reindeerHappy;
    [SerializeField] private GameObject reindeerSad;

    [SerializeField] private GameObject gameOverSign;
    [SerializeField] private GameObject victorySign;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowWinScreen()
    {
        reindeerHappy.SetActive(true);
        reindeerSad.SetActive(false);

        victorySign.SetActive(true);
        gameOverSign.SetActive(false);
        
        gameObject.SetActive(true);
    }

    public void ShowLoseScreen()
    {
        reindeerHappy.SetActive(false);
        reindeerSad.SetActive(true);

        victorySign.SetActive(false);
        gameOverSign.SetActive(true);

        gameObject.SetActive(true);
    }

    public void PlayAgain()
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
