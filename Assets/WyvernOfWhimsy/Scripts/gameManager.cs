using UnityEngine;
using System.Collections;

namespace WyvernOfWhimsy
{
    public class gameManager : MonoBehaviour
    {
        public float fails;
        public AudioSource musicSource;
        public AudioSource failSource;
        public AudioSource succeedSource;
        public static gameManager instance;
        [SerializeField] AudioClip gameMusic;
        [SerializeField] AudioClip failNote;
        [SerializeField] AudioClip succeedNote;
        [SerializeField] hitterMover HitterMover;
        private int failCutoff;
        

        private void Start()
        {
            StartCoroutine(DoTimer(10f));
            failCutoff = 5 - (int)(Managers.MinigamesManager.GetCurrentMinigameDifficulty() * 5);
            if (failCutoff < 1) failCutoff = 1;
            fails = 0;
            instance = this;
            musicSource = Managers.AudioManager.CreateAudioSource();
            failSource = Managers.AudioManager.CreateAudioSource();
            succeedSource = Managers.AudioManager.CreateAudioSource();
            musicSource.clip = gameMusic;
            failSource.clip = failNote;
            succeedSource.clip = succeedNote;
            musicSource.Play();
        }

        public void handHit(float handNum)
        {
            succeedSource.Play();
            HitterMover.AnimateHitter();
        }

        public void handMissed(float handNum, deerSprites DeerSprites)
        {
            failSource.Play();
            DeerSprites.AngerUp();
            fails++;
            if (failCutoff <= fails)
            {
                Managers.MinigamesManager.DeclareCurrentMinigameLost();
                Managers.MinigamesManager.EndCurrentMinigame(0.5f);
            }
        }

        private IEnumerator DoTimer(float time)
        {
            while (time > 0)
            {
                yield return null;
                time -= Time.deltaTime;
            }
            Managers.MinigamesManager.DeclareCurrentMinigameWon();
            Managers.MinigamesManager.EndCurrentMinigame();

        }
    }
}
