using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace WyvernOfWhimsy
{
    public class deerSpawner : MonoBehaviour
    {
        public List<GameObject> deerList = new List<GameObject>(); 
        public GameObject normalDeer; 
        public GameObject redDeer; 
        
        [SerializeField] private float spawnTime; 
        private Coroutine spawnDeer; 
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start() 
        {
            deerList.Add(normalDeer); 
            deerList.Add(redDeer); 
            int listIndex = UnityEngine.Random.Range(0, 2); 
            Instantiate(deerList[listIndex], transform.position, Quaternion.identity);
            spawnDeer = StartCoroutine(SpawnDeer());
        }
        IEnumerator SpawnDeer()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(spawnTime);
                int listIndex = Random.Range(0, deerList.Count);
                Instantiate(deerList[listIndex], transform.position, Quaternion.identity);
            }
        }
    }
}
/*using System.Collections;
using UnityEngine;

namespace WyvernOfWhimsy
{
    public class deerSpawner : MonoBehaviour
    {
        [Header("Prefabs")]
        public GameObject normalDeer;
        public GameObject redDeer;

        [Header("Rhythm Settings")]
        [SerializeField] private float bpm = 144f;       // Beats per minute
        [SerializeField] private int beatsToSpawn = 1;   // Spawn on every Nth beat

        [Header("Audio")]
        [SerializeField] private AudioSource songSource; // Assign the AudioSource from their AudioManager

        [Header("Scripts")]
        [SerializeField] gameManager GameManager;

        private float beatInterval;  // seconds per beat
        private int beatCount = 0;

        private void Start()
        {
            // Validate input
            if (bpm <= 0f || beatsToSpawn <= 0)
            {
                Debug.LogError("BPM and beatsToSpawn must be greater than 0.");
                enabled = false;
                return;
            }

            songSource = GameManager.musicSource;

            if (songSource == null)
            {
                Debug.LogError("AudioSource not assigned to deerSpawner.");
                enabled = false;
                return;
            }

            beatInterval = 60f / bpm;
            StartCoroutine(SpawnBeats());
        }

        private IEnumerator SpawnBeats()
        {
            // Wait until the AudioSource starts playing
            while (!songSource.isPlaying)
                yield return null;

            // Initialize the next beat based on the AudioSource's current playback time
            double nextBeatTime = songSource.time + beatInterval;

            while (songSource.isPlaying)
            {
                double waitTime = nextBeatTime - songSource.time;
                if (waitTime > 0)
                    yield return new WaitForSecondsRealtime((float)waitTime);

                // Increment beat counter
                beatCount++;

                // Spawn deer if it's the correct beat
                if (beatCount % beatsToSpawn == 0)
                    SpawnDeer();

                nextBeatTime += beatInterval;
            }
        }

        private void SpawnDeer()
        {
            GameObject prefab = Random.value < 0.8f ? normalDeer : redDeer;
            Instantiate(prefab);
            Debug.Log($"Spawned {prefab.name} at song time {songSource.time:F4}");
        }
    }
}*/







