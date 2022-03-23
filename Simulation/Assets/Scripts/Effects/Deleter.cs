using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A script that deletes an object after a surtain time.
/// </summary>
public class Deleter : MonoBehaviour
{
    /// <summary>The time that the object is deleted after.</summary>
    public float DeleteTime;

    /// <summary>The system time when that start is called.</summary>
    private float startTime;

    /// <summary>
    /// Sets start time to the system time.
    /// </summary>
    private void Start()
    {
        startTime = Time.time;
    }

    /// <summary>
    /// Deletes the object after the delete time has passed.
    /// </summary>
    void Update()
    {
        if (Time.time > startTime + DeleteTime) Destroy(gameObject);
    }
}
