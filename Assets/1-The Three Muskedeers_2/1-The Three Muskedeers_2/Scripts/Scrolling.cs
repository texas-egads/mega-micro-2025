using UnityEngine;
namespace The_Three_Muskedeers
{
    public class Scrolling : MonoBehaviour
    {
        [SerializeField] private float scrollSpeed;
        Vector2 currentScroll;
        SpriteRenderer spriteRenderer;
        private Material material;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            material = spriteRenderer.material;
        }

        // Update is called once per frame
        void Update()
        {
            float currentScrollSpeed = scrollSpeed * Time.deltaTime;
            float scrollSpeedNormalized = currentScrollSpeed / spriteRenderer.bounds.size.x;
            currentScroll.x -= scrollSpeedNormalized;
            material.mainTextureOffset = currentScroll;
        }
    }
}
