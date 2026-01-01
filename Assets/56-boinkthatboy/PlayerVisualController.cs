using UnityEngine;

namespace boinkthatboy
{
    public class PlayerVisualController : MonoBehaviour
    {
        [Header("References")]
        public PlayerMovement movement;
        public Animator animator;

        public Transform visualRoot;

        [Header("Animation Parameters")]
        public string movingBool = "Moving";
        public string groundedBool = "Grounded";
        public string velZFloat = "VelZ";
        public string speedFloat = "Speed";

        [Header("Triggers")]
        public string jumpTrigger = "Jump";
        public string landTrigger = "Land";

        [Header("Flip")]
        public float flipDeadzone = 0.02f;

        private int movingHash;
        private int groundedHash;
        private int velZHash;
        private int speedHash;
        private int jumpHash;
        private int landHash;

        private float baseScaleX;

        void Awake()
        {
            if (!movement) { movement = GetComponent<PlayerMovement>(); }
            if (!animator && visualRoot) { animator = visualRoot.GetComponentInChildren<Animator>(); }
            if (!visualRoot) { visualRoot = transform; }

            CacheHashes();

            baseScaleX = Mathf.Abs( visualRoot.localScale.x );
            if ( baseScaleX <= 0f ) { baseScaleX = 1f; }
        }

        void CacheHashes()
        {
            movingHash = Animator.StringToHash( movingBool );
            groundedHash = Animator.StringToHash( groundedBool );
            velZHash = Animator.StringToHash( velZFloat );
            speedHash = Animator.StringToHash( speedFloat );

            jumpHash = Animator.StringToHash( jumpTrigger );
            landHash = Animator.StringToHash( landTrigger );
        }

        void Update()
        {
            if ( !movement ) { return; }

            Vector3 v = movement.Velocity;
            float flatSpeed = new Vector2(v.x, v.y).magnitude;

            // flip left and right horizontally based on movement
            if (visualRoot != null)
            {
                if (v.x > flipDeadzone)
                {
                    visualRoot.localScale = new Vector3(-baseScaleX, visualRoot.localScale.y, visualRoot.localScale.z);
                }
                else if (v.x < -flipDeadzone)
                {
                    visualRoot.localScale = new Vector3(+baseScaleX, visualRoot.localScale.y, visualRoot.localScale.z);
                }
            }

            if ( !animator ) { return; }

            bool grounded = movement.IsGrounded;
            bool moving = flatSpeed > 0.05f;

            animator.SetBool(movingHash, moving);
            animator.SetBool(groundedHash, grounded);
            animator.SetFloat(velZHash, v.z);
            animator.SetFloat(speedHash, flatSpeed);

            if (movement.WasGrounded && !movement.IsGrounded)
            {
                // left ground
                if (!string.IsNullOrEmpty(jumpTrigger))
                    animator.SetTrigger(jumpHash);
            }
            else if (!movement.WasGrounded && movement.IsGrounded)
            {
                // landed
                if (!string.IsNullOrEmpty(landTrigger))
                    animator.SetTrigger(landHash);
            }
        }
    }
}


