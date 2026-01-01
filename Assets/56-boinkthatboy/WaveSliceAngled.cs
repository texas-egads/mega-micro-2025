using UnityEngine;

// !!
// parts of this code was created with ChatGPT or Gemini
// !!

namespace boinkthatboy
{
    [RequireComponent(typeof(LineRenderer))]
    public class WaveSliceAngled : MonoBehaviour
    {
        [Header("Refs")]
        public FluidTopology sim;

        [Header("Sampling")]
        [Min(8)] 
        public int points = 256;
        public float overshoot = 0.05f;

        public float heightScale = 0.35f;
        public float zOffset = 8.0f;             
        public bool allowNegativeHeights = true;  

        [Header("Motion")]
        public float travelTime = 0.45f;
        public float fadeOutTime = 0.18f;

        [Header("Trail")]
        [Range(0.01f, 1f)] 
        public float trailLength01 = 0.18f;
        [Range(0.0f, 1f)] 
        public float trailSoftness01 = 0.12f;

        [Header("Color")]
        public Color lineColor = Color.black;
        [Range(0f, 1f)] public float opacity = 0.85f;

        LineRenderer line;

        float t;
        float startDelay;
        float totalTime;

        Vector2 aW, bW;

        Gradient gradient;
        GradientColorKey[] cKeys;
        GradientAlphaKey[] aKeys;

        void Awake()
        {
            line = GetComponent<LineRenderer>();
            line.useWorldSpace = true;
            line.alignment = LineAlignment.View;
            line.numCornerVertices = 2;
            line.numCapVertices = 2;
            gradient = new Gradient();
            cKeys = new[] { new GradientColorKey(lineColor, 0f), new GradientColorKey(lineColor, 1f) };
            aKeys = new GradientAlphaKey[6];
            line.positionCount = 0;
        }

        public void Begin(
            FluidTopology simRef,
            float angleRad,
            float offset01,
            float heightScaleRef,
            float zOffsetRef,
            Color color,
            float opacityRef,
            float delaySeconds
        )
        {
            sim = simRef;
            heightScale = heightScaleRef;
            zOffset = zOffsetRef;
            lineColor = color;
            opacity = Mathf.Clamp01(opacityRef);
            startDelay = Mathf.Max(0f, delaySeconds);
            t = -startDelay;
            totalTime = travelTime + fadeOutTime;

            ComputeSegmentFromAngle(angleRad, offset01);

            line.positionCount = Mathf.Max(2, points);

            cKeys[0] = new GradientColorKey(lineColor, 0f);
            cKeys[1] = new GradientColorKey(lineColor, 1f);

            ApplyTrailGradient(0f, 0f);
            gameObject.SetActive(true);
        }

        void Update()
        {
            if (sim == null)
            {
                line.positionCount = 0;
                gameObject.SetActive( false );
                return;
            }

            t += Time.deltaTime;

            if (t < 0f)
            {
                ApplyTrailGradient( 0f, 0f );
                return;
            }

            UpdateGeometry();

            float head01 = ( travelTime <= 0f ) ? 1f : Mathf.Clamp01(t / travelTime);

            float globalFade = 1f;
            if (t > travelTime)
            {
                float ft = ( fadeOutTime <= 0f ) ? 1f : Mathf.Clamp01((t - travelTime) / fadeOutTime);
                globalFade = 1f - ft;
            }

            ApplyTrailGradient(head01, globalFade);

            if (t >= totalTime)
            {
                line.positionCount = 0;
                gameObject.SetActive(false);
            }
        }

        void UpdateGeometry()
        {
            int totalPts = Mathf.Max(2, points);
            if (line.positionCount != totalPts) line.positionCount = totalPts;

            for (int i = 0; i < totalPts; i++)
            {
                float u = i / (float)(totalPts - 1);
                Vector2 pW = Vector2.Lerp(aW, bW, u);

                float h = SampleHeightBilinearWorld(pW);

                if (!allowNegativeHeights)
                    h = Mathf.Max(0f, h);

                float z = zOffset + (h * heightScale);

                line.SetPosition(i, new Vector3(pW.x, pW.y, z));
            }
        }

        void ApplyTrailGradient(float head, float fade)
        {
            float len = Mathf.Clamp01(trailLength01);
            float soft = Mathf.Clamp01(trailSoftness01) * len;

            float tail = Mathf.Clamp01(head - len);

            float t0 = Mathf.Clamp01(tail);
            float t1 = Mathf.Clamp01(tail + soft);
            float h0 = Mathf.Clamp01(head - soft);
            float h1 = Mathf.Clamp01(head);

            if (t1 < t0) t1 = t0;
            if (h0 < t1) h0 = t1;
            if (h1 < h0) h1 = h0;

            float aMax = opacity * Mathf.Clamp01(fade);

            aKeys[0] = new GradientAlphaKey(0f, 0f);
            aKeys[1] = new GradientAlphaKey(0f, t0);
            aKeys[2] = new GradientAlphaKey(aMax, t1);
            aKeys[3] = new GradientAlphaKey(aMax, h0);
            aKeys[4] = new GradientAlphaKey(0f, h1);
            aKeys[5] = new GradientAlphaKey(0f, 1f);

            gradient.SetKeys(cKeys, aKeys);
            line.colorGradient = gradient;
        }

        float SampleHeightBilinearWorld(Vector2 world)
        {
            Bounds b = sim.GetBounds();
            int res = sim.Resolution;

            float x01 = Mathf.Clamp01(Mathf.InverseLerp(b.min.x, b.max.x, world.x));
            float y01 = Mathf.Clamp01(Mathf.InverseLerp(b.min.y, b.max.y, world.y));
            float gx = x01 * (res - 1);
            float gy = y01 * (res - 1);
            int x0 = Mathf.FloorToInt(gx);
            int y0 = Mathf.FloorToInt(gy);
            int x1 = Mathf.Min(x0 + 1, res - 1);
            int y1 = Mathf.Min(y0 + 1, res - 1);

            float tx = gx - x0;
            float ty = gy - y0;

            float h00 = sim.GetHeight(x0, y0);
            float h10 = sim.GetHeight(x1, y0);
            float h01 = sim.GetHeight(x0, y1);
            float h11 = sim.GetHeight(x1, y1);

            float hx0 = Mathf.Lerp(h00, h10, tx);
            float hx1 = Mathf.Lerp(h01, h11, tx);
            return Mathf.Lerp(hx0, hx1, ty);
        }

        void ComputeSegmentFromAngle( float angleRad, float offset )
        {
            Bounds b = sim.GetBounds();
            Vector2 center = new Vector2( b.center.x, b.center.y );

            Vector2 dir = new Vector2( Mathf.Cos( angleRad ), Mathf.Sin( angleRad ) ).normalized;
            Vector2 n = new Vector2(-dir.y, dir.x);

            float halfDiag = 0.5f * Mathf.Sqrt( b.size.x * b.size.x + b.size.y * b.size.y );
            float offsetDist = Mathf.Clamp(offset, -1f, 1f) * halfDiag;

            Vector2 shiftedCenter = center + n * offsetDist;

            float extra = overshoot * Mathf.Max(b.size.x, b.size.y);
            float halfLen = halfDiag + extra;

            aW = shiftedCenter - dir * halfLen;
            bW = shiftedCenter + dir * halfLen;
        }
    }
}