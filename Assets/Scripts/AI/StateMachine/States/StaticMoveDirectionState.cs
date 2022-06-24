using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticMoveDirectionState : BaseMovementState, IState<AIController>
{
    public StaticMoveDirectionState(AIController owner) : base(owner) { }

    public void MoveInDirection()
    {
        owner.transform.Translate(owner.moveDirection * owner.moveSpeed * Time.deltaTime, Space.World);
    }

    public override void Execute()
    {
        base.Execute();
        MoveInDirection();
    }
}
