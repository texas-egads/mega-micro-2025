
using UnityEngine;

namespace TeamShakra
{
  public class GameController : MonoBehaviour
  {
    // SPECIAL NAIL DEFINITIONS:
    // NAIL 1 POS: (-5.8, 2, 2) no rotation
    private Vector3 NAIL_1_POS = new Vector3(-5.8f, 2, 2);
    private Quaternion NAIL_1_ROTATION = Quaternion.identity;
    // NAIL 2 POS: (-2.5, -2, 2) 90 degree rotation
    private Vector3 NAIL_2_POS = new Vector3(-2.5f, -2, 2);
    private Quaternion NAIL_2_ROTATION = Quaternion.Euler(0, 0, 90);
    private float gameDifficulty;
    private int numNails = 0;
    [SerializeField] private GameObject nailPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      gameDifficulty = Managers.MinigamesManager.GetCurrentMinigameDifficulty();
      //Declare lost until all nails removed
      Managers.MinigamesManager.DeclareCurrentMinigameLost();
      // EASY DIFFICULTY: 1st Nail
      if (gameDifficulty <= 0.33f)
      {
        numNails = 1;
        spawnNail(NAIL_1_POS, NAIL_1_ROTATION);
      }
      // MEDIUM DIFFICULTY: 2nd Nail
      else if (gameDifficulty <= 0.66f)
      {
        numNails = 1;
        spawnNail(NAIL_2_POS, NAIL_2_ROTATION);
      }
      // HARD DIFFICULTY: Both Nails
      else
      {
        numNails = 2;
        spawnNail(NAIL_1_POS, NAIL_1_ROTATION);
        spawnNail(NAIL_2_POS, NAIL_2_ROTATION);
      }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void removeNail()
    {
      numNails--;
      if (numNails == 0)
      {
        Managers.MinigamesManager.DeclareCurrentMinigameWon();
        Managers.MinigamesManager.EndCurrentMinigame();
      }
    }

    public void timeUp()
    {
      Managers.MinigamesManager.EndCurrentMinigame();
    }

    private GameObject spawnNail(Vector3 position, Quaternion rotation)
    {
      // Scaling handled by prefab
      GameObject newNail = Instantiate(nailPrefab, position, rotation);
      return newNail;
    }
  }
}
