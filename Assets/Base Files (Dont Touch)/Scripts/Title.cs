using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Title : MonoBehaviour
{
    public Image fader;
    public float fadeSpeed;
    public float[] difficultyStarts;
    public float[] difficultyEnds;
    private bool canScene;
    private void Start()
    {
        canScene = true;
        StartCoroutine("Unfade");
    }
    private IEnumerator Unfade()
    {
        float timer = 0;
        while (timer <= 1)
        {
            fader.color = Color.Lerp(Color.black, Color.clear, timer);
            timer += Time.deltaTime * fadeSpeed;
            yield return null;
        }
        fader.gameObject.SetActive(false);
    }
    public void StartGame()
    {
        if (!canScene) return;
        canScene = false;
        StartCoroutine("LoadTargetScene", "Main");
    }

    public void ShowCredits()
    {
        if (!canScene) return;
        canScene = false;
        StartCoroutine("LoadTargetScene", "TestCredits");
    }
    private IEnumerator LoadTargetScene(string name)
    {
        float timer = 0;
        fader.gameObject.SetActive(true);
        while (timer <= 1)
        {
            fader.color = Color.Lerp(Color.clear, Color.black, timer);
            timer += Time.deltaTime * fadeSpeed;
            yield return null;
        }
        SceneManager.LoadScene(name);
    }

    public void SetDifficulty(int diff)
    {
        PlayerPrefs.SetFloat("difficultyStart", difficultyStarts[(int)diff]);
        PlayerPrefs.SetFloat("difficultyEnd", difficultyEnds[(int)diff]);
    }
}
