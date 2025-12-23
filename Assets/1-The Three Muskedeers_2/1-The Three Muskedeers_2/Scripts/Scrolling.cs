using UnityEngine;
using UnityEngine.UI;
namespace The_Three_Muskedeers
{
    public class Scrolling : MonoBehaviour
    {
        [SerializeField] private float scrollSpeed;
        //Vector2 currentScroll;
        //Image image;
        //SpriteRenderer spriteRenderer;
        //private Material material;
        [SerializeField] private RawImage image;


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            //image = GetComponent<Image>();
            //material = image.material;
        }

        // Update is called once per frame
        void Update()
        {
            image.uvRect = new Rect(image.uvRect.position - new Vector2(scrollSpeed, 0) * Time.deltaTime, image.uvRect.size);
            //float currentScrollSpeed = scrollSpeed * Time.deltaTime;
            //float scrollSpeedNormalized = currentScrollSpeed / image.bounds.size.x;
            //currentScroll.x -= scrollSpeedNormalized;
            //material.mainTextureOffset = currentScroll;
        }
    }
}
