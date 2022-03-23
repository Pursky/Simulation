using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A finite state machine.
/// </summary>
public class StateMachine
{
    /// <summary>The object that the state machine is attached to.</summary>
    public Object Parent;
    /// <summary>The currently active state.</summary>
    public State CurrentState;

    /// <summary>
    /// The constructor of the state machine.
    /// </summary>
    /// <param name="parent">The object that the state machine is attached to.</param>
    public StateMachine(Object parent)
    {
        this.Parent = parent;
    }

    /// <summary>
    /// Sets the initial state of the state machine.
    /// </summary>
    /// <param name="initState"></param>
    public void SetInitState(State initState)
    {
        CurrentState = initState;
    }

    /// <summary>
    /// Runs the update and transition check methods of the current state.
    /// </summary>
    public void Update()
    {
        CurrentState.TransitionCheck();
        CurrentState.Update();
    }
}
