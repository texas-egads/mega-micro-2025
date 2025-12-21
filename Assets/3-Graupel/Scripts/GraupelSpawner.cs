using UnityEngine;
using UnityEngine.UI;

namespace Graupel
{

    public class GraupelSpawner : MonoBehaviour
    {
        public Canvas canvas;
        public GameObject graupelPrefab;
        public Sprite[] graupelSprites;

        public int spawnCount = 1;

        private bool object1Spawned = false;

        void Start()
        {
            for (int i = 0; i < spawnCount; i++)
            {
                SpawnGraupel();
            }
        }

        void SpawnGraupel()
        {
            GameObject obj = Instantiate(graupelPrefab, canvas.transform);

            RectTransform rect = obj.GetComponent<RectTransform>();
            GraupelFall fall = obj.GetComponent<GraupelFall>();

            // Assign canvas to movement script
            fall.canvas = canvas;

            // Random starting position inside canvas
            float canvasWidth = canvas.GetComponent<RectTransform>().rect.width;
            float canvasHeight = canvas.GetComponent<RectTransform>().rect.height;

            rect.anchoredPosition = new Vector2( Random.Range(-canvasWidth / 2f, canvasWidth / 2f), Random.Range(-canvasHeight / 2f, canvasHeight / 2f));

            // Assign a random sprite from the list
            Image img = obj.GetComponent<Image>();
            if (graupelSprites.Length > 0)
            {
                img.sprite = graupelSprites[Random.Range(0, graupelSprites.Length)];
            }

            // Assign tag
            if (!object1Spawned)
            {
                obj.tag = "Object 1";
                object1Spawned = true;
            }
            else
            {
                obj.tag = "Object 2";
            }
        }
    }

}
