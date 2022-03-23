using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A fleeing state for an actor.
/// </summary>
public class Fleeing : State
{
    /// <summary>The main entity of the actor.</summary>
    private Transform entity;
    /// <summary>The main entity of the actor that this actor is fleeing from.</summary>
    private Transform otherEntity;
    /// <summary>The actor that the state belongs to.</summary>
    private Actor actor;
    /// <summary>The actor instance of the actor that this actor is fleeing from.</summary>
    private Actor other;
    /// <summary>Whether the actor is fleeing from the player.</summary>
    private bool fleeingFromPlayer;

    /// <summary>The position that the actor is being steered towards.</summary>
    private Vector3 target;

    /// <summary>The time that the fleeing has been going for.</summary>
    private float fleeingTime;

    /// <summary>
    /// The states constructor.
    /// </summary>
    /// <param name="stateMachine">The state machine that the state belongs to.</param>
    /// <param name="other">The actor or player that this actor is fleeing from.</param>
    public Fleeing(StateMachine stateMachine, Transform other) : base(stateMachine)
    {
        fleeingFromPlayer = other.gameObject.name.Equals("Player");

        actor = (Actor)stateMachine.Parent;
        entity = actor.transform.GetChild(0);
        otherEntity = fleeingFromPlayer ? other : other.GetChild(0);

        actor.MaxSpeedMod = 2;
        actor.AccelerationMod = 3;

        target = GetEscapePoint();

        if (!fleeingFromPlayer) this.other = other.GetComponent<Actor>();
    }

    /// <summary>
    /// Changes the state of the actor to roaming:
    /// If the fleeing has been going for too long and other is the player,
    /// If the fleeing has been going for too long and other isn't hunting anymore.
    /// </summary>
    public override void TransitionCheck()
    {
        if (fleeingFromPlayer)
        {
            if (fleeingTime > 5) stateMachine.CurrentState = new Roaming(stateMachine);
            return;
        }

        if (fleeingTime > 5 && !(other.StateMachine.CurrentState is Hunting))
        {
            stateMachine.CurrentState = new Roaming(stateMachine);
        }
    }

    /// <summary>
    /// Steers the actor towards it's target position.
    /// </summary>
    public override void Update()
    {
        fleeingTime += Time.deltaTime;

        if (Vector3.Distance(entity.position, target) < actor.ClosingDistance) target = GetEscapePoint();

        actor.CurrentTarget = target;
        actor.Seek(entity.position, target);
    }

    /// <summary>
    /// Returns a new target position away from the other actor.
    /// </summary>
    private Vector3 GetEscapePoint()
    {
        Vector3 distanceVector = entity.position - otherEntity.position;
        Vector3 random;

        do random = Random.insideUnitSphere.normalized * actor.TargetRadius;
        while (Vector3.Angle(distanceVector, random) > 45);

        Vector3 point = entity.position + distanceVector.normalized * actor.TargetRadius + random;

        point = actor.CorrectPoint(point);
        
        return point;
    }
}