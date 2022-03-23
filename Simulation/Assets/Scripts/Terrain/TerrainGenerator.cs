using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A script that generates the terrain at the beginning of the scene.
/// </summary>
public class TerrainGenerator : MonoBehaviour
{
    /// <summary>The static instance of the singelton.</summary>
    public static TerrainGenerator Instance;

    /// <summary>The Array containing all noise layers.</summary>
    [Header("Data")]
    public NoiseLayer[] NoiseLayers;
    /// <summary>The Array containing data for all textures.</summary>
    public TextureData[] TextureData;
    /// <summary>The Array containing data for all grass types.</summary>
    public GrassData[] GrassData;
    /// <summary>The Array containing data for all details.</summary>
    public DetailData[] DetailData;
    /// <summary>The Array containing data for all actors.</summary>
    public ActorData[] ActorData;
    /// <summary>The Scriptalble Object containing data modifiable in the main menu.</summary>
    public GenerateData GenerateData;

    /// <summary>The amount on fade between textures where they meet.</summary>
    [Header("Misc")]
    public float TextureFade;
    /// <summary>Whether actors don't spawn.</summary>
    public bool SpawnNoActors;


    /// <summary>The generators terrain instance.</summary>
    [HideInInspector] public Terrain Terrain;
    /// <summary>The resolution of the height map.</summary>
    [HideInInspector] public int HeightRes;
    /// <summary>The resolution of the detail map.</summary>
    [HideInInspector] public int DetailRes;
    /// <summary>The actual width and height of the terrain in ingame units.</summary>
    [HideInInspector] public float RealRes;

    /// <summary>The offset of the noise function.</summary>
    private Vector2 offset;
    /// <summary>A transform that is initially parented to all details and actors.</summary>
    private Transform objects;
    /// <summary>A transform that all actors ignoring quadrants parented to.</summary>
    private Transform ignoreQuadrants;

    /// <summary>The scenes main camera.</summary>
    private Camera cam;

    /// <summary>
    /// Initializes the singelton, sets multiple values and generates the terrain.
    /// </summary>
    void Awake()
    {
        if (!Instance) Instance = this;
        else Destroy(this);

        cam = Camera.main;

        if (GenerateData.ActorDensity == 0)
        {
            GenerateData.ActorDensity = 1;
            GenerateData.DetailDensity = 1;
            GenerateData.TerrainScale = 1;
            GenerateData.TerrainRoughness = 1;
        }

        Terrain = GetComponent<Terrain>();
        UpdateRenderDistance();

        HeightRes = Terrain.terrainData.heightmapResolution;
        DetailRes = Terrain.terrainData.detailResolution;
        RealRes = Terrain.terrainData.size.x;

        objects = transform.GetChild(0);
        ignoreQuadrants = transform.GetChild(1);

        GenerateTerrainMesh();
        GenerateTerrainContent();

        ObjectManager.Instance.Sort();
    }

    /// <summary>
    /// Generates the terrains height map based on a noise function and the noise layers.
    /// </summary>
    private void GenerateTerrainMesh()
    {
        offset = UnityEngine.Random.insideUnitCircle * 255;
        float[,] heights = new float[HeightRes, HeightRes];

        for (int y = 0; y < HeightRes; y++)
        {
            for (int x = 0; x < HeightRes; x++)
            {
                float value = 0;

                foreach(NoiseLayer layer in NoiseLayers)
                {
                    float tempValue = Mathf.PerlinNoise(x * layer.Frequency * GenerateData.TerrainScale + offset.x, y * layer.Frequency * GenerateData.TerrainScale + offset.y);
                    tempValue *= layer.Height;

                    if (!layer.IgnoreLimits) tempValue = Mathf.Clamp(tempValue, 0.25f, 0.7f);
                    else tempValue *= GenerateData.TerrainRoughness;

                    value += tempValue;
                }

                heights[x, y] = value;
            }
        }

        Terrain.terrainData.SetHeights(0, 0, heights);
    }

    /// <summary>
    /// Generates Textures, Grass, Details and then Actors based their data and the height map.
    /// </summary>
    private void GenerateTerrainContent()
    {
        int[][,] detailmap = new int[Terrain.terrainData.detailPrototypes.Length][,];
        for (int i = 0; i < detailmap.Length; i++) detailmap[i] = new int[DetailRes, DetailRes];
        
        float[,,] alphamap = new float[DetailRes, DetailRes, Terrain.terrainData.alphamapLayers];

        for (int y = 0; y < alphamap.GetLength(0); y++)
        {
            for (int x = 0; x < alphamap.GetLength(1); x++)
            {
                float interpolX = (float)x / (float)Terrain.terrainData.alphamapWidth;
                float interpolY = (float)y / (float)Terrain.terrainData.alphamapHeight;

                float height = Terrain.terrainData.GetHeight(y, x);
                float steepness = Terrain.terrainData.GetSteepness(interpolY, interpolX);

                GenerateTexture(x, y, ref alphamap, height, steepness);
                GenerateGrass(x, y, ref detailmap, height, steepness);
                GenerateDetail(x, y, height, steepness);
            }
        }

        Terrain.terrainData.SetAlphamaps(0, 0, alphamap);

        for (int i = 0; i < detailmap.Length; i++)
        {
            Terrain.terrainData.SetDetailLayer(0, 0, i, detailmap[i]);
        }

        if (SpawnNoActors) return;

        foreach (ActorData data in ActorData)
        {
            for (int i = 0; i < data.Amount * GenerateData.ActorDensity; i++)
            {
                Vector3 position = new Vector3(Random.Range(-RealRes * 0.5f, RealRes * 0.5f), 0, Random.Range(-RealRes * 0.5f, RealRes * 0.5f));
                Transform actor = GameObject.Instantiate(data.Prefab, position, Quaternion.identity).transform;
                
                if (data.IgnoreQuadrants) actor.parent = ignoreQuadrants;
                else actor.parent = objects;
            }
        }
    }

    /// <summary>
    /// Generates a texture value on the alphamap based on texture data, height and steepness.
    /// </summary>
    /// <param name="x">The current x-position on the alphamap.</param>
    /// <param name="y">The current z-position on the alphamap.</param>
    /// <param name="alphamaps">the alphamaps of the terrain.</param>
    /// <param name="height">the height at the current position<./param>
    /// <param name="steepness">the steepness at the current position<.</param>
    private void GenerateTexture(int x, int y, ref float[,,] alphamaps, float height, float steepness)
    {
        TextureData data;

        for (int i = 0; i < TextureData.Length; i++)
        {
            data = TextureData[i];

            if (height > data.MinHeight && height <= data.MaxHeight && steepness > data.MinAngle - TextureFade && steepness <= data.MaxAngle + TextureFade)
            {
                float MaxPlus = data.MaxAngle + TextureFade;
                float MaxMinus = data.MaxAngle - TextureFade;
                float MinPlus = data.MinAngle + TextureFade;
                float MinMinus = data.MinAngle - TextureFade;

                if (steepness > MinMinus && steepness < MinPlus) alphamaps[x, y, i] = Mathf.InverseLerp(MinMinus, MinPlus, steepness);
                else if (steepness > MaxMinus && steepness < MaxPlus) alphamaps[x, y, i] = Mathf.InverseLerp(MaxPlus, MaxMinus, steepness);
                else alphamaps[x, y, i] = 1;
            }
        }
    }

    /// <summary>
    /// Generates a grass value on the detailmaps based on grass data, height and steepness.
    /// </summary>
    /// <param name="x">The current x-position on the detailmap.</param>
    /// <param name="y">The current z-position on the detailmap.</param>
    /// <param name="detailmap">the detailmap of the terrain.</param>
    /// <param name="height">the height at the current position<./param>
    /// <param name="steepness">the steepness at the current position<.</param>
    private void GenerateGrass(int x, int y, ref int[][,] detailmap, float height, float steepness)
    {
        GrassData data;

        for (int i = 0; i < GrassData.Length; i++)
        {
            data = GrassData[i];

            if (height > data.MinHeight && height < data.MaxHeight && steepness > data.MinAngle && steepness < data.MaxAngle)
            {
                if (data.Probability == 1 ) detailmap[i][x, y] = data.Opacity;
                else if (Random.value < data.Probability) detailmap[i][x, y] = data.Opacity;
            }
        }
    }

    /// <summary>
    /// Generates details on the map based on detail data.
    /// </summary>
    /// <param name="x">The current x-position on the map.</param>
    /// <param name="y">The current z-position on the map.</param>
    /// <param name="height">the height at the current position<./param>
    /// <param name="steepness">the steepness at the current position<.</param>
    private void GenerateDetail(int x, int y, float height, float steepness)
    {
        DetailData data;

        for (int i = 0; i < DetailData.Length; i++)
        {
            data = DetailData[i];

            if (height > data.MinHeight && height < data.MaxHeight && steepness > data.MinAngle && steepness < data.MaxAngle)
            {
                SpawnDetail(i, x, y, data.Probability, data.Align);
            }
        }
    }

    /// <summary>
    /// Spawns a detail based on height and steepness.
    /// </summary>
    /// <param name="index">The index of the detail in the detail data array.</param>
    /// <param name="x">The current x-position on the map.</param>
    /// <param name="y">The current z-position on the map.</param>
    /// <param name="probability">The chance that the detail spawns.</param>
    /// <param name="align">Whether the detail rotates to align with the terrain.</param>
    private void SpawnDetail(int index, float x, float y, float probability, bool align)
    {
        float random = Random.value;

        if (Random.value / GenerateData.DetailDensity > probability) return;
        random = Random.value;

        GameObject detailObject = Instantiate(DetailData[index].Prefab);

        x += random - 0.5f;
        x /= (float)HeightRes;

        y += random - 0.5f;
        y /= (float)HeightRes;

        float height = Terrain.terrainData.GetInterpolatedHeight(y, x);

        Vector3 position = transform.position + new Vector3(y * RealRes, height, x * RealRes);
        if (align) detailObject.transform.up = Terrain.terrainData.GetInterpolatedNormal(y, x);

        detailObject.transform.position = position;
        detailObject.transform.localScale += Vector3.one * random;
        detailObject.transform.Rotate(Vector3.up * random * 360);

        detailObject.transform.parent = objects;
    }

    /// <summary>
    /// Returns the terrain height at the given position on the x-z-plane.
    /// </summary>
    /// <param name="position">The position on the x-z-plane</param>
    public static float GetHeight(Vector2 position)
    {
        position += Vector2.one * Instance.RealRes * 0.5f;
        position /= Instance.RealRes;
        return Instance.Terrain.terrainData.GetInterpolatedHeight(position.x, position.y) -145;
    }

    /// <summary>
    /// Returns the terrain height at the given position.
    /// </summary>
    /// <param name="position">The position on the terrain.</param>
    public static float GetHeight(Vector3 position)
    {
        Vector2 newPosition = new Vector2(position.x, position.z);
        return GetHeight(newPosition);
    }

    /// <summary>
    /// Returns the terrain normal at the given position on the x-z-plane.
    /// </summary>
    /// <param name="position">The position on the x-z-plane</param>
    public static Vector3 GetNormal(Vector2 position)
    {
        position += Vector2.one * Instance.RealRes * 0.5f;
        position /= Instance.RealRes;
        return Instance.Terrain.terrainData.GetInterpolatedNormal(position.x, position.y);
    }

    /// <summary>
    /// Returns the terrain normal at the given position.
    /// </summary>
    /// <param name="position">The position on the terrain.</param>
    public static Vector3 GetNormal(Vector3 position)
    {
        Vector2 newPosition = new Vector2(position.x, position.z);
        return GetNormal(newPosition);
    }

    /// <summary>
    /// Updates the detail object distance when the render distance was modified.
    /// </summary>
    public void UpdateRenderDistance()
    {
        Terrain.detailObjectDistance = MainCamera.Instance.RenderDistance * 1.5f;
    }
}