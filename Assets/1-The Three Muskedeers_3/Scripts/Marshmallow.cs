using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace The_Three_Muskedeers
{
    public class Marshmallow : MonoBehaviour
    {
        public static AudioSource clipSource;
        public static AudioClip[] clips;
        public AudioClip winClip;
        public AudioClip loseClip;
        public List<MarshmallowMeow> marshmallows; // assign all marshmallow objects in inspector
        private int selectedIndex = 0;

        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip marshMusic;
        private int activeCount;

        void Start()
        {
            if (clips == null) clips = new AudioClip[2];
            clips[0] = winClip;
            clips[1] = loseClip;
            if (!clipSource) clipSource = Managers.AudioManager.CreateAudioSource();

            float difficulty = Managers.MinigamesManager.GetCurrentMinigameDifficulty();
            UpdateSelection();

            _audioSource = Managers.AudioManager.CreateAudioSource();
            _audioSource.loop = true;
            _audioSource.clip = marshMusic;
            _audioSource.Play();

            if (difficulty < 0.33) // Easy
                activeCount = 1;
            else if (difficulty < 0.66) // Medium
                activeCount = 2;
            else // Hard
                activeCount = 3;

            for (int i = 0; i < marshmallows.Count; i++)
            {
                bool shouldBeActive = i < activeCount;
                marshmallows[i].uiImage.gameObject.SetActive(shouldBeActive);
                float rand = Random.Range(0f, 1f);
                int initCook = rand <= 0.6f ? 0 : rand <= 0.25f ? 1 : 2;
                for (int j=0; j<initCook; j++)
                {
                    marshmallows[i].SwitchImage();
                }
            }
        }
        private int currentCook;
        void Update()
        {
            //Navigate between marshmallows
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                currentCook = 0;
                selectedIndex--;

                if (selectedIndex < 0)
                {
                    selectedIndex = activeCount - 1;
                }

                UpdateSelection();
            }
            else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                currentCook = 0;
                selectedIndex++;

                if (selectedIndex >= activeCount)
                {
                    selectedIndex = 0;
                }

                UpdateSelection();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                currentCook++;
                if(currentCook == 3)
                {
                    marshmallows[selectedIndex].SwitchImage();
                    CheckWinCondition();
                    currentCook = 0;
                }
            }
        }
        private bool gameOver;
        void CheckWinCondition()
        {
            if (gameOver) return;
            foreach (var marshmallow in marshmallows)
            {
                if (!marshmallow.uiImage.gameObject.activeSelf)
                    continue;
                if (marshmallow.CurrentIndex == 5)
                {
                    clipSource.PlayOneShot(clips[1]);
                    Managers.MinigamesManager.DeclareCurrentMinigameLost();
                    Managers.MinigamesManager.EndCurrentMinigame(0.5f);
                    gameOver = true;
                    return; // STOP CHECKING
                }
                if (marshmallow.CurrentIndex < 4)
                {
                    return;
                }
            }
            Managers.MinigamesManager.DeclareCurrentMinigameWon();
        }

        void UpdateSelection()
        {
            // Optional: visually indicate selection
            for (int i = 0; i < marshmallows.Count; i++)
            {
                if (i == selectedIndex)
                {
                    marshmallows[i].Highlight(true);  // Highlight active
                }
                else
                {
                    marshmallows[i].Highlight(false); // Remove highlight
                }
            }

        }
    }


    [System.Serializable]
    public class MarshmallowMeow
    {
        public Image uiImage;
        public Sprite[] images;
        public int currentIndex = 0;

        public int CurrentIndex => currentIndex;

        // Changes the specific marshmallow's image to the next stage
        public void SwitchImage()
        {
            if (currentIndex < images.Length - 1)
            {
                currentIndex++;
            }
            if (currentIndex >= images.Length)
            {
                return;
            }
            uiImage.sprite = images[currentIndex];
            if(currentIndex == 4)
            {
                Marshmallow.clipSource.PlayOneShot(Marshmallow.clips[0]);
            }
        }

        // Highlights the currently selected marshmallow
        public void Highlight(bool active)
        {
            uiImage.color = active ? Color.yellow : Color.white;
        }
    }
}