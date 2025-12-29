using UnityEngine;
using UnityEngine.UI;
namespace The_Three_Muskedeers
{
    public class Move : MonoBehaviour
    {
        [SerializeField] private float speed;
        public bool canWin;
        public bool canBePressed;

        [SerializeField] private Canvas canvas;
        [SerializeField] private Camera cam;
        Vector2 WorldObject_ScreenPosition;

        [SerializeField] private float xOffset = 410;
        [SerializeField] private float yOffset = 130;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            canWin = true;
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(WorldObject_ScreenPosition.x - xOffset, WorldObject_ScreenPosition.y - yOffset);
        }

        // Update is called once per frame
        void Update()
        {
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            Vector2 ViewportPosition = cam.WorldToViewportPoint(GameObject.FindGameObjectWithTag("Object 2").GetComponent<RectTransform>().anchoredPosition);
            WorldObject_ScreenPosition = new Vector2(
            ViewportPosition.x * canvasRect.sizeDelta.x,
            ViewportPosition.y * canvasRect.sizeDelta.y);

            if (canWin)
            {
                Managers.MinigamesManager.DeclareCurrentMinigameWon();
            }
            // move present
            gameObject.GetComponent<RectTransform>().anchoredPosition += new Vector2(speed, 0) * Time.deltaTime;
            if (Input.GetKeyDown(KeyCode.Space) && canBePressed)
            {
                Destroy(gameObject);
            }
            if (gameObject.GetComponent<RectTransform>().anchoredPosition.x > WorldObject_ScreenPosition.x - 7500)
            {
                canWin = false;
                Managers.MinigamesManager.DeclareCurrentMinigameLost();
                Managers.MinigamesManager.EndCurrentMinigame(0);
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            canBePressed = true;
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            canBePressed = false;
        }

        public void Spawn()
        {
            Instantiate(gameObject);
        }
    }
}
