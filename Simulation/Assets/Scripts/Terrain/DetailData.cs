using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A struct that contains spawn data for details.
/// </summary>
[System.Serializable]
public struct DetailData
{
    /// <summary>The details prefab.</summary>
    public GameObject Prefab;
    /// <summary>The minimal height that the detail can spawn on.</summary>
    public float MinHeight;
    /// <summary>The maximal height that the detail can spawn on.</summary>
    public float MaxHeight;
    /// <summary>The minimal ground angle that the detail can spawn on.</summary>
    public float MinAngle;
    /// <summary>The maximal ground angle that the detail can spawn on.</summary>
    public float MaxAngle;
    /// <summary>The chance that the detail spawns.</summary>
    public float Probability;
    /// <summary>Whether the detail rotates to align with the terrain.</summary>
    public bool Align;
}
