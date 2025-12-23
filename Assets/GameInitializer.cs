using UnityEngine;
using UnityEngine.UI;

namespace ZABsters {
    public class GameInitializer : MonoBehaviour
    {
        [System.NonSerialized]
        public bool isSantaWorkshop = false;

        public Sprite[] sprites;

        public SpriteRenderer bgImage;

        void Start()
        {
            //ranodmly assign isSantaWorkshop value
            Debug.Log("WSG BRODIE");
            isSantaWorkshop = Random.value > 0.5f;

            if(isSantaWorkshop)
            {
                bgImage.sprite = sprites[0];
            }
            else
            {
                bgImage.sprite = sprites[1];
            }
        }


    }
}