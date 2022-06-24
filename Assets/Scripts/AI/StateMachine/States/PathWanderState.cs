using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathWanderState : AvoidState, IState<AIController>
{
    public PathWanderState(AIController owner, SteeringType type) : base(owner, type) { }

    internal void GetRandomTarget()
    {
        owner.targetNode = RandomUtilities.GetRandomElements(owner.gridCore.GridList.Where(x => x.IsWalkable == true), 1)[0];
    }

    private void SetPathIfNeeded()
    {
        if (owner.path == null || owner.path.Count <= 0)
        {
            GetRandomTarget();
            SetPath(owner.targetNode);
        }
    }

    public override void Execute()
    {
        SetPathIfNeeded();

        base.Execute();
    }

}
