using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A script for tha camera in the main menu.
/// </summary>
public class MenuCamera : MonoBehaviour
{
    /// <summary>The center of the cameras rotation movement.</summary>
    public Vector3 BasePosition;
    /// <summary>The speed that the camera rotates at.</summary>
    public float Speed;
    /// <summary>The distance that the camera has to the base position.</summary>
    public float Distance;

    /// <summary>
    /// Rotates and moves the camera.
    /// </summary>
    void Update()
    {
        transform.eulerAngles = transform.eulerAngles + Vector3.up * Speed * Time.deltaTime;

        transform.position = BasePosition - transform.forward * Distance;
    }
}