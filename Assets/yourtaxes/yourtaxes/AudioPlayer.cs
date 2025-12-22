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
                AudioSource win = Managers.AudioManager.CreateAudioSource();
                win.clip = winMusic;
                win.Play();
                startedPlaying = true;
            }
            if (wlc.hasLost && !startedPlaying && !source.isPlaying)
            {
                AudioSource lose = Managers.AudioManager.CreateAudioSource();
                lose.clip = loseMusic;
                lose.Play();
                startedPlaying = true;
            }
        }
    }
}

