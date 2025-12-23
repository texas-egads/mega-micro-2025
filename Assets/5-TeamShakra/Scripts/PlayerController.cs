using System;
using NUnit.Framework;
using UnityEditor.Build;
using UnityEngine;

namespace TeamShakra
{
  public class PlayerController : MonoBehaviour
  {
    // All floats in * per second
    [SerializeField] private float ROTATION_SPEED; // degrees / second
    private float swingStartSpeed; 
    [SerializeField] private float SWING_TIME; // seconds
    [SerializeField] private float MOVEMENT_SPEED; // units / second
    private float swingTimer = 0.0f;
    private GameObject intersectingNail = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      swingStartSpeed = 10 * ROTATION_SPEED;
    }

    // Update is called once per frame
    void Update()
    {
      // Get Inputs
      float movementInput = Input.GetAxis("Vertical") * MOVEMENT_SPEED;
      // For some reason the wrong way around?
      float rotationInput = Input.GetAxis("Horizontal") * ROTATION_SPEED * -1;
      bool spacePressed = Input.GetButtonDown("Space");
      if (spacePressed && swingTimer <= 0.0f) 
      {
        if (intersectingNail != null)
        {
          NailHandler nailScript = intersectingNail.GetComponent<NailHandler>();
          Vector3 worldMovement = nailScript.hammerPull();
          transform.position += worldMovement;
        }
        else
        {
          swingTimer = SWING_TIME;
        }
      }

      // Update Positions
      float movementAmount = movementInput * Time.deltaTime;
      float rotationAmount;
      if (swingTimer > 0.0f) 
      {
        swingTimer -= Time.deltaTime;
        float swingPercentComplete = (SWING_TIME - swingTimer) / SWING_TIME;
        float swingSpeed = Mathf.Lerp(swingStartSpeed, 0.0f, swingPercentComplete);
        rotationAmount = -1.0f * swingSpeed * Time.deltaTime;
      } 
      else 
      {
        rotationAmount = rotationInput * Time.deltaTime;
      }
      transform.Translate(0, movementAmount, 0);
      transform.Rotate(0, 0, rotationAmount);
    }

    public void intersectNail(GameObject nail)
    {
      intersectingNail = nail;
    }

    public void removeIntersectNail()
    {
      intersectingNail = null;
    }
  }
}
