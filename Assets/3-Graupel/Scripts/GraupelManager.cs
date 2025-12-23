using UnityEngine;

namespace Graupel 
{
    public class GraupelManager : MonoBehaviour
    {
        private RectTransform rectTransform;
        private bool hasCollided = false;

        void Start()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        void Update() 
        {
            if (!hasCollided)
            {
                CheckPlayerOverlap();
            }
        }

        void CheckPlayerOverlap()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            RectTransform playerRect = player.GetComponent<RectTransform>();

            if (RectOverlaps(rectTransform, playerRect))
            {
                hasCollided = true;

                if (CompareTag("Object 1"))
                {   
                    Debug.Log("Touched Object 1");
                    Managers.MinigamesManager.DeclareCurrentMinigameWon();
                    Managers.MinigamesManager.EndCurrentMinigame();
                }
                else if (CompareTag("Object 2"))
                {
                    Debug.Log("Touched Object 2");
                    Managers.MinigamesManager.DeclareCurrentMinigameLost();
                    Managers.MinigamesManager.EndCurrentMinigame();
                }
            }
        }

        bool RectOverlaps(RectTransform a, RectTransform b)
        {
            Rect rectA = GetWorldRect(a);
            Rect rectB = GetWorldRect(b);
            return rectA.Overlaps(rectB);
        }

        Rect GetWorldRect(RectTransform rt)
        {
            Canvas.ForceUpdateCanvases();

            Vector3[] corners = new Vector3[4];
            rt.GetWorldCorners(corners);

            return new Rect(corners[0].x, corners[0].y, corners[2].x - corners[0].x, corners[2].y - corners[0].y);
        }
    }
}

