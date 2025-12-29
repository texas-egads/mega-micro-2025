using UnityEngine;

public class TitleManager : MonoBehaviour
{

    public void startGame()
    {
        selectDifficulty();
        Managers.__instance.scenesManager.LoadSceneImmediate("Main");
    }

    public void quitGame()
    {
        Application.Quit();
    }

    public void seeCredits()
    {

    }

    private void selectDifficulty()
    {
        //bring up UI

    }

}
