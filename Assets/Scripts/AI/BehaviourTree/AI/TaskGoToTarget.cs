using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class TaskGoToTarget : TaskPatrol
    {
        public TaskGoToTarget(Transform transform) : base(transform) { }

        public override NodeState Evaluate()
        {
            Transform targetTransform = (Transform)GetDataRecursive("target");
            
            if (targetTransform != null)
            {
                if (nodesToTarget.Count == 0)
                {
                    GuardBehaviourTree.grid.GetXY(targetTransform.position, out int x, out int y);
                    nodesToTarget = SetPath(x, y);
                }

                if (nodesToTarget.Count != 0)
                    if (target == null || Vector3.Distance(_transform.position, target.Position) <= 0.4)
                        target = nodesToTarget.Dequeue();

                Move();

                state = NodeState.RUNNING;
                return state;
            }
            
            state = NodeState.FAILURE;
            return state;
        }

        public override void OnExit()
        {
            Debug.Log("Exiting TaskGoToTarget.");
            ClearDataRecursive("target");
            nodesToTarget.Clear();
            target = null;
        }
    }
}