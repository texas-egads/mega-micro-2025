using UnityEngine;

namespace ZABsters {
    public abstract class ITask : MonoBehaviour
    {
        void Update()
        {
            if(CheckKeyPressed())
            {
                if(CheckTool())
                {
                    //if the tool is correct, declare the minigame won and end it
                    Managers.MinigamesManager.DeclareCurrentMinigameWon();
                }
                else
                {
                    //if the tool is incorrect, declare the minigame lost and end it
                    Managers.MinigamesManager.DeclareCurrentMinigameLost();
                }
                Managers.MinigamesManager.EndCurrentMinigame();

            }
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
