using UnityEngine;

// !!
// parts of this code was created with ChatGPT or Gemini
// !!

namespace boinkthatboy
{
    public class WaveEmitter : MonoBehaviour
    {
        [Range(0.0f, 5.0f)]
        public float strength = 0.85f;

        [Range(0.0f, 20.0f)]
        public float frequency = 3.2f;

        public float getForce( float time )
        {
            float pulse = Mathf.Sin(time * frequency * Mathf.PI * 2f);
            return strength * pulse;
        }
    }
}
    
