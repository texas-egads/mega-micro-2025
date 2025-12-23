using UnityEngine;
using UnityEngine.UI;

namespace ZABsters {
    public class GameInitializer : MonoBehaviour
    {
        [System.NonSerialized]
        public bool isSantaWorkshop = false;

        [System.NonSerialized]
        public int taskNumber = 0;

        public Sprite[] sprites;

        public Image bgImage;

        void Start()
        {
            //ranodmly assign isSantaWorkshop value
            // Debug.Log("WSG BRODIE");
            // isSantaWorkshop = Random.value > 0.5f;

            //task number is randomly either 0, 1, 2:
            taskNumber = Random.Range(0, 3);

            //if 0, set bgimage to sprites[0], otherwise set to sprites[1]
            bgImage.sprite = (taskNumber == 0) ? sprites[0] : sprites[1];

            //0 is gift task
            //1 is sleigh repair
            //2 is snow shoveling task
        }


    }
}