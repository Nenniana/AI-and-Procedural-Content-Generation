using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree {
    public class CheckEnemyInAttackRange : Node
    {
        private Transform _trasform;

        public CheckEnemyInAttackRange(Transform transform)
        {
            _trasform = transform;
        }

        public override NodeState Evaluate()
        {
            object _target = GetDataRecursive("target");

            if (_target == null)
            {
                state = NodeState.FAILURE;
                return state;
            }

            Transform target = (Transform)_target;

            if (target != null && Vector3.Distance(_trasform.position, target.position) <= GuardBehaviourTree.attackRange)
            {
                state = NodeState.SUCCESS;
                return state;
            }

            state = NodeState.FAILURE;
            return state;
        }

        public override void OnExit()
        {
            ClearDataRecursive("target");
            //Debug.Log("Entering CheckEnemyInAttackRange.");
        }
    }
}