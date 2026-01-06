using UnityEngine;

namespace yourtaxes
{
    public class SoldierSpawner : MonoBehaviour
    {
        [SerializeField]
        private int amntBoxes;
        [SerializeField]
        private float[] StandardWeights;
        [SerializeField]
        private int regularChance;
        [SerializeField]
        private int largeChance;
        [SerializeField]
        private int smallChance;
        [SerializeField]
        private GameObject[] soldiers;
        private float dificulty;
        private int subdivisions;


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            dificulty = Managers.MinigamesManager.GetCurrentMinigameDifficulty();
            int[] sides = new int[] { -1, 1 };
            int chosen = Random.Range(0, 2);
            bool hasSpawnedLarge = false;
            for (int i = 0; i < amntBoxes; i++)
            {
                int soldierIndex = 0;
                int soldierChance = Random.Range(0, 100) + 1;
                if (((soldierChance <= largeChance) && !hasSpawnedLarge) || (subdivisions == 1))
                {
                    soldierIndex = 1;
                    hasSpawnedLarge = true;
                }
                else if (soldierChance <= (regularChance + largeChance))
                {
                    soldierIndex = 0;
                }
                else if (soldierChance <= (regularChance + largeChance + smallChance))
                {
                    soldierIndex = 2;
                }


                GameObject currentSoldier = soldiers[soldierIndex];
                Vector3 soldierPos = new Vector3(0, 0);
                if (soldierIndex == 0)
                {
                    soldierPos.x = Random.Range(3f, 4.5f) * sides[chosen];
                    soldierPos.y = 2.5f;

                }
                else if (soldierIndex == 1)
                {
                    soldierPos.x = Random.Range(3.0f, 3.5f) * sides[chosen];
                    soldierPos.y = 2.5f;
                }
                if (soldierIndex == 2)
                {
                    soldierPos.x = Random.Range(4.5f, 4.75f) * sides[chosen];
                    soldierPos.y = 2.5f;
                }
                Rigidbody2D currentRigidbody = currentSoldier.GetComponent<Rigidbody2D>();
                currentRigidbody.mass = StandardWeights[soldierIndex] * (0.5f + (0.5f * dificulty));

                
                
                Instantiate(currentSoldier, soldierPos, Quaternion.identity);
            }

        }
    }
}

