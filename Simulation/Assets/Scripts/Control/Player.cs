using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A singelton that controlls the player.
/// </summary>
public class Player : MonoBehaviour
{
    /// <summary>The static instance of the singelton.</summary>
    public static Player Instance;

    /// <summary>The speed that player rotates at.</summary>
    [Header("Movement Data")]
    public float RotationSpeed;
    /// <summary>The maximal speed that player moves at.</summary>
    public float Speed;
    /// <summary>The height that the player surfaces at.</summary>
    public float SurfacingHeight;
    /// <summary>The speed that the player drifts in the water with.</summary>
    public float SwingSpeed;
    /// <summary>The force that the player drifts in the water with.</summary>
    public float SwingForce;

    /// <summary>The maximal volume of the rotor sound effect.</summary>
    [Header("Misc")]
    public float RotorVolume;
    /// <summary>The foodchain value of the player relative to actors.</summary>
    public int FoodChain;
    /// <summary>The rotor part of the player mesh.</summary>
    public Transform Rotor;
    /// <summary>The particle system of the players rotor.</summary>
    public ParticleSystem RotorParticles;

    /// <summary>The value of the horizontal movement axis.</summary>
    private float moveX;
    /// <summary>The value of the vertical movement axis.</summary>
    private float moveY;
    /// <summary>The value of the ascention axis.</summary>
    private float ascention;
    /// <summary>The players rigidbody.</summary>
    private Rigidbody rigbod;
    /// <summary>The players audio source.</summary>
    private AudioSource source;

    /// <summary>The vector for the players drift effect.</summary>
    private Vector3 effectVector;
    /// <summary>The timer for the players drift effect.</summary>
    private float effectTimer;

    /// <summary>
    /// Initializes the singelton and sets multiple values.
    /// </summary>
    void Awake()
    {
        if (!Instance) Instance = this;
        else Destroy(this);

        rigbod = GetComponent<Rigidbody>();
        source = GetComponent<AudioSource>();
        transform.position = Vector3.up * SurfacingHeight;
    }

    /// <summary>
    /// Controlls the rotors particle effect, surface effects and audio volume.
    /// </summary>
    void Update()
    {
        if (MainUI.Instance.Paused) return;

        ParticleSystem.EmissionModule module = RotorParticles.emission;
        module.rateOverTime = rigbod.velocity.magnitude * 5;

        rigbod.drag = transform.position.y > SurfacingHeight ? 0 : 1;
        rigbod.useGravity = transform.position.y > SurfacingHeight;

        float value = rigbod.velocity.magnitude / Speed;

        source.volume = value * RotorVolume;
        source.pitch = value;

        HandleControl();
    }

    /// <summary>
    /// reads the values of the movement axes and set's the players rotation.
    /// </summary>
    private void HandleControl()
    {
        moveX = Input.GetAxis("Horizontal");
        moveY = Input.GetAxis("Vertical");
        ascention = Input.GetAxis("Ascention");

        if (moveY > 0) transform.forward += MainCamera.Instance.transform.forward * Time.deltaTime * RotationSpeed;
        if (moveY < 0) transform.forward += -MainCamera.Instance.transform.forward * Time.deltaTime * RotationSpeed;
        if (moveX > 0) transform.forward += MainCamera.Instance.transform.right * Time.deltaTime * RotationSpeed;
        if (moveX < 0) transform.forward += -MainCamera.Instance.transform.right * Time.deltaTime * RotationSpeed;
    }

    /// <summary>
    /// Handles the drift effect underwater.
    /// </summary>
    private void HandleMovementEffects()
    {
        effectTimer += Time.fixedDeltaTime;
        effectVector = new Vector3(1, 0, 1) * Mathf.Sin(effectTimer * SwingSpeed) * SwingForce;

        rigbod.velocity += effectVector;
    }

    /// <summary>
    /// Handles the players movement.
    /// </summary>
    private void FixedUpdate()
    {
        if (transform.position.y > SurfacingHeight || MainUI.Instance.Paused) return;

        Vector3 velocity = Vector3.zero;

        if (!Mathf.Approximately(moveX, 0)) velocity += transform.forward * Mathf.Sqrt(moveX * moveX);
        if (!Mathf.Approximately(moveY, 0)) velocity += transform.forward * Mathf.Sqrt(moveY * moveY);
        
        velocity += transform.up * ascention;
        if (velocity.magnitude > 1) velocity.Normalize();

        velocity *= Speed;

        if (transform.position.y > SurfacingHeight - 1 && velocity.y > 0) 
        {
            velocity.y *= Mathf.InverseLerp(SurfacingHeight, SurfacingHeight - 1, transform.position.y);
        }

        Rotor.transform.Rotate(0, 0, velocity.magnitude * Time.fixedDeltaTime * 300);

        rigbod.velocity = velocity;

        HandleMovementEffects();
    }
}