using UnityEngine;

namespace ZABsters {
    public abstract class ITask : MonoBehaviour
    {
        void Update()
        {
            if(CheckTool())
            {
                // Perform the task using the tool
                //and now just put win logic:
                Debug.Log("Won!");
                Managers.MinigamesManager.DeclareCurrentMinigameWon();
                Managers.MinigamesManager.EndCurrentMinigame();

            }
        }
        public abstract bool CheckTool();
    }
}
