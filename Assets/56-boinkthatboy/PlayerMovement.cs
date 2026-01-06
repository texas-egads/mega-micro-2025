using UnityEngine;

// !!
// parts of this code was created with ChatGPT or Gemini
// !!

namespace boinkthatboy
{
    [RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement")]
        public float acceleration = 15f;
        public float maxSpeed = 1f;

        [Tooltip("ground braking : 1-no brake")]
        [Range(0f, 1f)] public float noInputBrakeMultiplier = 0.90f;

        [Header("Jump")]
        public float jumpImpulse = 4f;

        public float groundSkin = 0.03f;

        [Header("air control")]
        [Range(0f, 1f)] public float airControlSameDir = 0.25f;

        [Tooltip("in place air control.")]
        [Range(0f, 1f)] public float airControlInPlace = 0.45f;

        [Header("Landing")]
        public float landingBrakeGraceTime = 0.20f;

        [Range(0f, 1f)] public float landingMomentumMultiplier = 0.35f;

        public bool setLinearDampingFromScript = false;
        public float groundLinearDamping = 0.2f;
        public float airLinearDamping = 0.05f;

        private Rigidbody rb;
        private BoxCollider col;

        private bool isGrounded;
        private bool wasGrounded;
        private float lastLandedFixedTime = -999f;

        private Vector2 jumpInputRaw;
        private bool inPlaceJump;

        private LayerMask groundMask;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            col = GetComponent<BoxCollider>();

            //Physics.gravity = new Vector3( 0f, 0f, -9.81f );
           
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeRotation;

            int groundLayer = LayerMask.NameToLayer( "Ground" );
            groundMask = (groundLayer >= 0) ? (1 << groundLayer) : 0;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                float x = Input.GetAxisRaw( "Horizontal" );
                float y = -Input.GetAxisRaw( "Vertical" ); 
                jumpInputRaw = new Vector2(x, y);
                inPlaceJump = (jumpInputRaw == Vector2.zero);

                Jump();
            }
        }

        void FixedUpdate()
        {
            Vector3 customGravity = new Vector3(0f, 0f, -9.81f);
            rb.AddForce(customGravity * rb.mass, ForceMode.Force);

            wasGrounded = isGrounded;
            CheckGround();

            if (!wasGrounded && isGrounded)
            {
                lastLandedFixedTime = Time.fixedTime;

                Vector3 v = rb.linearVelocity;
                v.x *= landingMomentumMultiplier;
                v.y *= landingMomentumMultiplier;

                if (v.z > 0f) v.z = 0f;
                rb.linearVelocity = v;
            }

            if (setLinearDampingFromScript)
                rb.linearDamping = isGrounded ? groundLinearDamping : airLinearDamping;

            Move();
        }

        void Move()
        {
            float x = Input.GetAxisRaw( "Horizontal" );
            float y = -Input.GetAxisRaw( "Vertical" ); 
            Vector2 input = new Vector2(x, y);

            if (isGrounded)
            {
                rb.AddForce(new Vector3(input.x, input.y, 0f) * acceleration, ForceMode.Acceleration);

                bool inLandingGrace = (Time.fixedTime - lastLandedFixedTime) < landingBrakeGraceTime;

                if (!inLandingGrace && input == Vector2.zero)
                {
                    Vector3 v = rb.linearVelocity;
                    v.x *= noInputBrakeMultiplier;
                    v.y *= noInputBrakeMultiplier;
                    rb.linearVelocity = v;
                }
            }
            else
            {
                if (inPlaceJump)
                {
                    rb.AddForce(new Vector3(input.x, input.y, 0f) * (acceleration * airControlInPlace), ForceMode.Acceleration);
                }
                else
                {
                    Vector2 allowed = input;

                    if (jumpInputRaw.x == 0f) allowed.x = 0f;
                    else if (Mathf.Sign(allowed.x) != Mathf.Sign(jumpInputRaw.x)) allowed.x = 0f;

                    if (jumpInputRaw.y == 0f) allowed.y = 0f;
                    else if (Mathf.Sign(allowed.y) != Mathf.Sign(jumpInputRaw.y)) allowed.y = 0f;

                    rb.AddForce(new Vector3(allowed.x, allowed.y, 0f) * (acceleration * airControlSameDir), ForceMode.Acceleration);
                }
            }

            Vector3 vel = rb.linearVelocity;
            Vector2 flat = new Vector2(vel.x, vel.y);

            if (flat.magnitude > maxSpeed)
            {
                flat = flat.normalized * maxSpeed;
                vel.x = flat.x;
                vel.y = flat.y;
                rb.linearVelocity = vel;
            }
        }

        void Jump()
        {
            Vector3 v = rb.linearVelocity;
            v.z = 0f;
            rb.linearVelocity = v;

            rb.AddForce(Vector3.forward * jumpImpulse, ForceMode.Impulse);
        }

        void CheckGround()
        {
            if (groundMask == 0)
            {
                isGrounded = false;
                return;
            }

            Vector3 center = col.bounds.center;
            Vector3 halfExtents = col.bounds.extents;

            isGrounded = Physics.BoxCast(
                center,
                halfExtents * 0.95f,
                Vector3.back,
                transform.rotation,
                groundSkin,
                groundMask,
                QueryTriggerInteraction.Ignore
            );
        }

        public bool IsGrounded => isGrounded;
        public bool WasGrounded => wasGrounded;
        public Rigidbody Rigidbody => rb;
        public Vector3 Velocity => rb != null ? rb.linearVelocity : Vector3.zero;
    }
    
}
