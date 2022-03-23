using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A scripts for a creature that moves on a set path in the main menu.
/// </summary>
public class MenuActor : MonoBehaviour
{
    /// <summary>The rate that the rigidbodys velocity is changed at by steering.</summary>
    public float Acceleration;
    /// <summary>The maximal speed that the rigidbody can have.</summary>
    public float MaxSpeed;
    /// <summary>The speed of the meshes animation relative to the rigidbodies velocity.</summary>
    public float AnimationSpeedMult;
    /// <summary>The minimal distance the menu actor has to have to its target in order to switch to the next one.</summary>
    public float ClosingDistance;

    /// <summary>The path that the menu actor follows.</summary>
    private Vector3[] path;
    /// <summary>The index of the path point, the actor is steering towards.</summary>
    private int currentIndex;
    /// <summary>The menu actors rigidbody.</summary>
    private Rigidbody rigbod;
    /// <summary>The menu actors animator.</summary>
    private Animator animator;

    /// <summary>
    /// Sets multiple values, sets the menu actors path and moves it to a random point on the path.
    /// </summary>
    void Start()
    {
        rigbod = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        path = new Vector3[transform.childCount - 2];

        for (int i = 2; i < transform.childCount; i++) path[i - 2] = transform.GetChild(i).position;
        for (int i = 2; i < transform.childCount; i++) Destroy(transform.GetChild(i).gameObject);

        int random = Random.Range(0, path.Length);

        transform.position = path[random];
        currentIndex = random;
    }

    /// <summary>
    /// Steers the actor, checks for target changes and updates the animations speed.
    /// </summary>
    void Update()
    {
        Seek(transform.position, path[currentIndex]);

        if (Vector3.Distance(transform.position, path[currentIndex]) < ClosingDistance)
        {
            currentIndex++;
            if (currentIndex > path.Length - 1) currentIndex = 0;
        }

        animator.speed = rigbod.velocity.magnitude * AnimationSpeedMult;

        if (rigbod.velocity.magnitude > 0) transform.forward = rigbod.velocity;
    }

    /// <summary>
    /// Steers the menu actor towards a position.
    /// </summary>
    /// <param name="from">The base position for the steer.</param>
    /// <param name="to">The target position of the steer.</param>
    public void Seek(Vector3 from, Vector3 to)
    {
        Vector3 distanceVector = to - from;

        Vector3 seekVector = distanceVector.normalized * Acceleration * Time.deltaTime;
        rigbod.velocity += seekVector;

        if (rigbod.velocity.magnitude > MaxSpeed) rigbod.velocity -= rigbod.velocity * Time.deltaTime;
    }

    /// <summary>
    /// Draws gizmos for the path. 
    /// </summary>
    private void OnDrawGizmos()
    {
        for (int i = 2; i < transform.childCount; i++)
        {
            Gizmos.color = Color.white;

            if (i == transform.childCount - 1) Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(0).position);
            else Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(i + 1).position);
        }
    }
}