using UnityEngine;

namespace yourtaxes
{
    public class NewMonoBehaviourScript : MonoBehaviour
    {
        [SerializeField]
        private float playerBounds;
        [SerializeField]
        private float soldierOffset;
        [SerializeField]
        private float soldierSpeed;
        [SerializeField]
        private int speed;
        [SerializeField]
        private int regularChance;
        [SerializeField]
        private int largeChance;
        [SerializeField]
        private int smallChance;
        [SerializeField]
        private GameObject[] soldiers;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            //Debug.Log(transform.position.x);

            if (Input.GetButtonDown("Space") || Input.GetButton("Enable Debug Button 1"))
            {
                Vector3 soldierPos = transform.position;
                soldierPos.y += soldierOffset;
                int soldierIndex = 0;
                int soldierChance = Random.Range(0, 100) + 1;
                //Debug.Log(soldierChance);
                if (soldierChance <= regularChance)
                {
                    soldierIndex = 0;
                }
                else if (soldierChance <= (regularChance + smallChance))
                {
                    soldierIndex = 1;
                }
                else if (soldierChance <= (regularChance + smallChance + largeChance))
                {
                    soldierIndex = 2;
                }


                GameObject currentSoldier = soldiers[soldierIndex];

                Rigidbody2D rb = Instantiate(soldiers[soldierIndex], soldierPos, Quaternion.identity).GetComponent<Rigidbody2D>();
                rb.linearVelocityY = -soldierSpeed;
                Debug.Log("boxSpeed " + rb.linearVelocityY);

            }
            if (Input.GetButtonDown("Enable Debug Button 2"))
            {
                Managers.MinigamesManager.DeclareCurrentMinigameWon();
                Managers.MinigamesManager.EndCurrentMinigame();
            }
        }

        void FixedUpdate()
        {
            if (Input.GetAxis("Horizontal") > 0)
            {
                if (transform.position.x < playerBounds)
                {
                    transform.Translate(Vector2.right * speed * Time.deltaTime);
                }
            }
            if (Input.GetAxis("Horizontal") < 0)
            {
                if (transform.position.x > -playerBounds)
                {
                    transform.Translate(Vector2.left * speed * Time.deltaTime);
                }
            }
        }
    }
}

