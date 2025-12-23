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
        [SerializeField]
        private GameObject guamRestraint;
        [SerializeField]
        private Animator senator;
        private WinLoseConditions wlc;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            wlc = guamRestraint.GetComponent<WinLoseConditions>();
        }

        // Update is called once per frame
        void Update()
        {
            if ((Input.GetButtonDown("Space") || Input.GetButton("Enable Debug Button 1")) && !wlc.hasEnded)
            {
                senator.Play("senatorPullLever");
                Vector3 soldierPos = transform.position;
                soldierPos.y += soldierOffset;
                int soldierIndex = 0;
                int soldierChance = Random.Range(0, 100) + 1;
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
                Rigidbody2D rb = Instantiate(soldiers[soldierIndex], soldierPos, Quaternion.identity).GetComponent<Rigidbody2D>();
                rb.linearVelocityY = -soldierSpeed;
            }
        }

        void FixedUpdate()
        {
            Vector3 scale = transform.localScale;
            if (Input.GetAxis("Horizontal") > 0)
            {
                scale.x = 1;
                if (transform.position.x < playerBounds)
                {
                    transform.Translate(Vector2.right * speed * Time.deltaTime);
                }
            }
            if (Input.GetAxis("Horizontal") < 0)
            {
                scale.x = -1;
                if (transform.position.x > -playerBounds)
                {
                    transform.Translate(Vector2.left * speed * Time.deltaTime);
                }
            }
            transform.localScale = scale;
        }
    }
}

