using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidState : PathfindingState, IState<AIController>
{
    public AvoidState(AIController owner, SteeringType type) : base(owner, type) { }

    public override void Enter()
    {
        base.Enter();
        //Debug.Log("Entering AvoidState");
    }

    public override void Execute()
    {
        base.Execute();

        if (owner.avoidOthers)
            GetClosestEntities();
    }

    protected void GetClosestEntities()
    {
        //Debug.Log("This happened.");

        Collider2D[] results = new Collider2D[5];

        Physics2D.OverlapCircleNonAlloc(owner.transform.position, owner.rayRange, results);

        foreach (Collider2D c in results)
        {
            if (c != null && c.tag == owner.tag)
            {
                Vector2 rayDir = (c.transform.position - owner.transform.position);
                float distance = Vector2.Distance(c.transform.position, owner.transform.position);

                if (owner.debug)
                    Debug.DrawRay(owner.transform.position, rayDir.normalized * distance, Color.red);

                Vector2 testrayDir = Vector2.zero;
                if (rayDir.magnitude != 0)
                    testrayDir = (1 / (distance * rayDir).magnitude) * -rayDir.normalized;

                owner.moveDirection += testrayDir * 0.5f;
            }
        }
    }

    public override void Exit()
    {
        //owner.drawRays = false;
    }
}
