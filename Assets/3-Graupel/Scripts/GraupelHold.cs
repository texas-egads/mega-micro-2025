using UnityEngine;
using UnityEngine.UI;

namespace Graupel
{
    public class GraupelPlayer : MonoBehaviour
    {
        public RectTransform sliderRect;
        public float maxWidth = 300f;
        public float changeAmount = 100f;
        float currentWidth = 0f;

        void Update()
        {
            if (Input.GetKey(KeyCode.W))
            {
                Debug.Log("W Pressed");
                currentWidth += changeAmount * Time.deltaTime;
            }
            else
            {
                currentWidth -= changeAmount * Time.deltaTime;
            }

            currentWidth = Mathf.Clamp(currentWidth, 0f, maxWidth);
            sliderRect.sizeDelta = new Vector2(currentWidth, sliderRect.sizeDelta.y);
        }
    }
}




