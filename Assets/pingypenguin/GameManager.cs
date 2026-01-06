using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;
using UnityEngine;

namespace pingypenguin {
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private AudioClip musicClip;
        [SerializeField] private AudioClip grassCutSound;
        [SerializeField] private AudioClip winClip;
        [SerializeField] private GameObject grassObj;
        [SerializeField] private GameObject player;

        public AnimationCurve GrassBlocksNeeded;
        private int grassLeft;

        private AudioSource cutSource;
        private AudioSource music;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            float difficulty = Managers.MinigamesManager.GetCurrentMinigameDifficulty();
            grassLeft = Mathf.CeilToInt(GrassBlocksNeeded.Evaluate(difficulty));
            CreateGrassChunk();

            music = Managers.AudioManager.CreateAudioSource();
            music.clip = musicClip;
            music.Play();
            cutSource = Managers.AudioManager.CreateAudioSource();
            cutSource.clip = grassCutSound;
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        // Decrements the grass count by one and checks if there is no grass left
        // The game is won if no grass is left
        public void GrassCut() {
            grassLeft--;
            cutSource.Play();
            if (grassLeft == 0) {
                Managers.MinigamesManager.DeclareCurrentMinigameWon();
                Managers.MinigamesManager.EndCurrentMinigame(1f);
                player.GetComponent<ParticleSystem>().Emit(20);
                player.GetComponent<PlayerController>().movementSpeed = 0f;
                AudioSource winSource = Managers.AudioManager.CreateAudioSource();
                winSource.clip = winClip;
                winSource.Play();
                music.Stop();
                this.enabled = false;
            }
        }

        // Creates a semi-rectangular chunk of grass based on the difficulty provided
        // The chunk "expands" from the center outward, ensuring that each sub-rectangular
        // chunk is filled before increasing the size and adding new grass tiles
        private void CreateGrassChunk() {
            int grassPlaced = 0;
            int desiredWidth = 2;
            int desiredHeight = 1;
            List<Vector2> validPositions = CreateValidPositions(desiredWidth, desiredHeight);
            validPositions.Shuffle();
            // Keeps placing grass tiles until the number of grass tiles equals grassLeft
            while (grassPlaced < grassLeft) {
                // If all valid positions have been used, then increase the desiredWidth and desiredHeight,
                // and get the new valid positions for that outer rectangular border
                if (validPositions.Count == 0) {
                    desiredWidth += 2;
                    desiredHeight++;
                    validPositions = CreateValidPositions(desiredWidth, desiredHeight);
                    validPositions.Shuffle();
                }
                GameObject newGrass = Instantiate(grassObj);
                newGrass.GetComponent<GrassObject>().gm = this;
                // Since the list is shuffled, the current first element of the list should be random
                Vector2 newPos = validPositions[0];
                newGrass.transform.position = newPos;
                validPositions.Remove(newPos);
                grassPlaced++;
            }
        }

        /* Gets and returns the valid positions for the outer rectangular border of the 
        // desired width and height
        */
        private List<Vector2> CreateValidPositions(int desiredWidth, int desiredHeight) {
            List<Vector2> validPositions = new List<Vector2>();
            float minY = -(desiredHeight + 1) / 2 + 0.5f;
            float maxY = desiredHeight / 2 - 0.5f;
            float maxX = desiredWidth / 2 - 0.5f;
            // Adds the positions on the left and right side of the border
            // If desiredHeight is even, start from the bottom and go up
            // Otherwise, start from the top and go down
            for (int h = 0; h < desiredHeight - 1; h++) {
                validPositions.Add(new Vector2(-maxX, desiredHeight % 2 == 0 ? minY + h : maxY - h));
                validPositions.Add(new Vector2(maxX, desiredHeight % 2 == 0 ? minY + h : maxY - h));
            }
            // Adds the positions on the edge that is either above the rectangle if
            // desiredHeight is even, or below the rectangle if desiredHeight is odd
            for (int w = 0; w < desiredWidth; w++) {
                validPositions.Add(new Vector2(-maxX + w, desiredHeight % 2 == 0 ? maxY : minY));
            }
            return validPositions;
        }
    }
}
