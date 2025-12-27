using System.Collections;
using UnityEngine;

namespace WyvernOfWhimsy
{
    public class buttonController : MonoBehaviour
    {
        [System.Serializable]
        public struct KeyColorPair
        {
            public KeyCode key;
            public Color color;
        }

        public Color defaultColor;
        public bool isPressed;
        [SerializeField] public KeyColorPair[] keyColors;
        private SpriteRenderer spriteRenderer;
        private KeyCode? activeKey;
        private Coroutine pressing;


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // Update is called once per frame
        void Update()
        {
            foreach (var pair in keyColors)
            {
                if (Input.GetKeyDown(pair.key))
                {
                    Press(pair);
                    break;
                }
            }

            if (activeKey.HasValue && Input.GetKeyUp(activeKey.Value))
            {
                Release();
            }
        }

        private void Press(KeyColorPair pair)
        {
            spriteRenderer.color = pair.color;
            activeKey = pair.key;
            if (pressing == null)
            {
                StartCoroutine(PressedDown());
            }
        }

        private void Release()
        {
            spriteRenderer.color = defaultColor;
            activeKey = null;
            if (pressing != null)
            {
                StopCoroutine(pressing);
            }
        }

        IEnumerator PressedDown()
        {
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = defaultColor;
        }
    }
}