using UnityEngine;
using System.Collections.Generic;

// !!
// parts of this code was created with ChatGPT or Gemini
// !!

namespace boinkthatboy
{
    public class FluidTopology : MonoBehaviour
    {
        // settings
        [Header("Simulation")]
        [Range(256, 2048)]
        public int resolution = 1024;

        [Range(0.0f, 1.0f)]
        public float waveSpeed = 0.45f;

        [Header("Physics")]
        public float damping = 0.9995f;
        public float diffusion = 0.000f;

        [Header("Interaction")]
        [Range(0.0f, 0.5f)]
        public float forceRadius = 0.015f;

        [Range(0.5f, 10.0f)]
        public float force = 5.0f;

        [Header("Visuals")]
        [Range(0.0f, 5.0f)]
        public float density = 1.35f;

        [Range(0.0f, 0.99f)]
        public float lineThickness = 0.68f;

        [Header("Object Interaction")]
        public LayerMask interactorMask;
        [Range(1, 128)]
        public int samples = 32;
        public float forceScale = 4.0f;

        public Color32 lineColor = new Color32(0, 0, 0, 255);
        public Color32 backgroundColor = new Color32(255, 255, 255, 255);

        private float[,] currentHeight;
        private float[,] prevHeight;
        private float[,] nextHeight;

        private Texture2D texture;
        private SpriteRenderer spriteRenderer;

        private readonly List<Collider2D> overlap = new List<Collider2D>(64);
        private ContactFilter2D filter;

        private Unity.Collections.NativeArray<Color32> textureData;

        void Start()
        {
            currentHeight = new float[resolution, resolution];
            prevHeight = new float[resolution, resolution]; 
            nextHeight = new float[resolution, resolution]; 

            texture = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false);
            textureData = texture.GetRawTextureData<Color32>();
            texture.filterMode = FilterMode.Bilinear; // test trilinear maybe
            texture.wrapMode = TextureWrapMode.Clamp;

            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
                spriteRenderer = gameObject.AddComponent<SpriteRenderer>();

            spriteRenderer.enabled = true; 

            spriteRenderer.sprite = Sprite.Create(texture,
                new Rect(0, 0, resolution, resolution),
                new Vector2(0.5f, 0.5f));

            interactorMask = LayerMask.GetMask("Object 1");

            filter = new ContactFilter2D();
            filter.useLayerMask = true;
            filter.layerMask = interactorMask;
            filter.useTriggers = true;

            generateTexture();
        }

        private void generateTexture()
        {
            float scale = 0.012f;
            float offset = Random.Range(0f, 100f);

            for (int x = 0; x < resolution; x++)
            {
                for (int y = 0; y < resolution; y++)
                {
                    float heightNoise = Mathf.PerlinNoise((x * scale) + offset, (y * scale) + offset);
                    float height = (heightNoise - 0.5f) * 15.0f; // multiplier

                    // initial velocity
                    float velocityNoise = Mathf.PerlinNoise((x * scale) - offset, (y * scale) - offset);
                    float velocity = (velocityNoise - 0.5f) * 0.1f;

                    currentHeight[x, y] = height;
                    prevHeight[x, y] = height - velocity;
                    nextHeight[x, y] = height;
                }
            }
        }

        void Update()
        {
            // temporary input
            //if (Input.GetMouseButtonDown(0))
            //{
            //    inputHandle();
            //}

            applyObjectInteractions();

            simulate();
            updateTexture();
        }

        void inputHandle()
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Bounds bounds = spriteRenderer.bounds;

            if (bounds.Contains(mousePos))
            {
                float xPercent = (mousePos.x - bounds.min.x) / bounds.size.x;
                float yPercent = (mousePos.y - bounds.min.y) / bounds.size.y;

                int x = Mathf.Clamp((int)(xPercent * resolution), 0, resolution - 1);
                int y = Mathf.Clamp((int)(yPercent * resolution), 0, resolution - 1);


                deltaMomentum(x, y, force);
            }
        }

        void deltaMomentum(int xCord, int yCord, float force)
        {
            int radius = (int)(resolution * forceRadius);
            if (radius <= 0) return;

            for ( int y = -radius; y <= radius; y++ )
            {
                for ( int x = -radius; x <= radius; x++ )
                {
                    int targetX = xCord + x;
                    int targetY = yCord + y; 

                    if ( targetX > 1 && targetX < resolution - 2
                      && targetY > 1 && targetY < resolution - 2 )
                    {
                        int r2 = radius * radius;
                        int d2 = x * x + y * y;

                        if ( d2 <= r2 )
                        {
                            float distance = Mathf.Sqrt( d2 );

                            // smothing

                            float multi = distance / radius;
                            float falloff = Mathf.Exp(-multi * multi * 4.0f);

                            float impulse = force * falloff;

                            
                            prevHeight[targetX, targetY] -= impulse;

                            
                            currentHeight[targetX, targetY] += impulse * 0.5f;

                        }
                    }
                }
            }
        }

        void simulate()
        {
            float c = waveSpeed;      
            float c2 = c * c;
           
            float damp = damping;

            for (int x = 1; x < resolution - 1; x++)
            {
                for (int y = 1; y < resolution - 1; y++)
                {
                    float center = currentHeight[x, y];
                    float lap =
                        currentHeight[x - 1, y] +
                        currentHeight[x + 1, y] +
                        currentHeight[x, y - 1] +
                        currentHeight[x, y + 1] -
                        4f * center;

                    
                    float nh = (2f * center - prevHeight[x, y]) + (c2 * lap);

                    
                    if (diffusion > 0f)
                        nh = Mathf.Lerp(nh, center + 0.25f * lap, diffusion);

                    
                    nh *= damp;

                    nextHeight[x, y] = nh;
                }
            }

            // swap buffers
            var temp = prevHeight;
            prevHeight = currentHeight;
            currentHeight = nextHeight;
            nextHeight = temp;
        }


        void updateTexture()
        {
            // Unity.Collections.NativeArray<Color32> textureData = texture.GetRawTextureData<Color32>();

            for ( int i = 0; i < resolution * resolution; i++ )
            {
                int x = i % resolution;
                int y = i / resolution;

                float height = currentHeight[x, y];

                float angle = height * density;
                float sinValue = Mathf.Sin( angle );

                bool isLine = sinValue > lineThickness;

                textureData[i] = isLine ? lineColor : backgroundColor;
            }

            texture.Apply(false);
        }

        bool worldToGrid( Vector2 worldPos, out int x, out int y )
        {
            Bounds bound = spriteRenderer.bounds;

            x = y = 0;

            if ( !bound.Contains(worldPos) ) { return false;  }

            float xPercent = (worldPos.x - bound.min.x) / bound.size.x;
            float yPercent = (worldPos.y - bound.min.y) / bound.size.y;

            x = Mathf.Clamp((int)(xPercent * resolution), 0, resolution - 1);
            y = Mathf.Clamp((int)(yPercent * resolution), 0, resolution - 1);

            return true;
        }

        void emissionPoint( Collider2D c, float force )
        {
            Bounds bounds = spriteRenderer.bounds;
            Bounds cBounds = c.bounds;

            Vector2 p = cBounds.ClosestPoint(bounds.center);

            if (worldToGrid(p, out int gx, out int gy))
            {
                deltaMomentum(gx, gy, force);
            }
        }

        void applyObjectInteractions()
        {
            Bounds bound = spriteRenderer.bounds;

            filter.layerMask = interactorMask;

            overlap.Clear();

            Physics2D.OverlapBox(
                bound.center,
                bound.size,
                0f,
                filter,
                overlap
            );

            float time = Time.time;

            for (int i = 0; i < overlap.Count; i++)
            {
                Collider2D c = overlap[i];
                if (c == null) continue;

                WaveEmitter emitter = c.GetComponent<WaveEmitter>();
                if (emitter == null) continue;

                float force = emitter.getForce(time) * forceScale;

                emissionPoint( c, force );
            }
        }

        public bool TrySampleHeightWorld(Vector2 world, out float h)
        {
            h = 0f;
            Bounds b = spriteRenderer.bounds;
            if (!b.Contains(world)) return false;

            float x01 = Mathf.InverseLerp(b.min.x, b.max.x, world.x);
            float y01 = Mathf.InverseLerp(b.min.y, b.max.y, world.y);

            x01 = Mathf.Clamp01(x01);
            y01 = Mathf.Clamp01(y01);

            float gx = x01 * (resolution - 1);
            float gy = y01 * (resolution - 1);

            int x0 = Mathf.FloorToInt(gx);
            int y0 = Mathf.FloorToInt(gy);
            int x1 = Mathf.Min(x0 + 1, resolution - 1);
            int y1 = Mathf.Min(y0 + 1, resolution - 1);

            float tx = gx - x0;
            float ty = gy - y0;

            float h00 = currentHeight[x0, y0];
            float h10 = currentHeight[x1, y0];
            float h01 = currentHeight[x0, y1];
            float h11 = currentHeight[x1, y1];

            float hx0 = Mathf.Lerp(h00, h10, tx);
            float hx1 = Mathf.Lerp(h01, h11, tx);
            h = Mathf.Lerp(hx0, hx1, ty);
            return true;
        }

        public void AddImpulseWorld(Vector2 world, float force)
        {
            if (worldToGrid(world, out int gx, out int gy))
                deltaMomentum(gx, gy, force);
        }


        public Texture2D GetTexture() => texture;
        public float GetHeight(int x, int y) => currentHeight[x, y];
        public int Resolution => resolution;
        public Bounds GetBounds()
        {
            if (spriteRenderer != null)
                return spriteRenderer.bounds;

            Vector3 size = transform.lossyScale;
            return new Bounds(transform.position, size);
        }
    }
}
    
