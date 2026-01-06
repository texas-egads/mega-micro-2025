using UnityEngine;

namespace boinkthatboy
{
    public class FishingHookTimingUI : MonoBehaviour
    {
        [Header("Needle")]
        public RectTransform needle;

        [Tooltip("Swing angle")]
        public float maxAngle = 75f;

        [Header("Speed")]
        public float cyclesPerSecond = 1.2f;

        [Header("Zones")]
        public float greenZone = 0.1f;
        public float yellowZone = 0.3f;

        float time;
        bool paused;

        public void Show(bool show)
        {
            gameObject.SetActive( show );
            if ( show ) { ResetCycleRandom(); }
        }

        public void SetPaused(bool p)
        {
            paused = p;
        }

        public void ResetCycleRandom()
        {
            time = Random.Range(0f, 10f);
            paused = false;
        }

        public void ResetCycleToLeft()
        {
            if (cyclesPerSecond > 0.001f)
            {
                time = -1f / (4f * cyclesPerSecond);
            }
            else
            {
                time = 0f;
            }

            paused = false;
        }

        public float getNeedle()
        {
            if (needle == null) return 0.5f;

            float angle = needle.localEulerAngles.z;
            if (angle > 180f) angle -= 360f;

            float t = Mathf.InverseLerp(-maxAngle, maxAngle, angle);
            return Mathf.Clamp01(t);
        }

        public float getScore(float needle01)
        {
            float dist = Mathf.Abs(needle01 - 0.5f);

            if (dist <= greenZone) return 1f;
            if (dist <= yellowZone) return 0.5f;
            return 0f;
        }

        public void SetZones(float green, float yellow)
        {
            greenZone = green;
            yellowZone = yellow;
        }

        void OnEnable()
        {
            ResetCycleRandom();
        }

        void Update()
        {
            if (paused) { return; }
            if (needle == null) { return; }

            time += Time.deltaTime;

            float sine = Mathf.Sin( time * 2f * Mathf.PI * cyclesPerSecond );
            float z = sine * maxAngle;

            needle.localEulerAngles = new Vector3(0f, 0f, z);
        }
    }
}