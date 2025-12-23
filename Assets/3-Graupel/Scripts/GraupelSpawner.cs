using UnityEngine;
using UnityEngine.UI;

namespace Graupel
{

    public class GraupelSpawner : MonoBehaviour
    {
        public Canvas canvas;
        public GameObject graupelPrefab;
        public Sprite[] graupelSprites;

        public Color highlightColor = Color.yellow;

        public float difficulty = 0;
        public int spawnCount = 5;

        private bool object1Spawned = false;

        void Start()
        {
            difficulty = Managers.MinigamesManager.GetCurrentMinigameDifficulty() * 100f;
            spawnCount += (int)((difficulty/10)/2);
            
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

            // Movement bounds
            fall.areaWidth = 400f;  
            fall.areaHeight = 450f;

            // Random starting position 
            float halfWidth = fall.areaWidth / 2f;
            float halfHeight = fall.areaHeight / 2f;

            rect.anchoredPosition = new Vector2( Random.Range(-halfWidth / 2f, halfWidth / 2f), Random.Range(-halfHeight / 2f, halfHeight / 2f));

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
                HighlightObject(obj);
                object1Spawned = true;
            }
            else
            {
                obj.tag = "Object 2";
            }
        }

        void HighlightObject(GameObject obj1)
        {
            Image img = obj1.GetComponent<Image>();
            img.color = highlightColor;
        }
    }
}
