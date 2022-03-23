using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A singelton that sorts all spawned details and actors into quadrants and updates them.
/// </summary>
public class ObjectManager : MonoBehaviour
{
    /// <summary>The static instance of the singelton.</summary>
    public static ObjectManager Instance;

    /// <summary>The width and height of the quadrant grid.</summary>
    public int Resolution;

    /// <summary>The width and height of a quadrant.</summary>
    [HideInInspector] public float Step;

    /// <summary>An array containing all quadrants.</summary>
    private GameObject[,] quadrants;
    /// <summary>The distance between an edge and the center of a quadrant.</summary>
    private float offset;
    /// <summary>The maximal distance a quadrant has to have to the player on one axis in order to load.</summary>
    private float loadDistance;

    /// <summary>
    /// Initializes the singelton and instantiates the quadrants.
    /// </summary>
    private void Awake()
    {
        if (!Instance) Instance = this;
        else Destroy(this);

        quadrants = new GameObject[Resolution, Resolution];
    }

    /// <summary>
    /// Initializes the quadrants and sorts every object childed to the manager into their quadrants.
    /// </summary>
    public void Sort()
    {
        Transform[] allObjects = new Transform[transform.childCount];

        Step = TerrainGenerator.Instance.RealRes / (float)Resolution;
        loadDistance = Step * 0.5f + MainCamera.Instance.RenderDistance;
        offset = Step * 0.5f;

        Vector3 startPos = new Vector3(-1, 0, -1) * TerrainGenerator.Instance.RealRes * 0.5f;

        for (int i = 0; i < allObjects.Length; i++) allObjects[i] = transform.GetChild(i);

        for (int y = 0; y < Resolution; y++)
        {
            for (int x = 0; x < Resolution; x++)
            {
                GameObject quadrant = new GameObject();

                quadrant.name = $"Quadrant: {x}, {y}";
                quadrant.transform.parent = transform;
                quadrant.transform.position = startPos + new Vector3(x, 0, y) * Step;

                quadrants[x, y] = quadrant;
            }
        }

        foreach(Transform obj in allObjects)
        {
            Vector3 position = obj.position - startPos;
            position /= Step;

            obj.transform.parent = quadrants[(int)position.x, (int)position.z].transform;
        }
    }

    /// <summary>
    /// Loads and unloads quadrants based on distance to the player.
    /// </summary>
    void Update()
    {
        float xDif;
        float zDif;

        Vector3 playerPos = Player.Instance.transform.position;

        foreach (GameObject quadrant in quadrants)
        {
            xDif = (quadrant.transform.position.x + offset) - playerPos.x;
            zDif = (quadrant.transform.position.z + offset) - playerPos.z;
            quadrant.SetActive((xDif < loadDistance && xDif > -loadDistance) && (zDif < loadDistance && zDif > -loadDistance));
        }
    }

    /// <summary>
    /// Updates the load distance when the render distance was modified.
    /// </summary>
    public void UpdateRenderDistance()
    {
        loadDistance = Step * 0.5f + MainCamera.Instance.RenderDistance;
    }
}