using UnityEngine;
using UnityEngine.UI;

public class Title : MonoBehaviour
{
    [SerializeField] private ScenesManager manager;
    [SerializeField] private GameObject diffSelect;
    [SerializeField] private GameObject creditScreen;
    [SerializeField] private GameObject returnButton;
    public static float difficulty;
    public void StartGame()
    {
        StartCoroutine(SelectDifficulty());
        manager.LoadSceneImmediate("Main");
    }

    public void ShowCredit()
    {
        creditScreen.SetActive(true);
        returnButton.SetActive(true);
    }

    public void returnTitle()
    {
        creditScreen.SetActive(false);
        returnButton.SetActive(false);
    }

    private System.Collections.IEnumerator SelectDifficulty()
    {
        diffSelect.SetActive(true);
        yield return null;
        EncounterCard hard = diffSelect.transform.GetChild(1).GetChild(0).GetComponent<EncounterCard>();
        EncounterCard normal = diffSelect.transform.GetChild(1).GetChild(1).GetComponent<EncounterCard>();
        EncounterCard easy = diffSelect.transform.GetChild(1).GetChild(2).GetComponent<EncounterCard>();

        hard.ResetSelection();
        normal.ResetSelection();
        easy.ResetSelection();

        yield return new WaitUntil(() => hard.IsCardSelected || normal.IsCardSelected || easy.IsCardSelected);
        if (hard.IsCardSelected)
        {
            difficulty = 1.2f;
        }
        else if (normal.IsCardSelected)
        {
            difficulty = 0.8f;
        }
        else
        {
            difficulty = 0f;
        }
        diffSelect.SetActive(false);
    }
}
