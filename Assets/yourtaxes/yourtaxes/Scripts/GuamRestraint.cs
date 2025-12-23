using UnityEngine;

namespace yourtaxes {
    public class GuamRestraint : MonoBehaviour
    {
        public bool guamTipped;
        [SerializeField]
        private Color regularColor;
        [SerializeField]
        public Color warningColor;
        [SerializeField]
        private float guamMaxAngle;
        [SerializeField]
        private float guamWarningAngle;
        [SerializeField]
        private float currentRotation;
        [SerializeField]
        private GameObject island;
        [SerializeField]
        private AudioClip splashClip;
        private Rigidbody2D rb;
        private Transform tf;
        private SpriteRenderer sr;
        private AudioPlayer ap;
        
        private WinLoseConditions wlc;
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            tf = GetComponent<Transform>();
            sr = island.GetComponent<SpriteRenderer>();
            wlc = GetComponent<WinLoseConditions>();
            ap = GetComponent<AudioPlayer>();
        }

        void Update()
        {
            currentRotation = tf.eulerAngles.z;
            //Debug.Log(currentRotation);
            if (tipped(guamMaxAngle) && !wlc.hasWon && !guamTipped)
            {
                guamTipped = true;
                ap.playAudio(splashClip);
                //Debug.Log("Guam Tipped");
            }
            if (tipped(guamWarningAngle))
            {
                //sr.color = warningColor;
                sr.color = Color.red;

            }
            else
            {
                sr.color = Color.white;
            }
            //Debug.Log(sr.color);
        }

        //freeses guam in place
        public void lockGuam()
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        //determines if guam is more than threshold degrees tilted away from being flat
        private bool tipped(float threshold)
        {
            if (currentRotation < threshold)
            {
                return false;
            }
            if (currentRotation > (360 - threshold))
            {
                return false;
            }
            return true;
        }
    }
}
