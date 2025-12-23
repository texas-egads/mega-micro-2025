using UnityEngine;

namespace TeamShakra
{
  public class TimerController : MonoBehaviour
  {
    private RectTransform timerBarFill;
    private GameController gameController;
    [SerializeField] private float gameTime = 10f;
    private float currentTime;
    private float totalWidth;

    void Start()
    {
      gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
      if (gameController == null)
      {
        Debug.LogError("Game Controller not found");
      }
      timerBarFill = gameObject.GetComponent<RectTransform>();
      currentTime = gameTime;
      totalWidth = timerBarFill.rect.width;
    }

    void Update()
    {
      currentTime -= Time.deltaTime;
      if (currentTime < 0) currentTime = 0;
      
      float timePercent = currentTime / gameTime;
      Debug.Log($"Current Time: {currentTime}");
      if (timePercent <= 0.0f)
      {
          gameController.timeUp();
      }
      float newWidth = totalWidth * timePercent;
      
      timerBarFill.sizeDelta = new Vector2(newWidth, timerBarFill.sizeDelta.y);
    }
  }
}