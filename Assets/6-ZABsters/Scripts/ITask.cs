using UnityEngine;

namespace ZABsters {
    public abstract class ITask : MonoBehaviour
    {
        //ideas of other shi to add for the tasks:
        //- num of times u need to press shi to win, like spacebar
        //u need to hold spacebar for some length etc
        //make the shiz more interesting.
        private AudioClipsList audioClipsList;
        private AudioSource winSound;
        private AudioSource loseSound;
        void Start()
        {
            //find component in scene of AudioClipsList:
            audioClipsList = FindObjectOfType<AudioClipsList>();
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
                float endGameDelay = 0.5f;
                if(CheckTool())
                {
                    //if the tool is correct, declare the minigame won and end it
                    Managers.MinigamesManager.DeclareCurrentMinigameWon();
                    winSound.Play();
                    endGameDelay = winSound.clip.length;
                    audioClipsList.bgMusicSource.Stop();
                    //play the correct sound:

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
                Invoke("EndMinigame", endGameDelay + 0.3f);

            }
        }

        void EndMinigame()
        {
            //end the minigame
            Managers.MinigamesManager.EndCurrentMinigame();
        }
        public abstract bool CheckTool();

        bool CheckKeyPressed()
        {
            //if either w, a, s, d keys are pressed, return true
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
