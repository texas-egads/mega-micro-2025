using UnityEngine;
using UnityEngine.UI;

namespace ZABsters {
    public class GameInitializer : MonoBehaviour
    {
        [System.NonSerialized]
        public bool isSantaWorkshop = false;

        [System.NonSerialized]
        public int taskNumber = 0;

        public Sprite[] sprites;

        public Image bgImage;

        private AudioClipsList audioClipsList;
        private AudioSource winSound;
        private AudioSource loseSound;
        private ToolSelectionAnimator toolAnimator; //to end ui animations

        private int sampleCorrectAnswer = 0;

        public GameObject[] taskObjects;
        public GameObject[] winObjects;
        public GameObject[] nonWinObjects;

        public AudioClip guitarClip;
        public AudioClip pixieClip;

        public AudioClip fireStartClip;
        public AudioClip extinguisherClip;
        public AudioClip timerClip;


        // public GameObject giftTaskObjects;
        // public GameObject snowTaskObjects;

        void Awake()
        {
            //ranodmly assign isSantaWorkshop value
            // Debug.Log("WSG BRODIE");
            // isSantaWorkshop = Random.value > 0.5f;

            //task number is randomly either 0, 1, 2:
            taskNumber = 0; //  Random.Range(0, 3);
            sampleCorrectAnswer = Random.Range(0, 3);
            Debug.Log("correct answer is: " + sampleCorrectAnswer);

            //if 0, set bgimage to sprites[0], otherwise set to sprites[1]
            bgImage.sprite = (taskNumber == 0) ? sprites[0] : sprites[1];

            //0 is gift task
            //1 is sleigh repair
            //2 is snow shoveling task
            if (taskNumber == 0)
            {
                //gift task
                taskObjects[0].SetActive(true);
            }
            else if (taskNumber == 1)
            {
                //sleigh repair task
                taskObjects[1].SetActive(true);
            }
            else if (taskNumber == 2)
            {
                //snow shoveling task
                taskObjects[2].SetActive(true);
            }

            audioClipsList = FindObjectOfType<AudioClipsList>();
            //find component in scene of tool slection animator
            toolAnimator = FindObjectOfType<ToolSelectionAnimator>();
            //get the win and lose sounds from the AudioClipsList component:
            winSound = Managers.AudioManager.CreateAudioSource();
            winSound.clip = audioClipsList.winClip;
            loseSound = Managers.AudioManager.CreateAudioSource();
            loseSound.clip = audioClipsList.loseClip;


        }

        void Update()
        {
            if(CheckKeyPressed())
            {
                //to initiate button ui feedback and fade out of ui
                int buttonIndex = -1;
                if (Input.GetKeyDown(KeyCode.W))
                {
                    buttonIndex = 0;
                }
                else if (Input.GetKeyDown(KeyCode.A))
                {
                    buttonIndex = 1;
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    buttonIndex = 2;
                }
                if (toolAnimator != null && buttonIndex != -1)
                {
                    toolAnimator.PulseButton(buttonIndex);
                    Invoke("CallFadeOut", 0.2f);
                }

                float endGameDelay = 0.5f;
                if(CheckTool())
                {
                    //if the tool is correct, declare the minigame won and end it
                    Managers.MinigamesManager.DeclareCurrentMinigameWon();
                    endGameDelay = winSound.clip.length;
                    audioClipsList.bgMusicSource.Stop();
                    winObjects[taskNumber].SetActive(true);
                    nonWinObjects[taskNumber].SetActive(false);
                    float delayBeforeWinSound = 0;
                    
                    //now lets do some specific shiz based off it

                    if(taskNumber == 1) // sleigh repair
                    {
                        AudioSource guitarSound = Managers.AudioManager.CreateAudioSource();
                        guitarSound.clip = guitarClip;
                        //so basically play guitar sound, then pixie sound at a slight delay:
                        guitarSound.Play();
                        Invoke("PlayPixieSound", guitarSound.clip.length);
                        endGameDelay = guitarSound.clip.length + pixieClip.length + 0.1f;
                    }
                    //Invoke("PlayWinSound", delayBeforeWinSound);

                }
                else
                {
                    //if the tool is incorrect, declare the minigame lost and end it
                    Managers.MinigamesManager.DeclareCurrentMinigameLost();
                    loseSound.Play();
                    endGameDelay = loseSound.clip.length;
                    audioClipsList.bgMusicSource.Stop();
                }
                //end minigame in 2 seconds:
                Invoke("EndMinigame", endGameDelay);

            }
        }

        void PlayWinSound()
        {
            winSound.Play();
        }

        void PlayPixieSound()
        {
            AudioSource pixieSound = Managers.AudioManager.CreateAudioSource();
            pixieSound.clip = pixieClip;
            pixieSound.Play();
        }

        void EndMinigame()
        {
            //end the minigame
            Managers.MinigamesManager.EndCurrentMinigame();
        }
        public bool CheckTool()
        {
            //correct asnwer. w is 0, a is 1, s is 2
            if(Input.GetKeyDown(KeyCode.W) && 0 == sampleCorrectAnswer)
            {
                return true;
            }
            else if (Input.GetKeyDown(KeyCode.A) && 1 == sampleCorrectAnswer)
            {
                return true;
            }
            else if (Input.GetKeyDown(KeyCode.S) && 2 == sampleCorrectAnswer)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        bool CheckKeyPressed()
        {
            //if either w, a, s keys are pressed, return true
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        void CallFadeOut()
        {
            if (toolAnimator != null)
            {
                toolAnimator.FadeOut();
            }
        }


    }
}