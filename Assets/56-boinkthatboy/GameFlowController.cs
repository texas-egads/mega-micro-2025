using UnityEngine;

namespace boinkthatboy
{
    public class GameFlowController : MonoBehaviour
    {
        [Header("Refs")]
        public GameRefs refs;

        [Header("Minigame Prefab")]
        public GameObject fishingHookMinigamePrefab;

        GameObject activeMinigame;

        void Start()
        {

            StartFishingHook();
        }

        void StartFishingHook()
        {
            if (activeMinigame != null)
                Destroy(activeMinigame);

            activeMinigame = Instantiate(fishingHookMinigamePrefab);

            var mg = activeMinigame.GetComponent<IMinigame>();
            var ctx = new MinigameContext
            {
                boatRb = refs.boatRb,
                catRb = refs.catRb,
                catAnimator = refs.catAnimator,
                playerMovement = refs.playerMovement,

                waterline = refs.waterline,
                startPoint = refs.startPoint,
                goalPoint = refs.goalPoint,

                fishingRodObject = refs.fishingRodObject,
                rodTip = refs.rodTip
            };

            mg.begin(ctx);
        }
    }
}
