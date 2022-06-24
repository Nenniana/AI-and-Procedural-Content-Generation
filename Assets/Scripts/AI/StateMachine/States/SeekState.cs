using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekState : AvoidState, IState<AIController>
{
    protected PNode currentTarget;

    public SeekState(AIController owner, SteeringType type) : base(owner, type) { }

    public override void Enter()
    {
        base.Enter();
        SetTarget();
    }

    public override void Execute()
    {
        base.Execute();
        AtTarget();
        SetTargetIfChanged();
    }

    private void SetTargetIfChanged()
    {
        if (currentTarget != owner.targetNode)
        {
            SetTarget();
        }
    }

    private void SetTarget()
    {
        currentTarget = owner.targetNode;
        SetPath(currentTarget);
    }

    private void AtTarget()
    {
        if (Vector3.Distance(owner.transform.position, currentTarget.Position) <= 0.4)
            owner.behaviourState.ChangeState(new PathWanderState(owner, type));
    }
}
