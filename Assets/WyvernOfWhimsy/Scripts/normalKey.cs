using UnityEngine;

namespace WyvernOfWhimsy
{

    public class normalKey : MonoBehaviour
    {
        public KeyCode[] keys = new KeyCode[]
        {
        KeyCode.W,
        KeyCode.A,
        KeyCode.S,
        KeyCode.D
        };
        public KeyCode keyToPress;
        [SerializeField] Sprite wSprite;
        [SerializeField] Sprite aSprite;
        [SerializeField] Sprite sSprite;
        [SerializeField] Sprite dSprite;
        private float randomIndex;
        private SpriteRenderer spriteRenderer;
        private void Awake()
        {
            int randomIndex = Random.Range(0, 4);
            keyToPress = keys[randomIndex];
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (keyToPress == KeyCode.W)
            {
                spriteRenderer.sprite = wSprite;
            }
            else if (keyToPress == KeyCode.A)
            {
                spriteRenderer.sprite = aSprite;
            }
            else if (keyToPress == KeyCode.S)
            {
                spriteRenderer.sprite = sSprite;
            }
            else if (keyToPress == KeyCode.D)
            {
                spriteRenderer.sprite = dSprite;
            }
        }
    }
}
