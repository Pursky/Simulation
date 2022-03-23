using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A struct that contains spawn data for grass.
/// </summary>
[System.Serializable]
public struct GrassData
{
    /// <summary>The minimal height that grass can spawn on.</summary>
    public float MinHeight;
    /// <summary>The maximal height that grass can spawn on.</summary>
    public float MaxHeight;
    /// <summary>The minimal ground angle that grass can spawn on.</summary>
    public float MinAngle;
    /// <summary>The maximal ground angle that grass can spawn on.</summary>
    public float MaxAngle;
    /// <summary>The opacity of the grass.</summary>
    public int Opacity;
    /// <summary>The change that the grass spawns.</summary>
    public float Probability;
}
