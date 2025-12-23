using UnityEngine;

namespace Graupel 
{
    public class GraupelFall : MonoBehaviour
    {
        public float speed = 300f;
        public float areaWidth = 800f;
        public float areaHeight = 450f;

        private RectTransform rectTransform;
        private Vector2 direction;

        public float difficulty;

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            direction = new Vector2(Random.value > 0.5f ? 1 : -1 , Random.value > 0.5f ? 1 : -1).normalized;
            difficulty = Managers.MinigamesManager.GetCurrentMinigameDifficulty() * 100f;
            speed += (difficulty * 2f);
        }

        void Update()
        {
            rectTransform.anchoredPosition += direction * speed * Time.deltaTime;
            CheckBounds();
        }

        void CheckBounds()
        {
            Vector2 pos = rectTransform.anchoredPosition;
            Vector2 size = rectTransform.sizeDelta;

            float halfWidth = size.x / 2f;
            float halfHeight = size.y / 2f;

            float minX = -100f;
            float maxX = 300f;
            float minY = -areaHeight / 2f + halfHeight;
            float maxY = areaHeight / 2f - halfHeight;

            // Horizontal Bounce
            if (pos.x <= minX || pos.x >= maxX)
            {
                direction.x *= -1;
                pos.x = Mathf.Clamp(pos.x, minX, maxX);
            }

            // Vertical Bounce
            if (pos.y <= minY || pos.y >= maxY)
            {
                direction.y *= -1;
                pos.y = Mathf.Clamp(pos.y, minY, maxY);
            }
            rectTransform.anchoredPosition = pos;
        }
    }
}
