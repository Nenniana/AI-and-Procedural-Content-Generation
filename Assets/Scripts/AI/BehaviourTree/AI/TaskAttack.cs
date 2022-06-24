using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class TaskAttack : Node
{

    public override NodeState Evaluate()
    {
        //Debug.Log("Now In target.");

        Transform target = (Transform)GetDataRecursive("target");

        if (target != null)
            GameObject.Destroy(target.gameObject);

        state = NodeState.RUNNING;
        return state;
    }

    public override void OnExit()
    {
        ClearDataRecursive("target");
        //Debug.Log("Entering TaskAttack.");
    }
}
