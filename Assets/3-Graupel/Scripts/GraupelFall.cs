using UnityEngine;

namespace Graupel 
{
    public class GraupelFall : MonoBehaviour
    {
        public float speed = 300f;
        public Canvas canvas;

        private RectTransform rectTransform;
        private RectTransform canvasRect;
        private Vector2 direction;

        void Awake()
        {
            canvas = GetComponentInParent<Canvas>();
            rectTransform = GetComponent<RectTransform>();
            canvasRect = canvas.GetComponent<RectTransform>();

            direction = new Vector2(Random.value > 0.5f ? 1 : -1 , Random.value > 0.5f ? 1 : -1).normalized;
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

            float canvasWidth = canvasRect.rect.width;
            float canvasHeight = canvasRect.rect.height;

            float minX = -canvasWidth / 2f + halfWidth;
            float maxX = canvasWidth / 2f - halfWidth;
            float minY = -canvasHeight / 2f + halfHeight;
            float maxY = canvasHeight / 2f - halfHeight;

            if (pos.x <= minX || pos.x >= maxX)
            {
                direction.x *= -1;
                pos.x = Mathf.Clamp(pos.x, minX, maxX);
            }

            if (pos.y <= minY || pos.y >= maxY)
            {
                direction.y *= -1;
                pos.y = Mathf.Clamp(pos.y, minY, maxY);
            }
            rectTransform.anchoredPosition = pos;
        }
    }
}
