using UnityEditor.UI;
using UnityEngine;


namespace TeamShakra
{
  public class NailHandler : MonoBehaviour
  {
    [SerializeField] private float HAMMER_PULL_DISTANCE = 0.2f;
    [SerializeField] private int PULLS_NEEDED = 3;
    [SerializeField] private float FLY_TIME = 1.0f;
    [SerializeField] private float SPIN_SPEED = 1800f; // degrees per second
    private GameObject nailDest;
    private GameController gameController;
    private int hammerPulls = 0;
    private bool isFlying = false;
    private float flyTimer = 0f;
    private Vector3 startPosition;
    private Vector3 targetPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      nailDest = GameObject.FindWithTag("NailDestination");
      if (nailDest == null)
      {
        Debug.LogError("Nail Destination not found");
      }
      gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
      if (gameController == null)
      {
        Debug.LogError("Game Controller not found");
      }
    }

    // Update is called once per frame
    void Update()
    {
      if (isFlying)
      {
        flyTimer += Time.deltaTime;
        float t = flyTimer / FLY_TIME;
        
        if (t >= 1f)
        {
          // Reached destination, destroy the nail
          gameController.removeNail();
          Destroy(gameObject);
          return;
        }
        
        // Linear interpolation for horizontal movement
        Vector3 currentPos = Vector3.Lerp(startPosition, targetPosition, t);
        
        // Add parabolic arc (height based on sine wave)
        float arcHeight = Mathf.Sin(t * Mathf.PI) * Vector3.Distance(startPosition, targetPosition) * 0.5f;
        currentPos.y += arcHeight;
        
        transform.position = currentPos;
        
        // Spin the nail
        transform.Rotate(0, 0, SPIN_SPEED * Time.deltaTime);
      }
    }

    public Vector3 hammerPull()
    {
      hammerPulls++;
      if (hammerPulls == PULLS_NEEDED)
      {
        startNailToss();
        return Vector3.zero;
      }
      Vector3 movement = new Vector3(HAMMER_PULL_DISTANCE, 0, 0);
      transform.Translate(movement);
      
      //TODO: Add sound here
      //TODO: Add wood break particles/texture here
      
      return movement;
    }

    void startNailToss()
    {
      // Remove colliders
      Collider2D[] colliders = gameObject.GetComponents<Collider2D>();
      foreach (Collider2D collider in colliders)
      {
        Destroy(collider);  
      }
      
      // Initialize flight parameters
      startPosition = transform.position;
      targetPosition = nailDest.transform.position;
      isFlying = true;
      flyTimer = 0f;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
      if (other.CompareTag("Hammer"))
      {
        GameObject hammerObj = other.gameObject;
        PlayerController player = hammerObj.GetComponent<PlayerController>();
        player.intersectNail(gameObject);
      }
    }

    void OnTriggerExit2D(Collider2D other)
    {
      if (other.CompareTag("Hammer"))
      {
        GameObject hammerObj = other.gameObject;
        PlayerController player = hammerObj.GetComponent<PlayerController>();
        player.removeIntersectNail();
      }
    }
  }
}
