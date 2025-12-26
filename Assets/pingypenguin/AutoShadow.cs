using UnityEngine;

namespace pingypenguin {
    public class AutoShadow : MonoBehaviour
    {
        private SpriteRenderer sr;
        private SpriteRenderer parentSr;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            sr = GetComponent<SpriteRenderer>();
            parentSr = transform.parent.GetComponent<SpriteRenderer>();
            ReloadShadow();
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void ReloadShadow(bool visible = true) {
            sr.sprite = visible ? parentSr.sprite : null;
        }
    }
}
