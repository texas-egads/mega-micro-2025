using System.Collections.Generic;
using UnityEngine;

// !!
// parts of this code was created with ChatGPT or Gemini
// !!

namespace boinkthatboy
{
    public class WaveSliceAngledSpawner : MonoBehaviour
    {
        public FluidTopology sim;
        public WaveSliceAngled slicePrefab;

        [Header("Pool")]
        public int poolSize = 18;
        [Range(1, 3)] 
        public int maxSimultaneous = 3;

        [Header("Spawn Timing")]
        public Vector2 spawnIntervalRange = new Vector2(0.25f, 0.85f);
        public Vector2Int burstCountRange = new Vector2Int(1, 3);

        [Header("Delay Dial")]
        [Range(0f, 1.0f)] 
        public float delayDial = 0.25f;

        [Header("Angle Bias")]
        [Range(0f, 60f)] 
        public float angleSpreadDeg = 20f;
        [Range(0f, 1f)] 
        public float wildAngleChance = 0.08f;

        [Header("Offset")]
        public Vector2 offsetRange = new Vector2(-0.75f, 0.75f);

        [Header("Slice Shape")]
        public int points = 256;

        [Header("Depth")]
        public float heightScale = 0.35f;  
        public float zOffset = 0.0f;       
        public bool allowNegativeHeights = true;

        [Header("Motion")]
        public float travelTime = 0.45f;
        public float fadeOutTime = 0.18f;

        [Header("Trail")]
        [Range(0.01f, 1f)] 
        public float trailLength01 = 0.18f;
        [Range(0.0f, 1f)] 
        public float trailSoftness01 = 0.12f;

        [Header("Color")]
        public Color lineColor = Color.black;
        [Range(0f, 1f)] 
        public float opacity = 0.85f;

        readonly List<WaveSliceAngled> pool = new();
        float timer;

        public bool paused;

        void Start()
        {
            for ( int i = 0; i < poolSize; i++ )
            {
                var s = Instantiate( slicePrefab, transform );
                s.gameObject.SetActive( false );
                pool.Add( s );
            }

            timer = Random.Range( spawnIntervalRange.x, spawnIntervalRange.y );
        }

        void Update()
        {
            if (paused) { return; }
            if (sim == null || slicePrefab == null) { return; }

            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                timer = Random.Range(spawnIntervalRange.x, spawnIntervalRange.y);
                int want = Random.Range(burstCountRange.x, burstCountRange.y + 1);
                SpawnBurst(want);
            }
        }

        void SpawnBurst(int count)
        {
            int alive = CountAlive();
            int canSpawn = Mathf.Clamp(maxSimultaneous - alive, 0, count);

            for (int i = 0; i < canSpawn; i++)
            {
                var s = GetFreeSlice();
                if (s == null) { return; }

                s.points = points;
                s.heightScale = heightScale;
                s.zOffset = zOffset;
                s.allowNegativeHeights = allowNegativeHeights;
                s.travelTime = travelTime;
                s.fadeOutTime = fadeOutTime;
                s.trailLength01 = trailLength01;
                s.trailSoftness01 = trailSoftness01;

                bool goRight = Random.value < 0.5f;
                float baseAngleDeg = goRight ? 0f : 180f;

                float tilt = Random.Range(-angleSpreadDeg, angleSpreadDeg);
                if (Random.value < wildAngleChance) { tilt = Random.Range(-60f, 60f); }

                float angleRad = (baseAngleDeg + tilt) * Mathf.Deg2Rad;

                float offset01 = Random.Range(offsetRange.x, offsetRange.y);
                float delay = Random.Range(0f, delayDial);

                s.Begin(sim, angleRad, offset01, heightScale, zOffset, lineColor, opacity, delay);
            }
        }

        int CountAlive()
        {
            int c = 0;
            for (int i = 0; i < pool.Count; i++)
                if (pool[i].gameObject.activeSelf) c++;
            return c;
        }

        WaveSliceAngled GetFreeSlice()
        {
            for (int i = 0; i < pool.Count; i++)
                if (!pool[i].gameObject.activeSelf) return pool[i];
            return null;
        }
    }
}
