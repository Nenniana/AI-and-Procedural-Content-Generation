using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PathfindingState : IState<AIController>
{
    protected AIController owner;
    protected SteeringType type;
    public PathfindingState(AIController owner, SteeringType type) { this.owner = owner; this.type = type; }

    public void SetPath(PNode targetNode)
    {
        owner.gridCore.GetXY(owner.transform.position, out int xPos, out int yPos);
        owner.path = new Queue<PNode>(owner.pathfindingController.FindPath(xPos, yPos, targetNode, false, owner.distanceCostType, owner.moveType, true));
    }

    protected bool DequeueIfNeeded()
    {
        if (owner.path != null && Vector3.Distance(owner.transform.position, owner.currentTargetPosition) <= 0.4 && owner.path.Count >= 1)
        {
            owner.currentTargetPosition = owner.path.Dequeue().Position;
            return true;
        }

        return false;
    }

    public virtual void Enter()
    {
        if (type == SteeringType.Translate)
            owner.movementState.ChangeState(new StaticMoveDirectionState(owner));
        else if (type == SteeringType.SetPositionAndRotation)
            owner.movementState.ChangeState(new StaticMovePosRotState(owner));
        else
            owner.movementState.ChangeState(new StaticMoveTowardsState(owner));
    }

    public void SetMoveDirection ()
    {
        owner.moveDirection = (owner.currentTargetPosition - owner.transform.position).normalized;
    }

    public virtual void Execute()
    {
        DequeueIfNeeded();
        SetMoveDirection();
    }

    public void ExecuteFixed()
    {
        
    }

    public virtual void Exit()
    {
        
    }
}
