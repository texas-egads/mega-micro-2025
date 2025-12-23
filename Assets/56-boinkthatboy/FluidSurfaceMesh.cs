using UnityEngine;
using System.Collections;

// !!
// parts of this code was created with ChatGPT or Gemini
// !!

namespace boinkthatboy
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class FluidSurfaceMesh : MonoBehaviour
    {
        [Header("Ref")]
        public FluidTopology sim;

        [Header("Mesh Resolution")]
        [Range(16, 256)] public int meshResolution = 128;

        [Header("Height")]
        public float meshHeightScale = 0.03f;
        public float baseZ = 0f;

        [Header("Material")]
        public bool autoCreateMaterial = true;

        public bool forceUnlit = true;

        [Range(0f, 1f)] public float smoothness = 0.05f;
        [Range(0f, 1f)] public float metallic = 0.0f;

        [Header("Performance")]
        [Range(1, 6)] public int normalRecalcEveryNFrames = 1;

        Mesh mesh;
        Vector3[] verts;
        Vector2[] uvs;
        int[] tris;

        MeshFilter mf;
        MeshRenderer mr;

        int frameCounter;

        void Awake()
        {
            mf = GetComponent<MeshFilter>();
            mr = GetComponent<MeshRenderer>();
        }

        IEnumerator Start()
        {
            if (sim == null)
            {
                enabled = false;
                yield break;
            }

            yield return new WaitUntil(() =>
            {
                var tex = sim.GetTexture();
                var b = sim.GetBounds();
                return tex != null && b.size.x > 0.0001f && b.size.y > 0.0001f;
            });

            BuildMesh();
            SetupMaterial();
        }

        void SetupMaterial()
        {
            if (mr == null) mr = GetComponent<MeshRenderer>();

            // fix backface culling stupid ahhh
            Shader shader = Shader.Find("Unlit/DoubleSidedTexture");

            if (shader == null)
                shader = Shader.Find("Unlit/Texture");

            if (shader == null)
                shader = Shader.Find("Standard");

            var mat = new Material(shader);

            Texture2D tex = sim != null ? sim.GetTexture() : null;
            if (tex != null)
                mat.mainTexture = tex;

            if (mat.shader != null && mat.shader.name == "Standard")
            {
                mat.SetFloat("_Glossiness", smoothness);
                mat.SetFloat("_Metallic", metallic);
            }

            if (mat.HasProperty("_Color"))
                mat.SetColor("_Color", Color.white);

            mr.sharedMaterial = mat;
        }

        void LateUpdate()
        {
            if (sim == null || mesh == null) return;

            UpdateVerticesFromSim();
            mesh.vertices = verts;

            if (!forceUnlit)
            {
                frameCounter++;
                if (frameCounter >= normalRecalcEveryNFrames)
                {
                    frameCounter = 0;
                    mesh.RecalculateNormals();
                }
            }

            mesh.RecalculateBounds();
        }

        void BuildMesh()
        {
            mesh = new Mesh();
            mesh.name = "FluidSurfaceMesh";

            int n = Mathf.Max(2, meshResolution);
            int vCount = n * n;

            verts = new Vector3[vCount];
            uvs = new Vector2[vCount];
            tris = new int[(n - 1) * (n - 1) * 6];

            Bounds b = sim.GetBounds();

            int k = 0;
            for (int y = 0; y < n; y++)
            {
                float v = y / (float)(n - 1);
                float wy = Mathf.Lerp(b.min.y, b.max.y, v);

                for (int x = 0; x < n; x++)
                {
                    float u = x / (float)(n - 1);
                    float wx = Mathf.Lerp(b.min.x, b.max.x, u);

                    verts[k] = new Vector3(wx, wy, baseZ);
                    uvs[k] = new Vector2(u, v);
                    k++;
                }
            }

            int ti = 0;
            for (int y = 0; y < n - 1; y++)
            {
                for (int x = 0; x < n - 1; x++)
                {
                    int i0 = y * n + x;
                    int i1 = y * n + (x + 1);
                    int i2 = (y + 1) * n + x;
                    int i3 = (y + 1) * n + (x + 1);

                    tris[ti++] = i0; tris[ti++] = i2; tris[ti++] = i1;
                    tris[ti++] = i1; tris[ti++] = i2; tris[ti++] = i3;
                }
            }

            mesh.vertices = verts;
            mesh.uv = uvs;
            mesh.triangles = tris;

            if (!forceUnlit) mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            mf.sharedMesh = mesh;
        }

        void UpdateVerticesFromSim()
        {
            int n = Mathf.Max(2, meshResolution);
            int simRes = sim.Resolution;
            Bounds b = sim.GetBounds();

            int k = 0;
            for (int y = 0; y < n; y++)
            {
                float v = y / (float)(n - 1);
                float wy = Mathf.Lerp(b.min.y, b.max.y, v);

                float gy = v * (simRes - 1);
                int y0 = Mathf.FloorToInt(gy);
                int y1 = Mathf.Min(y0 + 1, simRes - 1);
                float ty = gy - y0;

                for (int x = 0; x < n; x++)
                {
                    float u = x / (float)(n - 1);
                    float wx = Mathf.Lerp(b.min.x, b.max.x, u);

                    float gx = u * (simRes - 1);
                    int x0 = Mathf.FloorToInt(gx);
                    int x1 = Mathf.Min(x0 + 1, simRes - 1);
                    float tx = gx - x0;

                    float h00 = sim.GetHeight(x0, y0);
                    float h10 = sim.GetHeight(x1, y0);
                    float h01 = sim.GetHeight(x0, y1);
                    float h11 = sim.GetHeight(x1, y1);

                    float hx0 = Mathf.Lerp(h00, h10, tx);
                    float hx1 = Mathf.Lerp(h01, h11, tx);
                    float h = Mathf.Lerp(hx0, hx1, ty);

                    verts[k] = new Vector3(wx, wy, baseZ + h * meshHeightScale);
                    k++;
                }
            }
        }
    }
}


