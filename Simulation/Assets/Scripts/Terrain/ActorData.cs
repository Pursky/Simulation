using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A struct that contains spawn data for actors.
/// </summary>
[System.Serializable]
public struct ActorData
{
    /// <summary>The actors prefab.</summary>
    public GameObject Prefab;
    /// <summary>Whether the actor spawns outside of the quadrant system.</summary>
    public bool IgnoreQuadrants;
    /// <summary>The amount of actors of this type initially spawned across the map.</summary>
    public int Amount;
}