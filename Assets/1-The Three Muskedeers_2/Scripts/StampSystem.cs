using UnityEngine;
using UnityEngine.Events;
namespace The_Three_Muskedeers
{
    public class StampSystem : MonoBehaviour
    {
        [SerializeField] private float _bpm;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private Intervals[] _intervals;
        public float sampledTime;
        [SerializeField] private AudioClip easySound;
        [SerializeField] private AudioClip medSound;
        [SerializeField] private AudioClip hardSound;

        float difficulty;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _audioSource = Managers.AudioManager.CreateAudioSource();
            _audioSource.loop = true;

            difficulty = Managers.MinigamesManager.GetCurrentMinigameDifficulty();
            foreach (Intervals interval in _intervals)
            {
                interval.SetDifficulty(difficulty);
            }

            if (difficulty < 0.33)
                _audioSource.clip = easySound;   // Easy
            else if (difficulty < 0.66)
                _audioSource.clip = medSound;  // Medium
            else
                _audioSource.clip = hardSound;  // Hard

            _audioSource.Play();
        }

        // Update is called once per frame
        void Update()
        {
            foreach (Intervals interval in _intervals)
            {
                sampledTime = _audioSource.timeSamples / (_audioSource.clip.frequency * interval.GetIntervalLength(_bpm));
                interval.CheckForNewInteval(sampledTime);
            }
        }
    }

    [System.Serializable]
    public class Intervals
    {
        [SerializeField] public float _steps;
        [SerializeField] private UnityEvent _trigger;

        [Header("Patterns by Difficulty")]
        [SerializeField] private bool[] easyPattern;
        [SerializeField] private bool[] medPattern;
        [SerializeField] private bool[] hardPattern;
        private bool[] activePattern;
        public int _lastInterval;

        public void SetDifficulty(float difficulty)
        {
            if (difficulty < 0.33)
                activePattern = easyPattern;   // Easy
            else if (difficulty < 0.66)
                activePattern = medPattern;   // Medium
            else
                activePattern = hardPattern;   // Hard

            _lastInterval = -1;
        }

        public float GetIntervalLength(float bpm)
        {
            return 60f / (bpm * _steps);
        }

        public void CheckForNewInteval(float interval)
        {
            int currentStep = Mathf.FloorToInt(interval);
            if (activePattern.Length == 0)
            {
                return;
            }
            if (currentStep != _lastInterval)
            {
                _lastInterval = currentStep;
                int patternIndex = currentStep % activePattern.Length;
                if (activePattern[patternIndex])
                {
                    _trigger.Invoke();
                }
            }
        }
    }
}
