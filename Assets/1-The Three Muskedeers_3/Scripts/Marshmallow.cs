using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace The_Three_Muskedeers
{
    public class Marshmallow : MonoBehaviour
    {
        public List<MarshmallowMeow> marshmallows; // assign all marshmallow objects in inspector
        private int selectedIndex = 0;

        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip marshMusic;

        void Start()
        {
            float difficulty = Managers.MinigamesManager.GetCurrentMinigameDifficulty();
            UpdateSelection();

            _audioSource = Managers.AudioManager.CreateAudioSource();
            _audioSource.loop = true;
            _audioSource.clip = marshMusic;
            _audioSource.Play();

            int activeCount = 1;

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
            }
        }

        void Update()
        {
            //Navigate between marshmallows
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                selectedIndex--;

                if (selectedIndex < 0)
                {
                    selectedIndex = marshmallows.Count - 1;
                }

                UpdateSelection();
            }
            else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                selectedIndex++;

                if (selectedIndex >= marshmallows.Count)
                {
                    selectedIndex = 0;
                }

                UpdateSelection();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                marshmallows[selectedIndex].SwitchImage();
                CheckWinCondition();
            }
        }

        void CheckWinCondition()
        {
            foreach (var marshmallow in marshmallows)
            {
                if (!marshmallow.uiImage.gameObject.activeSelf)
                    continue;
                if (marshmallow.CurrentIndex == 5)
                {
                    Managers.MinigamesManager.DeclareCurrentMinigameLost();
                    Managers.MinigamesManager.EndCurrentMinigame(0.5f);
                    return; // STOP CHECKING
                }
                if (marshmallow.CurrentIndex == 0)
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
        }

        // Highlights the currently selected marshmallow
        public void Highlight(bool active)
        {
            uiImage.color = active ? Color.yellow : Color.white;
        }
    }
}