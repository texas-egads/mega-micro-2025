using UnityEngine;
using System.Collections;

namespace WyvernOfWhimsy
{
    public class hitterMover : MonoBehaviour
    {
        private float startY;
        private Coroutine currentAnim;
        [SerializeField] buttonController ButtonController;
        [SerializeField] float endY;
        public float slideSpeed;

        private void Start()
        {
            startY = transform.position.y;
        }


        public void AnimateHitter()
        {
            if (currentAnim != null)
            {
                StopCoroutine(currentAnim);
            }
            currentAnim = StartCoroutine(AnimateCycle());
        }

        IEnumerator AnimateCycle()
        {
            while (transform.position.y < endY)
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, endY, transform.position.z), slideSpeed * Time.deltaTime); 
                yield return null;
            }
            yield return new WaitForSeconds(0.5f);
            while (transform.position.y > startY)
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, startY, transform.position.z), slideSpeed * Time.deltaTime);
                yield return null;
            }

        }
    }
}
