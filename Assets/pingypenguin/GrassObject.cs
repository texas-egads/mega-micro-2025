using System;
using UnityEngine;

namespace pingypenguin {
    public class GrassObject : MonoBehaviour
    {
        private bool cut;

        public GameManager gm;
        [SerializeField] private Sprite cutImg;
        [SerializeField] private AutoShadow shadow;
        private SpriteRenderer sr;
        private ParticleSystem ps;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            sr = GetComponent<SpriteRenderer>();
            ps = GetComponent<ParticleSystem>();
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        private void OnTriggerEnter2D(Collider2D other) {
            // The grass becomes cut if the player's scissors
            // touches the grass
            if (!cut && other.tag == "Player") {
                cut = true;
                sr.sprite = cutImg;
                ps.Emit(5);
                shadow.ReloadShadow(false);
                gm.GrassCut();
            }
        }
    }
}
