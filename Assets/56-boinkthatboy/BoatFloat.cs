using UnityEngine;

namespace boinkthatboy
{
    public class BoatFloat : MonoBehaviour
    {
        [Header("Water plane")]
        public float waterHeightZ = 0.2f;

        [Header("Buoyancy")]
        public float floatStrength = 30f;   
        public float floatDamping = 6f;     

        [Header("Drag")]
        public float linearDragOnWater = 1.5f;
        public float angularDragOnWater = 2.0f;

        [Header("Stabilization")]
        public bool stabilizeUpright = true;
        public float uprightTorque = 8f;
        public float uprightDamping = 3f;

        Rigidbody rb;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        void FixedUpdate()
        {
            float z = rb.position.z;
            float zVel = rb.linearVelocity.z;

            float displacement = waterHeightZ - z; 
            float buoyancyForce = (displacement * floatStrength) - (zVel * floatDamping);

            rb.AddForce(Vector3.forward * buoyancyForce, ForceMode.Acceleration);

            // drag
            rb.linearDamping = linearDragOnWater;
            rb.angularDamping = angularDragOnWater;

            // avoid tipping
            if (stabilizeUpright)
            {
                Vector3 currentUp = transform.forward;                                    
            }
        }
    }
}

