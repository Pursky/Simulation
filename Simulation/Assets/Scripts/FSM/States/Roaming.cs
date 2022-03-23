using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A roaming state for an actor.
/// </summary>
public class Roaming : State
{
    /// <summary>The actor that the state belongs to.</summary>
    private Actor actor;
    /// <summary>The main entity of the actor.</summary>
    private Transform entity;

    /// <summary>The position that the actor is being steered towards.</summary>
    private Vector3 target;

    /// <summary>The timer that recalculates a new target if the current one hasn't been reached after a surtain time.</summary>
    private float repathTimer;
    /// <summary>The timer that checks for any actors or the player between surtain intervals.</summary>
    private float viewTimer;

    /// <summary>
    /// The states constructor.
    /// </summary>
    /// <param name="stateMachine">The state machine that the state belongs to.</param>
    public Roaming(StateMachine stateMachine) : base(stateMachine)
    {
        actor = (Actor)stateMachine.Parent;
        entity = actor.transform.GetChild(0);

        actor.AccelerationMod = 1;
        actor.MaxSpeedMod = 1;

        target = entity.position + entity.forward;
    }

    /// <summary>
    /// Changes the state of the actor to fleeing:
    /// If another actor or the player were spottet and:
    ///     The foodchain of the other is higher and the other isn't peaceful.
    /// Changes the state of the actor to hunting:
    /// If another actor was spottet and:
    ///     The foodchain of the other is lower, this actor isn't peaceful and a random value is lower than the hunting chance.
    /// </summary>
    public override void TransitionCheck()
    {
        if (viewTimer < Time.time)
        {
            viewTimer = Time.time + actor.ViewingInterval;
            Actor other = actor.LookForActor(out Transform player);

            if (player != null && Player.Instance.FoodChain > actor.FoodChain) new Fleeing(stateMachine, player);
            if (!other) return;
            if (other.FoodChain == actor.FoodChain) return;
            if (other.FoodChain < actor.FoodChain && actor.Peaceful) return;
            if (other.FoodChain > actor.FoodChain && other.Peaceful) return;

            if (other.FoodChain < actor.FoodChain && Random.value < actor.HuntingChance) stateMachine.CurrentState = new Hunting(stateMachine, other);
            if (other.FoodChain > actor.FoodChain) stateMachine.CurrentState = new Fleeing(stateMachine, other.transform);
        }
    }

    /// <summary>
    /// Steers the actor towards a random next position and updates the position if the actor is close enough or the repath time has passed.
    /// </summary>
    public override void Update()
    {
        repathTimer += Time.deltaTime;

        float distance = Vector3.Distance(target, entity.position);
        if (distance < actor.ClosingDistance || repathTimer > actor.RepathTime) target = CalculateNextPos();

        if (!actor.PointIsInBounds(entity.position)) target = actor.StartPos;
        
        actor.CurrentTarget = target;
        actor.Seek(entity.position, target);
    }

    /// <summary>
    /// Returns a new target position.
    /// </summary>
    private Vector3 CalculateNextPos()
    {
        Vector3 random;

        do random = Random.insideUnitSphere.normalized * actor.TargetRadius;
        while (Vector3.Angle(entity.forward, random) > 120);

        repathTimer = 0;

        return actor.CorrectPoint(entity.position + random);
    }
}
