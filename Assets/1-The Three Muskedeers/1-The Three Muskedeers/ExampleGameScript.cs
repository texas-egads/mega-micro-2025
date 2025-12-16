using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TEAM_NAME_SPACE{
    public class ExampleGameScript : MonoBehaviour
    {
        // DELETE THIS FILE BEFORE YOU SUBMIT //
        public TextMeshProUGUI DifficultyText;
        public TextMeshProUGUI UIText;
        public AnimationCurve SpacePressesNeeded;
        public string winText;

        public AudioClip loopSound;
        public AudioClip winSound;

        private int spaceCount;

        private void Start()
        {
            float difficulty = Managers.MinigamesManager.GetCurrentMinigameDifficulty();
            DifficultyText.text = $"Current Difficulty: {difficulty.ToString()}";
            spaceCount = Mathf.CeilToInt(SpacePressesNeeded.Evaluate(difficulty));

            UIText.text = $"Press space {spaceCount} times!";
            
            AudioSource loop = Managers.AudioManager.CreateAudioSource();
            loop.loop = true;
            loop.clip = loopSound;
            loop.Play();
        }

        private void Update()
        {
            if (Input.GetButtonDown("Space"))
            {
                spaceCount--;
                UIText.text = $"Press space {spaceCount} times!";
            }

            if(spaceCount == 0)
            {
                UIText.text = winText;

                AudioSource win = Managers.AudioManager.CreateAudioSource();
                win.PlayOneShot(winSound);

                Managers.MinigamesManager.DeclareCurrentMinigameWon();
                Managers.MinigamesManager.EndCurrentMinigame(1f);
                this.enabled = false;
            }
        }
    }
}
