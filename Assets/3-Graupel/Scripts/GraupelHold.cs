using UnityEngine;
using UnityEngine.UI;

namespace Graupel
{
    public class GraupelPlayer : MonoBehaviour
    {
        public Slider Slider_Main;

        public float changeAmount = 100f;

        void Start()
        {
            Slider_Main.minValue = 0f;
            Slider_Main.maxValue = .10f;
            Slider_Main.value = 0f;
        }

        void Update()
        {
            if (Input.GetKey(KeyCode.E))
            {
                Debug.Log("E Pressed");
                Slider_Main.value += changeAmount * Time.deltaTime;
            }
            else
            {
                Slider_Main.value -= changeAmount * Time.deltaTime;
            }

            Slider_Main.value = Mathf.Clamp(Slider_Main.value, 0f, .10f);
        }
    }
}




