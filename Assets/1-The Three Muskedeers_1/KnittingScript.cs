using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace The_Three_Muskedeers
{
    public class MyScript : MonoBehaviour
    {
        [Header("UI")]
        public TextMeshProUGUI DifficultyText;
        public TextMeshProUGUI UIText;

        public Image Background;

        [Header("Sock GameObjects (Left → Right)")]
        public GameObject oneSock;
        public GameObject twoSock;
        public GameObject threeSock;
        public GameObject fourSock;
        public GameObject fiveSock;
        public GameObject sixSock;
        public GameObject sevenSock;
        public GameObject eightSock;
        public GameObject nineSock;
        public GameObject tenSock;

        [Header("Sock Sprites")]
        public Sprite redsock;     // W
        public Sprite orangesock;  // A
        public Sprite greensock;   // S
        public Sprite bluesock;    // D
        public Sprite transparent;

        [Header("Audio")]
        public AudioClip loopSound;
        public AudioClip bellSound;

        public AudioClip cowbellSound;

        private readonly KeyCode[] validKeys =
        {
            KeyCode.W,
            KeyCode.A,
            KeyCode.S,
            KeyCode.D
        };

        private KeyCode[] sequence;
        private int inputIndex;
        private bool inputLocked = false;
        private bool finished;

        private AudioSource loopSource;
        private Image[] sockImages;

        private void Start()
        {
            sockImages = new Image[]
            {
                oneSock.GetComponent<Image>(),
                twoSock.GetComponent<Image>(),
                threeSock.GetComponent<Image>(),
                fourSock.GetComponent<Image>(),
                sevenSock.GetComponent<Image>(),
                eightSock.GetComponent<Image>(),
                sixSock.GetComponent<Image>(),
                nineSock.GetComponent<Image>(),
                fiveSock.GetComponent<Image>(),
                tenSock.GetComponent<Image>()
            };

            float difficulty = Managers.MinigamesManager.GetCurrentMinigameDifficulty();

            int sequenceLength = GetSequenceLength(difficulty);
            sequence = GenerateSequence(sequenceLength);

            loopSource = Managers.AudioManager.CreateAudioSource();
            loopSource.loop = true;
            loopSource.clip = loopSound;
            loopSource.Play();

            StartCoroutine(ShowSequence());
        }

        private void Update()
        {
            if (inputLocked || finished) return;

            if (Input.GetKeyDown(KeyCode.W))
            {
                HandleInput(KeyCode.W);
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                HandleInput(KeyCode.A);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                HandleInput(KeyCode.S);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                HandleInput(KeyCode.D);
            }
            else
            {
                return;
            }
        }

        // -----------------------------
        // Difficulty → Length
        // -----------------------------
        int GetSequenceLength(float difficulty)
        {
            int res = 0;
            if (difficulty < 0.25)
                res = 4;   // Easy
            else if (difficulty < 0.50)
                res = 6;   // Medium
            else if (difficulty < 0.75)
                res = 8;   // Hard
            else
            {
                foreach (Image i in sockImages)
                {
                    i.rectTransform.position += Vector3.left * 35f;
                }
                res = 10;   // Extreme
            }
            for (int i=res; i<sockImages.Length; i++)
            {
                sockImages[i].enabled = false;
            }
            return res;
        }

        // -----------------------------
        // Sequence Generation
        // -----------------------------
        KeyCode[] GenerateSequence(int length)
        {
            KeyCode[] seq = new KeyCode[length];

            ClearSocks();

            int keyIndex = UnityEngine.Random.Range(0, validKeys.Length);
            for (int i = 0; i < length; i++)
            {
                seq[i] = validKeys[keyIndex];
                keyIndex = (keyIndex + UnityEngine.Random.Range(1, validKeys.Length)) % validKeys.Length;
            }

            return seq;
        }

        // -----------------------------
        // Visuals
        // -----------------------------
        IEnumerator ShowSequence()
        {
            inputIndex = 0;

            sockImages = new Image[]
            {
                oneSock.GetComponent<Image>(),
                twoSock.GetComponent<Image>(),
                threeSock.GetComponent<Image>(),
                fourSock.GetComponent<Image>(),
                fiveSock.GetComponent<Image>(),
                sixSock.GetComponent<Image>(),
                sevenSock.GetComponent<Image>(),
                eightSock.GetComponent<Image>(),
                nineSock.GetComponent<Image>(),
                tenSock.GetComponent<Image>()
            };

            int sockIndex = 0;
            for (int i = 0; i < sequence.Length; i++)
            {
                while (!sockImages[sockIndex].enabled) { sockIndex++; }
                sockImages[sockIndex].sprite = GetSockSprite(sequence[i]);
                sockIndex++;
            }

            yield break;
        }

        void ClearSocks()
        {
            foreach (var img in sockImages)
                img.sprite = transparent;

        }

        Sprite GetSockSprite(KeyCode key)
        {
            switch (key)
            {
                case KeyCode.W: return redsock;
                case KeyCode.A: return orangesock;
                case KeyCode.S: return greensock;
                case KeyCode.D: return bluesock;
                default: return transparent;
            }
        }

        // -----------------------------
        // Input Handling
        // -----------------------------
        private int persistentSockIndex = 0;
        void HandleInput(KeyCode key)
        {
            while (!sockImages[persistentSockIndex].enabled) { persistentSockIndex++; }
            if (key == sequence[inputIndex])
            {
                sockImages[persistentSockIndex].color = Color.gray;
                AudioSource keySound = Managers.AudioManager.CreateAudioSource();
                keySound.PlayOneShot(bellSound);
                inputIndex++;
                persistentSockIndex++;

                if (inputIndex >= sequence.Length)
                {
                    finished = true;
                    inputLocked = true;
                    Managers.MinigamesManager.DeclareCurrentMinigameWon();
                    Managers.MinigamesManager.EndCurrentMinigame(1);
                }
            }
            else
            {
                finished = true;
                inputLocked = true;
                AudioSource keySound = Managers.AudioManager.CreateAudioSource();
                keySound.PlayOneShot(cowbellSound);
                sockImages[persistentSockIndex].color = Color.red;
                Managers.MinigamesManager.DeclareCurrentMinigameLost();
                Managers.MinigamesManager.EndCurrentMinigame(1);
            }
        }
    }
}
