using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeState : IState<AIController>
{
    protected AIController owner;
    protected Vector2 fleeDirection;
    public FleeState(AIController owner, Vector2 fleeDirection) { this.owner = owner; this.fleeDirection = fleeDirection; }

    public void Enter()
    {
        owner.moveDirection = fleeDirection;
        owner.movementState.ChangeState (new StaticMoveDirectionState(owner));
    }

    public void Execute()
    {

    }

    public void ExecuteFixed()
    {

    }

    public void Exit()
    {

    }
}
