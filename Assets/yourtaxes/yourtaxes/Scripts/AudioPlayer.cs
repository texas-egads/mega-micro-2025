using UnityEngine;

namespace yourtaxes
{
    public class AudioPlayer : MonoBehaviour
    {
        [SerializeField]
        private AudioClip mainMusic;
        [SerializeField]
        private AudioClip loseMusic;
        [SerializeField]
        private AudioClip winMusic;
        private WinLoseConditions wlc;
        private bool startedPlaying;
        private AudioSource source;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            source = Managers.AudioManager.CreateAudioSource();
            source.clip = mainMusic;
            source.Play();
            wlc = GetComponent<WinLoseConditions>();
            startedPlaying = false;

        }

        // Update is called once per frame
        void Update()
        {

            if (wlc.hasWon && !startedPlaying && !source.isPlaying)
            {
                //Debug.Log("hasWon");
                playAudio(winMusic);
                startedPlaying = true;
            }
            if (wlc.hasLost && !startedPlaying)
            {
                if (source.isPlaying)
                {
                    source.Stop();
                }
                playAudio(loseMusic);
                startedPlaying = true;
            }
        }

        public void playAudio(AudioClip clipToPlay)
        {
            AudioSource audioSource = Managers.AudioManager.CreateAudioSource();
            audioSource.clip = clipToPlay;
            audioSource.Play();
        }
    }
}

