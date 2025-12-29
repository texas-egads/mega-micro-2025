using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace ZABsters {
    public class ToolSelectionAnimator : MonoBehaviour
    {
        [Header("UI references")]
        public CanvasGroup canvasGroup;
        public Image glow1; //glow under first button
        public Image glow2; //glow under second button
        public Image glow3; //glow under third button
        
        [Header("tool sprites for fixing gift")] 
        public GameObject teddyBear;
        public GameObject extinguisher; 
        public GameObject appleCore;
        
        [Header("tool sprites for snow shoveling")]
        public GameObject shovel;
        public GameObject camera;
        public GameObject clock; 
        
        [Header("tool sprites for sleigh repair")]
        public GameObject hammer;
        public GameObject guitar;
        public GameObject dynamite;

        [Header("animation settings")]
        public float fadeInDuration = 0.5f;
        public float moveDistance = 20f;
        public float glowFadeDuration = 0.4f;
        public float glowInterval = 1f; //time between glow switches to match timer
        private GameInitializer gameInitializer;
        private bool isAnimating = false;
        private int currentGlowIndex = 0;
        private Coroutine glowCoroutine;
        //implicit button reference
        public int correctAnswer; //0 = w, 1 = a, 2 = s
        
        [Header("tool slots")]
        public Transform leftSlot;   //W button position
        public Transform centerSlot; //A button position
        public Transform rightSlot;  //S button position

        void Awake()
        {
            //check isSantaWorkshop w GameInitializer game object
            gameInitializer = FindObjectOfType<ZABsters.GameInitializer>();
            //have glow state set on the first button
            canvasGroup.alpha = 0f;
            SetGlowAlpha(glow1, 0f);
            SetGlowAlpha(glow2, 0f);
            SetGlowAlpha(glow3, 0f);
            //SetupToolSprites();
            //canvas should start disabled, so this waits for it to be enabled
        }
        void OnEnable()
        {
            if (!isAnimating)
            {
                SpawnRandomizedTools();
                StartCoroutine(FadeInAnimation());
            }
        }
        void SpawnRandomizedTools()
        {
            if (gameInitializer != null)
            {
                int taskNumber = gameInitializer.taskNumber;
                GameObject[] toolsForTask = GetToolsForTask(taskNumber, out GameObject correctTool);
                Transform[] slots = { leftSlot, centerSlot, rightSlot };
                //setting workshop tools randomly
                List<Transform> availableSlots = new List<Transform>(slots);
                List<int> availableButtons = new List<int> { 0, 1, 2 }; // W, A, S
                foreach (GameObject toolPrefab in toolsForTask)
                {
                    int randomIndex = Random.Range(0, availableSlots.Count);
                    Transform slot = availableSlots[randomIndex];
                    int buttonIndex = availableButtons[randomIndex];
                    //spawn tool in slot
                    GameObject tool = Instantiate(toolPrefab, slot);
                    RectTransform rectTransform = tool.GetComponent<RectTransform>();
                    rectTransform.anchoredPosition = Vector2.zero; // Center in slot
                    // setcorrect answer to button
                    if (toolPrefab == correctTool)
                    {
                        correctAnswer = buttonIndex;
                    }
                    availableSlots.RemoveAt(randomIndex);
                    availableButtons.RemoveAt(randomIndex);
                }
            }
        }
//         void SpawnRandomizedTools()
// {
//     Debug.Log("=== SpawnRandomizedTools START ===");
    
//     if (gameInitializer == null)
//     {
//         Debug.LogError("GameInitializer is NULL!");
//         return;
//     }
    
//     int taskNumber = gameInitializer.taskNumber;
//     Debug.Log($"Task Number: {taskNumber}");
    
//     GameObject correctTool;
//     GameObject[] toolsForTask = GetToolsForTask(taskNumber, out correctTool);
    
//     Debug.Log($"Tools to spawn: {toolsForTask.Length}");
//     Debug.Log($"Correct tool: {(correctTool != null ? correctTool.name : "NULL")}");
    
//     if (leftSlot == null) Debug.LogError("leftSlot is NULL!");
//     if (centerSlot == null) Debug.LogError("centerSlot is NULL!");
//     if (rightSlot == null) Debug.LogError("rightSlot is NULL!");
    
//     Transform[] slots = { leftSlot, centerSlot, rightSlot };
    
//     // Shuffle the slots
//     List<Transform> availableSlots = new List<Transform>(slots);
//     List<int> availableButtons = new List<int> { 0, 1, 2 };
    
//     foreach (GameObject toolPrefab in toolsForTask)
//     {
//         Debug.Log($"Processing tool: {(toolPrefab != null ? toolPrefab.name : "NULL PREFAB")}");
        
//         int randomIndex = Random.Range(0, availableSlots.Count);
//         Transform slot = availableSlots[randomIndex];
//         int buttonIndex = availableButtons[randomIndex];
        
//         Debug.Log($"Spawning in slot: {slot.name} at button index: {buttonIndex}");
        
//         // Spawn tool in slot
//         GameObject tool = Instantiate(toolPrefab, slot);
//         Debug.Log($"Tool instantiated: {tool.name}");
        
//         RectTransform rectTransform = tool.GetComponent<RectTransform>();
//         rectTransform.anchoredPosition = Vector2.zero;
        
//         Debug.Log($"Tool position set to: {rectTransform.anchoredPosition}");
        
//         // Check if this is the correct answer
//         if (toolPrefab == correctTool)
//         {
//             correctAnswer = buttonIndex;
//             Debug.Log($"CORRECT ANSWER SET TO: {correctAnswer}");
//         }
        
//         availableSlots.RemoveAt(randomIndex);
//         availableButtons.RemoveAt(randomIndex);
//     }
    
//     Debug.Log("=== SpawnRandomizedTools END ===");
// }
        GameObject[] GetToolsForTask(int taskNumber, out GameObject correctTool)
        {
            switch (taskNumber)
            {
                case 0: //gift wrapping task
                    correctTool = extinguisher;
                    return new GameObject[] { teddyBear, extinguisher, appleCore };
                    
                case 1: //sleigh repair task
                    correctTool = guitar;
                    return new GameObject[] { hammer, guitar, dynamite };
                    
                case 2: //snow shoveling task
                    correctTool = clock;
                    return new GameObject[] { shovel, camera, clock };
                    
                default:
                    correctTool = null;
                    return new GameObject[0];
            }
        }
        IEnumerator FadeInAnimation()
        {
            isAnimating = true;
            
            //store original position
            RectTransform rectTransform = GetComponent<RectTransform>();
            Vector2 originalPosition = rectTransform.anchoredPosition;
            Vector2 startPosition = originalPosition + Vector2.down * moveDistance; //set up slight movement animation
            rectTransform.anchoredPosition = startPosition;
            float elapsed = 0f;
            //fade in and move up
            //timer handler
            while (elapsed < fadeInDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeInDuration;
                
                //smooth fade and movement
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
                rectTransform.anchoredPosition = Vector2.Lerp(startPosition, originalPosition, t);
                
                yield return null;
            }
            canvasGroup.alpha = 1f;
            rectTransform.anchoredPosition = originalPosition;
            //done animating when in orgiinal position
            isAnimating = false;
            //start glow animation loop
            glowCoroutine = StartCoroutine(GlowCycleAnimation());
        }
        IEnumerator GlowCycleAnimation()
        {
            //glow1 -> glow2 -> glow3 -> repeat
            Image[] glows = { glow1, glow2, glow3 }; 
            while (true)
            {
                //fade in current glow
                yield return StartCoroutine(FadeGlow(glows[currentGlowIndex], 0f, 1f, glowFadeDuration));
                //wait for the rest of the second
                yield return new WaitForSeconds(glowInterval - glowFadeDuration);
                //fade out current glow while fading in next
                int nextIndex = (currentGlowIndex + 1) % 3;
                StartCoroutine(FadeGlow(glows[currentGlowIndex], 1f, 0f, glowFadeDuration));
                currentGlowIndex = nextIndex;
            }
        }
        IEnumerator FadeGlow(Image glow, float fromAlpha, float toAlpha, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(fromAlpha, toAlpha, elapsed / duration);
                SetGlowAlpha(glow, alpha);
                yield return null;
            }
            SetGlowAlpha(glow, toAlpha);
        }
        void SetGlowAlpha(Image glow, float alpha)
        {
            Color color = glow.color;
            color.a = alpha;
            glow.color = color;
        }
        public void FadeOut()
        {
            //stop glow animation
            if (glowCoroutine != null)
            {
                StopCoroutine(glowCoroutine);
            } 
            StartCoroutine(FadeOutAnimation());
        }
        IEnumerator FadeOutAnimation()
        {
            float elapsed = 0f;
            float duration = 0.3f;
            float startAlpha = canvasGroup.alpha;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / duration);
                yield return null;
            }
            canvasGroup.alpha = 0f;
            gameObject.SetActive(false);
        }
        public void PulseButton(int buttonIndex)
        {
            //buttonIndex: 0 = W, 1 = A , 2 = S 
            Image glowToPulse = null;
            switch (buttonIndex)
            {
                case 0:
                    glowToPulse = glow1;
                    break;
                case 1:
                    glowToPulse = glow2;
                    break;
                case 2:
                    glowToPulse = glow3;
                    break;
            }
            if (glowToPulse != null)
            {
                StartCoroutine(ButtonPulseEffect(glowToPulse));
            }
        }
        IEnumerator ButtonPulseEffect(Image glow) 
        {
            //quick bright flash
            float duration = 0.15f;
            float elapsed = 0f;
            Color originalColor = glow.color;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float intensity = Mathf.PingPong(elapsed / duration * 2f, 1f);
                glow.color = Color.Lerp(originalColor, Color.white, intensity * 0.5f);
                yield return null;
            }
            glow.color = originalColor;
        }
    }
}