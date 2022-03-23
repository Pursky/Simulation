using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A struct that contains data for textures.
/// </summary>
[System.Serializable]
public struct TextureData
{
    /// <summary>The minimal height that the texture appears on.</summary>
    public float MinHeight;
    /// <summary>The maximal height that the texture appears on.</summary>
    public float MaxHeight;
    /// <summary>The minimal ground angle that the texture appears on.</summary>
    public float MinAngle;
    /// <summary>The maximal ground angle that the texture appears on.</summary>
    public float MaxAngle;
}
