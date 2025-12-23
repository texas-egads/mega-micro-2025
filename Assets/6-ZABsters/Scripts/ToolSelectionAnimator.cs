using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace ZABsters {
    public class ToolSelectionAnimator : MonoBehaviour
    {
        [Header("UI references")]
        public CanvasGroup canvasGroup;
        public Image glow1; //glow under first button
        public Image glow2; //glow under second button
        public Image glow3; //glow under third button
        
        [Header("tool sprites for fixing gift")] //isSantasWorkshop
        public GameObject teddyBear;
        public GameObject extinguisher; //right answer, first button W
        public GameObject appleCore;
        
        [Header("tool sprites for snow shoveling")]
        public GameObject shovel;
        public GameObject camera;
        public GameObject clock; //right answer, third button s
        
        //not sure if we end up using these
        [Header("tool sprites for sleigh repair")]
        public GameObject hammer;
        public GameObject guitar; //right answer, second button a
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
        
        void Start()
        {
            //check isSantaWorkshop w GameInitializer game object
            gameInitializer = FindObjectOfType<GameInitializer>();
            //have glow state set on the first button
            canvasGroup.alpha = 0f;
            SetGlowAlpha(glow1, 0f);
            SetGlowAlpha(glow2, 0f);
            SetGlowAlpha(glow3, 0f);
            SetupToolSprites();
            //canvas should start disabled, so this waits for it to be enabled
        }
        void OnEnable()
        {
            if (!isAnimating)
            {
                StartCoroutine(FadeInAnimation());
            }
        }
        void SetupToolSprites()
        {
            if (gameInitializer != null)
            {
                bool isWorkshop = gameInitializer.isSantaWorkshop;
                //workshop tools
                teddyBear.SetActive(isWorkshop);
                extinguisher.SetActive(isWorkshop);
                appleCore.SetActive(isWorkshop);
                //snow tools
                shovel.SetActive(!isWorkshop);
                camera.SetActive(!isWorkshop);
                clock.SetActive(!isWorkshop);
                //sleigh ride tools, set to always false for now
                hammer.SetActive(false);
                guitar.SetActive(false);
                dynamite.SetActive(false);
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