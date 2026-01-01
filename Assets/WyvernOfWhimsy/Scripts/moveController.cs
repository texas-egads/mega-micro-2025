using System.Drawing;
using System.Collections;
using UnityEngine;

namespace WyvernOfWhimsy
{

    public class moveController : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
        public Sprite defaultHands;
        public Sprite coveredHands;
        public Sprite missedHands;

        [SerializeField] float handNum;
        [SerializeField] deerSprites DeerSprites;
        [SerializeField] normalKey NormalKey;

        private bool isInsideHitChecker;
        private bool isHit;
        private KeyCode keyToPress;
        private GameObject hitChecker;
        private buttonController ButtonController;

        void Start()
        {
            keyToPress = NormalKey.keyToPress;
            spriteRenderer = GetComponent<SpriteRenderer>();
        }


        private void Update()
        {
            if (isInsideHitChecker)
            {
                 if (Input.GetKeyDown(keyToPress) && !isHit)
                {
                    gameManager.instance.handHit(handNum);
                    isHit = true;
                    StartCoroutine(ChangeSprite(coveredHands));
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "HitChecker")
            {
                if (hitChecker == null)
                {
                    hitChecker = other.gameObject;
                    ButtonController = other.GetComponent<buttonController>();
                }
                isInsideHitChecker = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.tag == "HitChecker" && !isHit)
            {
                StartCoroutine(ChangeSprite(missedHands));
                isInsideHitChecker = false;
                gameManager.instance.handMissed(handNum, DeerSprites);
            }
        }
        IEnumerator ChangeSprite(Sprite changeSpriteTo)
        {
            yield return new WaitForSeconds(0.2f);
            spriteRenderer.sprite = changeSpriteTo;
        }
    }

}