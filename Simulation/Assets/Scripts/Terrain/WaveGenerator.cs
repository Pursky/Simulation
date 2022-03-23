using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A singelton that generates and updates a moving wave mesh on the surface around the player.
/// </summary>
public class WaveGenerator : MonoBehaviour
{
    /// <summary>The static instance of the singelton.</summary>
    public static WaveGenerator Instance;

    /// <summary>The resolution of the mesh.</summary>
    [Range(2, 256)]
    public int Resolution = 2;
    /// <summary>The direction that the waves move.</summary>
    public Vector3 WaveMovement;
    /// <summary>The height of the waves.</summary>
    public float WaveHeight;
    /// <summary>The scale of the noise function that generates the waves.</summary>
    public float WaveFrequency;
    /// <summary>The distance that the waves are generated for.</summary>
    public float WaveRenderDistance;

    /// <summary>The mesh filter of the object.</summary>
    [HideInInspector] public MeshFilter Filter;

    /// <summary>The mesh renderer of the object.</summary>
    new private Renderer renderer;
    /// <summary>The ingame unit distance between vertecies.</summary>
    private float step;
    /// <summary>The position where the mesh starts generating.</summary>
    private Vector3 startPos;
    /// <summary>The offset of the noise function.</summary>
    private Vector3 waveOffset;
    /// <summary>The wave render distance inverted.</summary>
    private float invertWaveRenderDistance;
    /// <summary>The quads that surround the wave mesh.</summary>
    private Transform[] quads;
    /// <summary>The plane underneath the wave mesh.</summary>
    private Transform underside;

    /// <summary>
    /// Initializes the singelton, sets multiple values and generates the initial mesh.
    /// </summary>
    void Awake()
    {
        if (!Instance) Instance = this;
        else Destroy(this);

        Filter = GetComponent<MeshFilter>();
        renderer = GetComponent<Renderer>();

        startPos = Vector3.zero + new Vector3(-1, 0, -1) * 0.5f;
        step = 1 / ((float)Resolution - 1);

        invertWaveRenderDistance = 1 / WaveRenderDistance;

        quads = new Transform[4];
        for (int i = 0; i < quads.Length; i++) quads[i] = transform.GetChild(i);
        underside = transform.GetChild(4);

        UpdateRenderDistance();

        GenerateMesh();
    }

    /// <summary>
    /// Updates the position of the wave mesh, disables the mesh if the camera is underwater and updates the waves.
    /// </summary>
    private void Update()
    {
        Vector3 playerPos = Player.Instance.transform.position;
        playerPos.y = 0;
        transform.position = playerPos;

        waveOffset += WaveMovement * Time.deltaTime;

        if (MainCamera.Instance.transform.position.y < MainCamera.Instance.SurfaceHeight)
        {
            renderer.enabled = false;
            return;
        }
        else renderer.enabled = true;

        GenerateWaves();
    }

    /// <summary>
    /// Generates the initial mesh based on resolution.
    /// </summary>
    private void GenerateMesh()
    {
        Mesh mesh = new Mesh();

        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();

        for (int y = 0, i = 0; y < Resolution; y++)
        {
            for (int x = 0; x < Resolution; x++, i++)
            {
                verts.Add(startPos + new Vector3(x * step, 0, y * step));

                if (x == Resolution - 1 || y == Resolution - 1) continue;

                tris.Add(i);
                tris.Add(i + Resolution);
                tris.Add(i + Resolution + 1);

                tris.Add(i);
                tris.Add(i + Resolution + 1);
                tris.Add(i + 1);
            }
        }

        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.RecalculateNormals();

        Filter.mesh = mesh;
    }

    /// <summary>
    /// Generates waves on the mesh based on frequency, height, offset and wave render distance.
    /// </summary>
    public void GenerateWaves()
    {
        Vector3[] verts = Filter.mesh.vertices;

        for (int i = 0; i < verts.Length; i++)
        {
            Vector3 vertPos = new Vector3(verts[i].x, 0, verts[i].z);
            if (vertPos.magnitude * transform.localScale.x > WaveRenderDistance) continue;

            Vector3 position = transform.position + (vertPos * transform.localScale.x) + waveOffset;
        
            float distanceModifier = -(vertPos.magnitude * transform.localScale.x) * invertWaveRenderDistance + 1;
        
            position *= WaveFrequency;
        
            verts[i] = vertPos + Vector3.up * Mathf.PerlinNoise(position.x, position.z) * WaveHeight * distanceModifier;
        }

        Filter.mesh.SetVertices(verts);
        Filter.mesh.RecalculateNormals();
    }

    /// <summary>
    /// Updates the size of the quads and the underside when the render distance was modified.
    /// </summary>
    public void UpdateRenderDistance()
    {
        float distance = MainCamera.Instance.RenderDistance * 1.5f;
        float baseScale = distance - 15;
        float offset = baseScale * 0.5f + 15;

        quads[0].transform.localPosition = Vector3.forward * offset / 30f;
        quads[1].transform.localPosition = -Vector3.forward * offset / 30f;
        quads[2].transform.localPosition = Vector3.right * offset / 30f;
        quads[3].transform.localPosition = -Vector3.right * offset / 30f;

        quads[0].transform.localScale = new Vector3(baseScale * 2 + 30, baseScale, 1) / 30f;
        quads[1].transform.localScale = new Vector3(baseScale * 2 + 30, baseScale, 1) / 30f;
        quads[2].transform.localScale = new Vector3(baseScale, 30, 1) / 30f;
        quads[3].transform.localScale = new Vector3(baseScale, 30, 1) / 30f;

        underside.localScale = new Vector3(distance / 15f, distance / 15f, 1);
    }
}
