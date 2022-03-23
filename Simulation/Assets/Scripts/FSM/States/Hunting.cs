using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A hunting state for an actor.
/// </summary>
public class Hunting : State
{
    /// <summary>The main entity of the actor.</summary>
    private Transform entity;
    /// <summary>The main entity of the actor that is being hunted.</summary>
    private Transform otherEntity;
    /// <summary>The actor that the state belongs to.</summary>
    private Actor actor;
    /// <summary>The actor instance of the actor that is being hunted.</summary>
    private Actor other;

    /// <summary>The position that the actor is being steered towards.</summary>
    private Vector3 target;

    /// <summary>The time that the hunt has been going for.</summary>
    private float huntingTime;

    /// <summary>
    /// The states constructor.
    /// </summary>
    /// <param name="stateMachine">The state machine that the state belongs to.</param>
    /// <param name="other">The actor that is being hunted.</param>
    public Hunting(StateMachine stateMachine, Actor other) : base(stateMachine)
    {
        actor = (Actor)stateMachine.Parent;
        entity = actor.transform.GetChild(0);
        otherEntity = other.transform.GetChild(0);
        target = otherEntity.position;

        actor.MaxSpeedMod = 2;
        actor.AccelerationMod = 3;

        this.other = other;
    }

    /// <summary>
    /// Changes the state of the actor to roaming:
    /// If the target has been reached,
    /// If another actor reached the target first.
    /// If the hunt has been going for too long.
    /// </summary>
    public override void TransitionCheck()
    {
        if (Vector3.Distance(entity.position + entity.forward * actor.EatingDistance, otherEntity.position) < 0.25f)
        {
            other.Respawn();
            stateMachine.CurrentState = new Roaming(stateMachine);
            Object.Instantiate(actor.EatParticlesPrefab, entity.position + entity.forward, Quaternion.identity).transform.parent = actor.ParticleDummy;
        }

        if (other.HasRespawned) stateMachine.CurrentState = new Roaming(stateMachine);
        if (huntingTime > 5) stateMachine.CurrentState = new Roaming(stateMachine);
    }

    /// <summary>
    /// Steers the actor towards the other actors position.
    /// </summary>
    public override void Update()
    {
        huntingTime += Time.deltaTime;

        target = otherEntity.position;
        actor.CurrentTarget = target;

        actor.Seek(entity.position + entity.forward * actor.EatingDistance, target);
    }
}