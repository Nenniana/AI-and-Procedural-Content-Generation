using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticMovePosRotState : BaseMovementState, IState<AIController>
{
    protected float wanderStrength = 1;

    protected Vector2 position;
    protected Vector2 velocity;
    protected Vector2 desiredDirection;

    public StaticMovePosRotState(AIController owner) : base(owner) { }

    public override void Enter ()
    {
        position = owner.transform.position;
    }

    protected void Steer()
    {
        Vector2 desiredVelocity = owner.moveDirection * owner.moveSpeed;
        Vector2 desiredSteeringForce = (desiredVelocity - velocity) * owner.steeringStrength;
        Vector2 acceleration = Vector2.ClampMagnitude(desiredSteeringForce, owner.steeringStrength) / 1;

        velocity = Vector2.ClampMagnitude(velocity+ acceleration * Time.deltaTime, owner.moveSpeed);
        position += velocity * Time.deltaTime;

        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        owner.transform.SetPositionAndRotation(position, Quaternion.Euler(0, 0, angle));
    }

    public override void Execute()
    {
        base.Execute();

        if (owner.moveDirection != null)
            Steer();
    }
}
