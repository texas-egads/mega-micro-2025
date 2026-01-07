using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreditsManager : MonoBehaviour
{
    public Image fader;
    public float fadeSpeed;
    private bool inChange;
    private void Start()
    {
        StartCoroutine("Unfade");
        panel.SetMinigame(panels[0]);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine("GoLeft");
        } else if (Input.GetKeyDown(KeyCode.D))
        {
            StartCoroutine("GoRight");
        }
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

    public RectTransform panelTransform;
    public CreditsPanel panel;
    public MinigameDefinition[] panels;
    public float range;
    public float loopTime;
    public AnimationCurve pos;
    private int panelIndex;

    public IEnumerator GoLeft()
    {
        if (!inChange)
        {
            inChange = true;
            // move out
            float timer = loopTime/2;
            while (timer >= 0)
            {
                timer -= Time.deltaTime;
                panelTransform.anchoredPosition = new Vector2(range * pos.Evaluate(timer / loopTime), panelTransform.anchoredPosition.y);
                yield return null;
            }

            // change
            panelIndex = (panelIndex + 1) % panels.Length;
            panel.SetMinigame(panels[panelIndex]);

            // move in again
            timer = loopTime;
            while (timer >= loopTime/2)
            {
                timer -= Time.deltaTime;
                panelTransform.anchoredPosition = new Vector2(range * pos.Evaluate(timer / loopTime), panelTransform.anchoredPosition.y);
                yield return null;
            }
            panelTransform.anchoredPosition = new Vector2(0, panelTransform.anchoredPosition.y);
            inChange = false;
        }
    }

    public IEnumerator GoRight()
    {
        if (!inChange)
        {
            inChange = true;
            // move out
            float timer = loopTime / 2;
            while (timer <= loopTime)
            {
                timer += Time.deltaTime;
                panelTransform.anchoredPosition = new Vector2(range * pos.Evaluate(timer / loopTime), panelTransform.anchoredPosition.y);
                yield return null;
            }

            // change
            panelIndex = (panelIndex - 1 + panels.Length) % panels.Length;
            panel.SetMinigame(panels[panelIndex]);

            // move in again
            timer = 0;
            while (timer <= loopTime / 2)
            {
                timer += Time.deltaTime;
                panelTransform.anchoredPosition = new Vector2(range * pos.Evaluate(timer / loopTime), panelTransform.anchoredPosition.y);
                yield return null;
            }
            panelTransform.anchoredPosition = new Vector2(0, panelTransform.anchoredPosition.y);
            inChange = false;
        }
    }
    private bool canScene = true;
    public void GoBack()
    {
        if (!canScene) return;
        canScene = false;
        StartCoroutine("LoadTargetScene", "TitleScreen");
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
}
