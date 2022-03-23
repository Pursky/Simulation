using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A script for a creature that exclusively exists as part of a flock.
/// </summary>
public class FlockActor : MonoBehaviour
{
    /// <summary>The flock actors position relative to the main entity.</summary>
    [HideInInspector] public Vector3 OffsetPosition;

    /// <summary>The actor the entity belongs to.</summary>
    private Actor actor;

    /// <summary>The main entity.</summary>
    private Transform mainEntity;
    /// <summary>The main entities rigidbody.</summary>
    private Rigidbody mainEntityRigbod;

    /// <summary>
    /// Sets references for members.
    /// </summary>
    void Start()
    {
        actor = transform.parent.GetComponent<Actor>();

        mainEntity = transform.parent.GetChild(0);
        mainEntityRigbod = mainEntity.GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Moves the flock actor towards the main entities position.
    /// </summary>
    void FixedUpdate()
    {
        Vector3 correct = (mainEntity.position + OffsetPosition) - transform.position;
        transform.position += correct * 2 * Time.fixedDeltaTime; 
    }
}