using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace lmckiller
{
    public class NewMonoBehaviourScript : MonoBehaviour
    {
        public TextMeshProUGUI DifficultyText;
        public TextMeshProUGUI UIText;
        public Slider progressBar;

        public Vector2 winArea;
        public float speed;
        public float decaySpeed;
        private bool canCook = true;

        private void Start()
        {

            float difficulty = Managers.MinigamesManager.GetCurrentMinigameDifficulty();
            DifficultyText.text = $"Current Difficulty: {difficulty.ToString()}";

            UIText.text = "Press space to cook!";

            Managers.MinigamesManager.DeclareCurrentMinigameLost();

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
                }
                if (progressBar.value >= winArea.x && progressBar.value <= winArea.y)
                {
                    Managers.MinigamesManager.DeclareCurrentMinigameWon();
                }
                else if (progressBar.value > winArea.y)
                {
                    canCook = false;
                    Managers.MinigamesManager.DeclareCurrentMinigameLost();
                }

                // Clamp value between 0 and max
                progressBar.value = Mathf.Clamp(progressBar.value, progressBar.minValue, progressBar.maxValue);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (targetObject != null)
                {
                    // If it's on, turn it off. If it's off, turn it on.
                    bool isVisible = targetObject.activeSelf;
                    targetObject.SetActive(!isVisible);
                }
            }


        }
    }
}
