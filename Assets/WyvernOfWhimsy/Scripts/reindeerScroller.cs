using UnityEngine;

namespace WyvernOfWhimsy
{
    public class reindeerScroller : MonoBehaviour
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
            transform.position -= new Vector3(beatMultiplier * beatTempo * Time.deltaTime, 0f, 0f);
        }
    }
}
