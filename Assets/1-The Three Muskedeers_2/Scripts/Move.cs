using UnityEngine;
namespace The_Three_Muskedeers
{
    public class Move : MonoBehaviour
    {
        [SerializeField] private float speed;
        public bool canWin;
        public bool canBePressed;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            canWin = true;
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(GameObject.FindGameObjectWithTag("Object 2").GetComponent<RectTransform>().anchoredPosition.x - 910, GameObject.FindGameObjectWithTag("Object 2").GetComponent<RectTransform>().anchoredPosition.y, 0);
        }

        // Update is called once per frame
        void Update()
        {
            if (canWin)
            {
                Managers.MinigamesManager.DeclareCurrentMinigameWon();
            }
            transform.position += new Vector3(speed, 0, 0) * Time.deltaTime;
            if (Input.GetKeyDown(KeyCode.Space) && canBePressed)
            {
                Destroy(gameObject);
            }
            if (gameObject.GetComponent<RectTransform>().anchoredPosition.x > GameObject.FindGameObjectWithTag("Object 2").GetComponent<RectTransform>().anchoredPosition.x + 400)
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
