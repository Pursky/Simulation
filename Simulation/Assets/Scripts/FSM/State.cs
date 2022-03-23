using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The state of a finite state machine.
/// </summary>
public abstract class State
{
    /// <summary>The state machine, that the state belongs to.</summary>
    protected StateMachine stateMachine;

    /// <summary>
    /// The states constructor.
    /// </summary>
    /// <param name="stateMachine">the state machine that the state belongs to.</param>
    public State(StateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    /// <summary>
    /// Executes the update for the state.
    /// </summary>
    public abstract void Update();

    /// <summary>
    /// Executes the transition check for the state.
    /// </summary>
    public abstract void TransitionCheck();
}
