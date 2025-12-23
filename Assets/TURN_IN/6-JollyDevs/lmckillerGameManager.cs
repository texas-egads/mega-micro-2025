using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace JollyDevs
{
    public class NewMonoBehaviourScript : MonoBehaviour
    {
        public Slider progressBar;
        public Image fillBar;

        public AnimationCurve diffultyCurve;

        public AudioClip loopSound;
        public AudioClip difficultySound;
        public AudioClip loseSound;

        public SpriteRenderer patty;

        public Sprite rawPatty;
        public Sprite cookedPatty;
        public Sprite burntPatty;

        public Vector2 winArea;
        float speed;
        float decaySpeed;
        private bool canCook = true;

        private void Start()
        {

            float difficulty = Managers.MinigamesManager.GetCurrentMinigameDifficulty();

            speed = diffultyCurve.Evaluate(difficulty);
            decaySpeed = diffultyCurve.Evaluate(difficulty);

            Managers.MinigamesManager.DeclareCurrentMinigameLost();
            
            
            AudioSource loop = Managers.AudioManager.CreateAudioSource();
            loop.loop = true;
            if (difficulty >= 0.75)
            {
                loop.clip = difficultySound;
            }
            else
            {
                loop.clip = loopSound;
            }
            loop.Play();

        }

        private void Update()
        {
            if (canCook)
            {
                if (progressBar.value > 0)
                {
                    progressBar.value -= decaySpeed * Time.deltaTime;
                }
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    progressBar.value += speed;
                } // cooked
                if (progressBar.value >= winArea.x && progressBar.value <= winArea.y)
                {
                    fillBar.color = Color.yellow;
                    Managers.MinigamesManager.DeclareCurrentMinigameWon();
                    patty.sprite = cookedPatty;
                    
                } // burnt
                else if (progressBar.value > winArea.y)
                {
                    canCook = false;
                    Managers.MinigamesManager.DeclareCurrentMinigameLost();
                    fillBar.color = Color.red;
                    patty.sprite = burntPatty;
                    Managers.MinigamesManager.EndCurrentMinigame(1.5f);

                    AudioSource lose = Managers.AudioManager.CreateAudioSource();
                    lose.PlayOneShot(loseSound);

                } else if (progressBar.value < winArea.x) // raw
                {
                    Managers.MinigamesManager.DeclareCurrentMinigameLost();
                    fillBar.color = Color.green;
                    patty.sprite = rawPatty;
                }

                // Clamp value between 0 and max
                progressBar.value = Mathf.Clamp(progressBar.value, progressBar.minValue, progressBar.maxValue);
            }
            
        }
    }
}
