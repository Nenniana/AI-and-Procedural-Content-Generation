using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticMoveTowardsState : BaseMovementState, IState<AIController>
{
    public StaticMoveTowardsState(AIController owner) : base(owner) { }

    public override void Execute()
    {
        base.Execute();

        owner.transform.position = Vector2.MoveTowards(owner.transform.position, owner.currentTargetPosition, owner.moveSpeed * Time.deltaTime);
    }
}
