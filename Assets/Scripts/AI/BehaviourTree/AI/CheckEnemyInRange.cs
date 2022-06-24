using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree {
    public class CheckEnemyInRange : Node
    {
        private static int _enemyLayerMask = 1 << 7;

        private Transform _trasform;

        public CheckEnemyInRange(Transform transform)
        {
            _trasform = transform;
        }

        public override NodeState Evaluate()
        {
            Transform _target = (Transform)GetDataRecursive("target");

            if (_target == null || Vector3.Distance(_trasform.position, _target.position) <= GuardBehaviourTree.FOVRange)
            {
                //Debug.Log("Target is null.");

                Collider2D collider = Physics2D.OverlapCircle(_trasform.position, GuardBehaviourTree.FOVRange, _enemyLayerMask);

                if (collider != null)
                {
                    Parent.Parent.SetData("target", collider.transform);

                    state = NodeState.SUCCESS;
                    return state;
                }

                state = NodeState.FAILURE;
                return state;
            }

            //Debug.Log("Target is not null.");

            state = NodeState.SUCCESS;
            return state;
        }

        public override void OnExit()
        {
            ClearDataRecursive("target");
            //Debug.Log("Entering CheckEnemyInRange.");
        }
    }
}