using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goap
{
    public class Goal_Chase : Goal_Base
    {
        [SerializeField] int chasePriority = 60;
        [SerializeField] float circleFovRange = 8f;
        [SerializeField] float maxDistanceToChase = 5f;
        [SerializeField] float distanceToStopChase = 7f;

        private int currentPriority = 0;
        private static int _enemyLayerMask = 1 << 7;
        private Transform _target;

        public Transform Target { get => _target; }

        public override void OnGoalActivated()
        {
            base.OnGoalActivated();
            Debug.Log("Goal_Chase activated.");
        }

        public override void OnTickGoal()
        {
            currentPriority = 0;

            Collider2D[] colliders = GetCollidersInRange();

            if (colliders.Length == 0)
                return;

            if (_target != null)
                currentPriority = Vector3.Distance(transform.position, _target.transform.position) > distanceToStopChase ? 0 : chasePriority;

            foreach (Collider2D enemy in colliders)
            {
                if (Vector3.Distance(transform.position, enemy.transform.position) <= maxDistanceToChase)
                {
                    _target = enemy.transform;
                    currentPriority = chasePriority;
                    return;
                }
            }
        }

        public override void OnGoalDeactivated()
        {
            Debug.Log("Goal_Chase deactivated.");
            _target = null;
        }

        public override int CalculatePriority()
        {
            return currentPriority;
        }

        public override bool CanRun()
        {
            Collider2D[] colliders = GetCollidersInRange();

            if (colliders.Length == 0)
                return false;

            foreach (Collider2D enemy in colliders)
            {
                if (Vector3.Distance(transform.position, enemy.transform.position) <= maxDistanceToChase)
                    return true;
            }

            return false;
        }

        private Collider2D[] GetCollidersInRange()
        {
            return Physics2D.OverlapCircleAll(transform.position, circleFovRange, _enemyLayerMask);
        }

        public override Dictionary<string, object> GetDesiredState()
        {
            return new Dictionary<string, object> { { "find_enemy", true } };
        }
    }
}