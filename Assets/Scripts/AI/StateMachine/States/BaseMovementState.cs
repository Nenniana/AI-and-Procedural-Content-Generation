using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMovementState : IState<AIController>
{
    protected AIController owner;
    protected Vector3 moveDirection;
    public BaseMovementState(AIController owner) { this.owner = owner; }

    public virtual void Enter()
    {
    }

    public virtual void Execute()
    {
        owner.transform.rotation = Quaternion.LookRotation(Vector3.forward, owner.moveDirection);
    }

    public void ExecuteFixed()
    {
        
    }

    public void Exit()
    {
        
    }
}
