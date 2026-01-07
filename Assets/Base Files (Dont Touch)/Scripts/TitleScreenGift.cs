using UnityEngine;
using UnityEngine.UI;

public class TitleScreenGift : MonoBehaviour
{
    private RectTransform t;
    private Image image;
    private float timer;
    public float range;
    public float loopTime;
    public AnimationCurve pos;
    public Sprite[] images;
    private int giftIndex;

    private void Start()
    {
        t = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        giftIndex = Random.Range(0, 5);
        image.sprite = images[giftIndex];
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= loopTime)
        {
            giftIndex = (giftIndex + Random.Range(1,5)) % 5;
            image.sprite = images[giftIndex];
            timer -= loopTime;
        }
        t.anchoredPosition = new Vector2(range * pos.Evaluate(timer/loopTime), t.anchoredPosition.y);
    }
}
