using UnityEngine;

namespace boinkthatboy
{
    [System.Serializable]
    public struct MinigameContext
    {
        public Rigidbody boatRb;
        public Rigidbody catRb;

        public PlayerMovement playerMovement;
        public Animator catAnimator;

        public Transform waterline;
        public Transform startPoint;
        public Transform goalPoint;

        public GameObject fishingRodObject;
        public Transform rodTip;
    }
}
