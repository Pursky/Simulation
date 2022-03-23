using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A script that checks for actors and the player in an actors view field.
/// </summary>
public class ViewChecker : MonoBehaviour
{
    /// <summary>The actor that the checker belongs to.</summary>
    private Actor actor;

    /// <summary>
    /// Sets the reference for the actor.
    /// </summary>
    void Start()
    {
        actor = transform.parent.GetComponent<Actor>();
    }

    /// <summary>
    /// Checks whether any colliding objects in the actors view range are actors or the player.
    /// </summary>
    /// <param name="other">The other collider.</param>
    private void OnTriggerStay(Collider other)
    {
        if ((other.gameObject.layer != 10 && other.gameObject.layer != 8) || !actor) return;
        actor.Other = other;
    }

    /// <summary>
    /// Sets the actors other reference to zero if an actor or the player leave the view range. 
    /// </summary>
    /// <param name="other">The other collider.</param>
    private void OnTriggerExit(Collider other)
    {
        if ((other.gameObject.layer != 10 && other.gameObject.layer != 8) || !actor) return;
        actor.Other = null;
    }
}