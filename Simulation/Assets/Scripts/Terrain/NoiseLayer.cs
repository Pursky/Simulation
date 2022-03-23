using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A struct for a layer of noise used to generate the terrain.
/// </summary>
[System.Serializable]
public struct NoiseLayer
{
    /// <summary>The height of the layer.</summary>
    public float Height;
    /// <summary>The frequency of the noise function.</summary>
    public float Frequency;
    /// <summary>Whether the layer isn't supposed to be clamped between max and min values.</summary>
    public bool IgnoreLimits;
}