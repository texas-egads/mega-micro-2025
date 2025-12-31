using UnityEngine;

public class MenusManager : MonoBehaviour
{

    public void StartGame()
    {
        ChooseDiff();
        Managers.__instance.scenesManager.LoadSceneImmediate("Main");
    }

    public void QiutToMenu()
    {
        Managers.__instance.scenesManager.LoadSceneImmediate("TitleScreen");
    }

    public void ShowCredit()
    {

    }

    private void ChooseDiff()
    {

    }
}
