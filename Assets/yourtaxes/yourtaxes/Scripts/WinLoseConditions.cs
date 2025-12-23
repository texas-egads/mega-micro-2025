using NUnit.Framework;
using UnityEngine;

namespace yourtaxes
{
    public class WinLoseConditions : MonoBehaviour
    {
        //determines if the player has survived until the time to win, and brings up the correct screen
        public bool hasWon;
        public bool hasLost;
        public float timeToWin;
        public bool hasEnded;
        [SerializeField]
        private GameObject loseScreen;
        [SerializeField]
        private Animator winScreenAnimator;
        [SerializeField]
        private Animator loseScreenAnimator;
        [SerializeField]
        private Animator senator;
        [SerializeField]
        private AudioClip winSound;
        [SerializeField]
        private AudioClip loseSound;
        private GuamRestraint guamRestraint;
        private AudioPlayer audioPlayer;
        private float timer;



        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            timer = 0;
            guamRestraint = GetComponent<GuamRestraint>();
            audioPlayer = GetComponent<AudioPlayer>();

        }

        // Update is called once per frame
        void Update()
        {
            timer += Time.deltaTime;
            if (timer >= timeToWin && !hasEnded)
            {
                if (!guamRestraint.guamTipped)
                {
                    hasWon = true;
                    guamRestraint.lockGuam();
                    winScreenAnimator.Play("winMoveUp", 0);
                    senator.Play("SenatorWin");
                    audioPlayer.playAudio(winSound);
                    Managers.MinigamesManager.DeclareCurrentMinigameWon();
                }
                else
                {
                    Managers.MinigamesManager.DeclareCurrentMinigameLost();
                    hasLost = true;
                    senator.Play("senatorLose");
                    audioPlayer.playAudio(loseSound);
                    loseScreenAnimator.Play("loseMoveUp");
                }
                hasEnded = true;
            }
        }
    }
}

