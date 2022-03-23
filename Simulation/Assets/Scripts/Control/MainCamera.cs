using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A singelton for tha camera in the main scene.
/// </summary>
public class MainCamera : MonoBehaviour
{
    /// <summary>The static instance of the singelton.</summary>
    public static MainCamera Instance;

    /// <summary>The speed that the camera can be rotated at.</summary>
    [Header("Movement Data")]
    public float RotationSpeed;
    /// <summary>The speed that the camera can be zoomed in and out at.</summary>
    public float ScrollSpeed;
    /// <summary>The standard distance of the camera to the player.</summary>
    public float Distance;
    
    /// <summary>The scenes main light source.</summary>
    [Header("Render Data")]
    public Light SceneLight;
    /// <summary>The fog color above the water surface.</summary>
    public Color SurfaceFogColor;
    /// <summary>The fog and light color below the water surface.</summary>
    public Color WaterFogColor;
    /// <summary>The y-position of the surface.</summary>
    public float SurfaceHeight;
    /// <summary>The far clipping plane of the camera.</summary>
    public float RenderDistance;

    /// <summary>The scenes main camera.</summary>
    private Camera cam;
    /// <summary>The current fog and light color underwater.</summary>
    private Color currentWaterFogColor;
    /// <summary>The light color above the water surface.</summary>
    private Color sceneLightColor;
    /// <summary>The audio source for the above water fx.</summary>
    private AudioSource surfaceSource;
    /// <summary>The audio source for the below water fx.</summary>
    private AudioSource underwaterSource;
    /// <summary>The current distance of the camera to the player.</summary>
    private float currentDistance;

    /// <summary>
    /// Initializes the singelton and sets multiple values. 
    /// </summary>
    void Awake()
    {
        if (!Instance) Instance = this;
        else Destroy(this);

        Cursor.lockState = CursorLockMode.Locked;
        transform.eulerAngles = new Vector3(25, 135);
        currentDistance = Distance;

        surfaceSource = GetComponent<AudioSource>();
        underwaterSource = GetComponents<AudioSource>()[1];

        cam = GetComponent<Camera>();
        cam.farClipPlane = RenderDistance;

        RenderSettings.fogEndDistance = RenderDistance;

        currentWaterFogColor = WaterFogColor;

        sceneLightColor = SceneLight.color;
    }

    /// <summary>
    /// Controlls the fog and light colors.
    /// </summary>
    void Update()
    {
        if (MainUI.Instance.Paused) return;

        currentWaterFogColor = WaterFogColor * (transform.position.y * 0.01f + 1f);

        bool isUnderwater = transform.position.y < SurfaceHeight;

        SceneLight.color = isUnderwater ? currentWaterFogColor : sceneLightColor;
        cam.clearFlags = isUnderwater ? CameraClearFlags.SolidColor : CameraClearFlags.Skybox;
        RenderSettings.fogColor = isUnderwater ? currentWaterFogColor : SurfaceFogColor;
        cam.backgroundColor = currentWaterFogColor;

        surfaceSource.mute = isUnderwater;
        underwaterSource.mute = !isUnderwater;

        RenderSettings.reflectionIntensity = (transform.position.y * 0.01f + 1f);

        HandleControl();
    }

    /// <summary>
    /// Takes the players input and handles the cameras movement.
    /// </summary>
    private void HandleControl()
    {
        float moveX = Input.GetAxis("Mouse X");
        float moveY = Input.GetAxis("Mouse Y");

        float scroll = Input.GetAxis("Mouse Scroll");

        Vector3 angles = transform.eulerAngles;
        angles += new Vector3(-moveY, moveX) * RotationSpeed;

        angles.z = 0;

        float x = angles.x;
        x = x > 180 ? x - 360 : x;
        angles.x = Mathf.Clamp(x, -80, 80);

        transform.eulerAngles = angles;

        currentDistance += scroll * Time.deltaTime * ScrollSpeed;
        currentDistance = Mathf.Clamp(currentDistance, 3, Distance * 3);

        Physics.Raycast(new Ray(Player.Instance.transform.position, -transform.forward), out RaycastHit hit, currentDistance + 1, LayerMask.GetMask("Terrain"));

        if (hit.transform) transform.position = hit.point + transform.forward;
        else transform.position = Player.Instance.transform.position - transform.forward * currentDistance;
    }

    /// <summary>
    /// Updates the render distance.
    /// </summary>
    public void UpdateRenderDistance()
    {
        cam.farClipPlane = RenderDistance;
        RenderSettings.fogEndDistance = RenderDistance;

        WaveGenerator.Instance.UpdateRenderDistance();
        TerrainGenerator.Instance.UpdateRenderDistance();
        ObjectManager.Instance.UpdateRenderDistance();
    }

    /// <summary>
    /// Updates the render distance when changed in the inspector.
    /// </summary>
    private void OnValidate()
    {
        if (cam == null) return;
        UpdateRenderDistance();
    }
}