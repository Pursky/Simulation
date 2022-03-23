using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A scriptable object that contains data editable in the main menu.
/// </summary>
[CreateAssetMenu]
public class GenerateData : ScriptableObject
{
    /// <summary>The density of actors on the map.</summary>
    [Range(0.25f, 1.5f)]
    public float ActorDensity;
    /// <summary>The density of details on the map.</summary>
    [Range(0.25f, 1.5f)]
    public float DetailDensity;
    /// <summary>The size of noise the map is being generated with.</summary>
    [Range(0.5f, 2f)]
    public float TerrainScale;
    /// <summary>The magnitude of noise functions outside of the initial one.</summary>
    [Range(0.2f, 2f)]
    public float TerrainRoughness;
}
