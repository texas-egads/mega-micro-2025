using UnityEngine;

namespace WyvernOfWhimsy
{
    public class conveyorScroller : MonoBehaviour
    {
        public float beatTempo;
        public float beatMultiplier;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            beatTempo = beatTempo / 60f;
        }

        // Update is called once per frame
        void Update()
        {
            if (transform.position.x > -10)
            {
                transform.position -= new Vector3(beatMultiplier * beatTempo * Time.deltaTime, 0f, 0f);
            } else
            {
                transform.position = new Vector3(10, transform.position.y, transform.position.z);
            }
            
        }
    }
}
