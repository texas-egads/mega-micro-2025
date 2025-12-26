using System;
using UnityEngine;

namespace pingypenguin {
    public class PlayerController : MonoBehaviour
    {
        public float movementSpeed = 2f;
        [SerializeField] private AudioClip cutSound;
        [SerializeField] private AutoShadow shadow;

        private float cooldown = 0.2f;
        private float cooldownTimeRemaining = 0f;

        private Rigidbody2D rb;
        private CircleCollider2D hitbox;
        private Animator playerAnimator;

        private Vector2 moveDir;
        private AudioSource sounds;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            playerAnimator = GetComponent<Animator>();
            hitbox = GetComponent<CircleCollider2D>();
            hitbox.enabled = false;
            hitbox.offset = Vector2.up * 0.5f;

            sounds = Managers.AudioManager.CreateAudioSource();
            sounds.clip = cutSound;
        }

        // Update is called once per frame
        void Update()
        {
            moveDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
            cooldownTimeRemaining -= Time.deltaTime;

            // If space is pressed and the cooldown period has passed,
            // cut any grass within the scissors hitbox
            if (cooldownTimeRemaining < 0f && Input.GetButton("Space"))
            {
                cooldownTimeRemaining = cooldown;
                playerAnimator.Play("cut");
                sounds.Play();
            }
        }

        // Updates the RigidBody2D's velocity and makes the player face
        // the direction of motion
        private void FixedUpdate() {
            rb.linearVelocity = moveDir * movementSpeed;
            if (movementSpeed > 0 && moveDir != Vector2.zero) {
                transform.rotation = Quaternion.LookRotation(Vector3.forward, moveDir);
                shadow.ReloadShadow();
            }
        }
    }
}
