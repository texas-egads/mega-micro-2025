using UnityEngine;

namespace boinkthatboy
{
    public class GameRefs : MonoBehaviour
    {
        [Header("Core Refs")]
        public Rigidbody boatRb;
        public Rigidbody catRb;
        public Animator catAnimator;
        public PlayerMovement playerMovement; 

        [Header("Anchors")]
        public Transform waterline;  
        public Transform startPoint;  
        public Transform goalPoint;   

        [Header("Fishing Props")]
        public GameObject fishingRodObject; 
        public Transform rodTip;        

    }
}
