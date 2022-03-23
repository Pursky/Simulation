using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A script that fades the material for sea particles invisible near the surface.
/// </summary>
public class SeaParticles : MonoBehaviour
{
    /// <summary>The sea particle material.</summary>
    public Material SeaParticleMaterial;

    /// <summary>
    /// Calculates and applies the opacity for the material.
    /// </summary>
    void Update()
    {
        float alpha = Mathf.InverseLerp(MainCamera.Instance.SurfaceHeight, MainCamera.Instance.SurfaceHeight - 5, MainCamera.Instance.transform.position.y);
        SeaParticleMaterial.color = new Color(1, 1, 1, alpha);
    }
}