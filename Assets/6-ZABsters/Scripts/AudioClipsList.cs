using UnityEngine;


namespace ZABsters {
    public class AudioClipsList : MonoBehaviour
    {
        public AudioClip bgMusicClip;
        public AudioClip winClip;
        public AudioClip loseClip;


        [System.NonSerialized]
        public AudioSource bgMusicSource;

        void Start()
        {
            bgMusicSource = Managers.AudioManager.CreateAudioSource();
            bgMusicSource.clip = bgMusicClip;
            bgMusicSource.loop = true;
            bgMusicSource.Play();
        }

    }
}