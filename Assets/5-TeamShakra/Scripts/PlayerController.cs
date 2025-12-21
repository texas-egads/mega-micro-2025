using System;
using NUnit.Framework;
using UnityEditor.Build;
using UnityEngine;

namespace TeamShakra
{
  public class PlayerController : MonoBehaviour
    {
      // All floats in * per second
      [SerializeField] private const float ROTATION_SPEED = 100.0f; // degrees / second
      private const float SWINGING_START_SPEED = 3 * ROTATION_SPEED; 
      [SerializeField] private const float SWING_DRAG = 10.0f;
      [SerializeField] private const float MOVEMENT_SPEED = 10.0f; // units / second
      private float swingSpeed = 0.0f;

      // Start is called once before the first execution of Update after the MonoBehaviour is created
      void Start()
      {
        
      }

      // Update is called once per frame
      void Update()
      {
        // Get Inputs
        float movementInput = Input.GetAxis("Vertical") * MOVEMENT_SPEED;
        float rotationInput = Input.GetAxis("Horizontal") * ROTATION_SPEED;
        Debug.Log($"Rotation Input: {rotationInput}");
        bool spacePressed = Input.GetButtonDown("Space");
        if (spacePressed && swingSpeed <= 0.0f) {
          swingSpeed = SWINGING_START_SPEED;
        }

        // Update Positions
        float movementAmount = movementInput * Time.deltaTime;
        float rotationAmount;
        if (swingSpeed > 0.0f) {
          swingSpeed -= SWING_DRAG;
          rotationAmount = -1.0f * swingSpeed * Time.deltaTime;
        } else {
          rotationAmount = rotationInput * Time.deltaTime;
        }
        transform.Translate(0, movementAmount, 0);
        transform.Rotate(0, 0, rotationAmount);
      }
    }
}
