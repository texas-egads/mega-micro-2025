using System.Collections;
using UnityEngine;

namespace WyvernOfWhimsy
{
    public class deerSprites : MonoBehaviour
    {
        public float angerLevel = 0;
        [SerializeField] private Sprite happySprite;
        [SerializeField] private Sprite midSprite;
        [SerializeField] private Sprite angrySprite;
        private SpriteRenderer spriteRenderer;

        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void AngerUp()
        {
            angerLevel++;
            if (angerLevel == 1)
            {
                StartCoroutine(turnRed());
                spriteRenderer.sprite = midSprite;
            } else if (angerLevel == 2)
            {
                StartCoroutine(turnRed());
                spriteRenderer.sprite = angrySprite;
            } else
            {
                StartCoroutine(turnRed());
            }
        }

        IEnumerator turnRed()
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.5f);
            spriteRenderer.color = Color.white;
        }
    }
}
