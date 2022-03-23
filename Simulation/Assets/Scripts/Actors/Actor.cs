using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A script for a creature that automatically moves, hunts and flees around the map.
/// </summary>
public class Actor : MonoBehaviour
{
    /// <summary>The rate that the rigidbodys velocity is changed at by steering.</summary>
    [Header("Speed Data")]
    public float Acceleration;
    /// <summary>The maximal speed that the rigidbody can have.</summary>
    public float MaxSpeed;
    /// <summary>The speed of the meshes animation relative to the rigidbodys velocity.</summary>
    public float AnimationSpeedMult;

    /// <summary>The distance at which a new point for pathing is generated.</summary>
    [Header("Pathing Data")]
    public float TargetRadius;
    /// <summary>The minimal distance the actor is supposed to have to the ground.</summary>
    public float GroundDistance;
    /// <summary>The minimal distance the actor has to have to its target in order to calculate a new one.</summary>
    public float ClosingDistance;
    /// <summary>The minimal and maximal y-position that the actor can raom in-between of.</summary>
    public Vector2 RoamingBounds;
    /// <summary>The minimal and maximal y-position that the ground has to have for the actor to be above.</summary>
    public Vector2 ExistingBounds;
    /// <summary>The time after which the actor generates a new target if the current one hasen't been reached.</summary>
    public float RepathTime;

    /// <summary>The probability that the actor hunts another when spotted.</summary>
    [Header("Hunt/Flee Data")]
    public float HuntingChance;
    /// <summary>Whether the actor is peaceful or not.</summary>
    public bool Peaceful;
    /// <summary>The value determining whether actors hunt each other. The higher the value, the higher on the foodchain they are.</summary>
    public int FoodChain;
    /// <summary>The distance that the actor can see ahead of itself.</summary>
    public float ViewingDistance;
    /// <summary>The radius of the view capsule.</summary>
    public float ViewingRadius;
    /// <summary>The Time between two view checks.</summary>
    public float ViewingInterval;
    /// <summary>The Distance the actor needs to have to another in order to eat it.</summary>
    public float EatingDistance;

    /// <summary>The size of the sphere that all actors in a flock move inside of.</summary>
    [Header("Misc")]
    public float FlockSize;
    /// <summary>The prefab for the particle effect that spawns when another actor was eaten.</summary>
    public GameObject EatParticlesPrefab;
    /// <summary>The Array containing all audio clips that the actor can play.</summary>
    public AudioClip[] Clips;

    /// <summary>The initial spawn position of the actor.</summary>
    [HideInInspector] public Vector3 StartPos;
    /// <summary>The position the actor is currently steering towards.</summary>
    [HideInInspector] public Vector3 CurrentTarget;
    /// <summary>The collider of another actor or the player when spotted.</summary>
    [HideInInspector] public Collider Other;
    /// <summary>The modifier for the acceleration.</summary>
    [HideInInspector] public float AccelerationMod;
    /// <summary>The modifier for the maximal speed.</summary>
    [HideInInspector] public float MaxSpeedMod;
    /// <summary>Whether the actor has just respawned.</summary>
    [HideInInspector] public bool HasRespawned;
    /// <summary>The transform that all spawned eating particles are parented to.</summary>
    [HideInInspector] public Transform ParticleDummy;
    /// <summary>The actors state machine.</summary>
    public StateMachine StateMachine;

    /// <summary>The array containing all entities in a flock, or the one entitiy if it isn't a flock.</summary>
    private Transform[] entities;
    /// <summary>The main entity of the actor.</summary>
    private Rigidbody rigbod;
    /// <summary>The array containing all animators in a flock, or the one animator if it isn't a flock.</summary>
    private Animator[] animators;
    /// <summary>The collider for the view capsule.</summary>
    private CapsuleCollider view;
    /// <summary>Whether the actor is supposed to be outside of the quadrant system.</summary>
    private bool ignoreQuadrants;
    /// <summary>The actors audio source.</summary>
    private AudioSource source;
    /// <summary>The timer that times the actors sound effects.</summary>
    private float soundTimer;

    /// <summary>The array containing random multipliers for the animation speed of every entitiy.</summary>
    private float[] AnimationSpeedMods;

    /// <summary>
    /// Sets all basic values for the actor and deletes it if the ground is not within the existing bounds.
    /// </summary>
    void Start()
    {
        entities = new Transform[transform.childCount];
        animators = new Animator[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            entities[i] = transform.GetChild(i);
            entities[i].TryGetComponent<Animator>(out animators[i]);
        }

        rigbod = entities[0].GetComponent<Rigidbody>();

        if (transform.position != Vector3.zero)
        {
            foreach(Transform entity in entities) entity.localPosition = transform.position;
            transform.position = Vector3.zero;
        }

        if (BeyondAcceptableRange(transform.GetChild(0).position))
        {
            Destroy(gameObject);
            return;
        }

        AnimationSpeedMods = new float[transform.childCount];
        for (int i = 0; i < AnimationSpeedMods.Length; i++)
        {
            AnimationSpeedMods[i] = Random.Range(0.9f, 1.1f);
        }

        entities[0].TryGetComponent<AudioSource>(out source);
        soundTimer = Random.Range(0, 12);

        view = entities[0].GetComponent<CapsuleCollider>();

        ignoreQuadrants = transform.parent.name.Equals("IgnoreQuadrants");

        view.height = ViewingDistance;
        view.radius = ViewingRadius;
        view.center = Vector3.forward * (ViewingDistance - ViewingRadius) * 0.5f * entities[0].localScale.z;

        AccelerationMod = 1;
        MaxSpeedMod = 1;

        float height = TerrainGenerator.GetHeight(entities[0].position) + GroundDistance;
        float min = height > RoamingBounds.x ? height : RoamingBounds.x;

        float randomHeight = Random.Range(min, RoamingBounds.y);
        foreach (Transform entity in entities) entity.position = new Vector3(entity.position.x, randomHeight, entity.position.z);

        if (entities.Length > 1) SpreadFlock();

        StartPos = entities[0].position;

        StateMachine = new StateMachine(this);
        StateMachine.SetInitState(new Roaming(StateMachine));

        ParticleDummy = GameObject.Find("ParticleDummy").transform;
    }


    /// <summary>
    /// Runs the state machine, sets rotation and animation speed and handles sound effects.
    /// </summary>
    void Update()
    {
        StateMachine.Update();

        for (int i = 0; i < entities.Length; i++)
        {
            if (rigbod.velocity.magnitude > 0) entities[i].forward = rigbod.velocity;
            if (animators[i]) animators[i].speed = rigbod.velocity.magnitude * AnimationSpeedMult * AnimationSpeedMods[i];
            if (entities[i].position.y > MainCamera.Instance.SurfaceHeight - 0.25f)
            {
                entities[i].position = new Vector3(entities[i].position.x, MainCamera.Instance.SurfaceHeight - 0.25f, entities[i].position.z);
            }

            if (source) source.mute = MainCamera.Instance.transform.position.y > MainCamera.Instance.SurfaceHeight;
        }

        if (Clips.Length == 0) return;

        soundTimer -= Time.deltaTime;
        if (soundTimer <= 0)
        {
            soundTimer = RandomTime();
            source.PlayOneShot(Clips[Random.Range(0, Clips.Length)]);
        }
    }

    /// <summary>
    /// Returns a random time for the sound effects.
    /// </summary>
    private float RandomTime() => Random.Range(8, 15);

    /// <summary>
    /// Returns whether a positions ground level is outside of the existing bounds.
    /// </summary>
    /// <param name="position">The position that is checked.</param>
    public bool BeyondAcceptableRange(Vector3 position)
    {
        float height = TerrainGenerator.GetHeight(position);
        return height < ExistingBounds.x || height > ExistingBounds.y;
    }

    /// <summary>
    /// Corrects the input point by clamping it between the roaming bounds and seperating it from the ground if it is to close. 
    /// </summary>
    /// <param name="point">The point that is corrected.</param>
    /// <returns></returns>
    public Vector3 CorrectPoint(Vector3 point)
    {
        float height = TerrainGenerator.GetHeight(point);
        float min = height > RoamingBounds.x ? height : RoamingBounds.x;

        point.y = Mathf.Clamp(point.y, min, RoamingBounds.y);
        if (Physics.CheckSphere(point, GroundDistance)) point += TerrainGenerator.GetNormal(point) * GroundDistance;
        
        point.y = Mathf.Clamp(point.y, min, RoamingBounds.y);

        return point;
    }

    /// <summary>
    /// Returns whether the point is in bounds of the map, on ground withing excisting range and in the quadrant that the actor is in.
    /// </summary>
    /// <param name="point">The point that is checked.</param>
    public bool PointIsInBounds(Vector3 point)
    {
        if ((point.x < getBoundsX().x || point.x > getBoundsX().y) && !ignoreQuadrants) return false;
        if ((point.z < getBoundsZ().x || point.z > getBoundsZ().y) && !ignoreQuadrants) return false;
        if (BeyondAcceptableRange(point)) return false;
        if (point.x > TerrainGenerator.Instance.RealRes * 0.5f || point.x < -TerrainGenerator.Instance.RealRes * 0.5f) return false;
        if (point.z > TerrainGenerator.Instance.RealRes * 0.5f || point.z < -TerrainGenerator.Instance.RealRes * 0.5f) return false;

        return true;
    }

    /// <summary>
    /// Returns the minimal and maximal x-position of the quadrant that the actor is in.
    /// </summary>
    public Vector2 getBoundsX() => new Vector2(transform.parent.position.x, transform.parent.position.x + ObjectManager.Instance.Step);

    /// <summary>
    /// Returns the minimal and maximal z-position of the quadrant that the actor is in.
    /// </summary>
    public Vector2 getBoundsZ() => new Vector2(transform.parent.position.z, transform.parent.position.z + ObjectManager.Instance.Step);
    
    /// <summary>
    /// Steers the actor towards a position.
    /// </summary>
    /// <param name="from">The base position for the steer.</param>
    /// <param name="to">The target position of the steer.</param>
    public void Seek(Vector3 from, Vector3 to)
    {
        Vector3 distanceVector = to - from;

        Vector3 seekVector = distanceVector.normalized * Acceleration * AccelerationMod * Time.deltaTime;
        rigbod.velocity += seekVector;

        if (rigbod.velocity.magnitude > MaxSpeed * MaxSpeedMod) rigbod.velocity -= rigbod.velocity * Time.deltaTime;
    }

    /// <summary>
    /// Returns an actor or the player if they are within viewing range.
    /// </summary>
    /// <param name="player">The player.</param>
    public Actor LookForActor(out Transform player)
    {
        player = null;
        if (Other)
        {
            if (Other.transform.parent.parent.TryGetComponent<Actor>(out Actor actor)) return actor;
            player = Other.transform.parent.parent;
        }
        return null;
    }

    /// <summary>
    /// Resets the actor to its starting state and position.
    /// </summary>
    public void Respawn()
    {
        if (entities.Length > 1) return;

        rigbod.velocity = Vector3.zero;

        Vector3 respawnPosition = StartPos;
        respawnPosition.y = TerrainGenerator.GetHeight(StartPos) - 1;

        entities[0].position = respawnPosition;
        StateMachine.CurrentState = new Roaming(StateMachine);

        HasRespawned = true;

        Invoke("ResetRespawn", 1);
    }
    
    /// <summary>
    /// Resets the respawn flag.
    /// </summary>
    private void ResetRespawn()
    {
        HasRespawned = false;
    }

    /// <summary>
    /// Sets positions for all entities in a flock.
    /// </summary>
    private void SpreadFlock()
    {
        for (int i = 0; i < entities.Length; i++)
        {
            if (i == 0) continue;
            entities[i].position += Random.insideUnitSphere * FlockSize;
            entities[i].GetComponent<FlockActor>().OffsetPosition = entities[i].position - entities[0].position;
        }
    }

    /// <summary>
    /// Draws gizmos for the target.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (StateMachine == null) return;

        Gizmos.color = Color.white;
        if (StateMachine.CurrentState is Hunting) Gizmos.color = Color.red;
        if (StateMachine.CurrentState is Fleeing) Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(CurrentTarget, 1);
    }
}