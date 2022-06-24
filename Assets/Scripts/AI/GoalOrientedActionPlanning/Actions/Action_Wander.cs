using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Goap
{
    public class Action_Wander : Action_Base
    {
        [SerializeField] protected float circleFovRange = 8f;

        System.Type[] supportedGoals = new System.Type[] { typeof(Goal_Wander) };

        protected PNode targetNode;
        protected PNode pathNode;
        protected Queue<PNode> nodesToTarget;
        protected Vector3 moveDirection;

        protected static int _enemyLayerMask = 1 << 7;

        public override System.Type[] GetSupportedGoals()
        {
            return supportedGoals;
        }

        public override int Cost(Dictionary<string, object> blackboard)
        {
            return 0;
        }

        public override void OnTick()
        {
            // Arrived at Destination
            if (Vector3.Distance(transform.position, targetNode.Position) <= 0.4)
                OnActivated(blackboard);

            // At node on path to location
            DequeueIfNeed();

            Move();
        }

        public override bool Perform()
        {
            if (Vector3.Distance(transform.position, targetNode.Position) <= 0.4)
                OnActivated(blackboard);

            Collider2D collider = Physics2D.OverlapCircle(transform.position, circleFovRange, _enemyLayerMask);

            if (collider != null)
            {
                blackboard.Add("target_food_gameObject", collider.gameObject);
                return true;
            }

            // At node on path to location
            DequeueIfNeed();

            Move();

            return false;
        }

        public override void OnActivated(Dictionary<string, object> blackboard)
        {
            Debug.Log("Action_Wander activated.");
            base.OnActivated(blackboard);
            targetNode = grid.GridList.Where(x => x.IsWalkable).OrderBy(x => Guid.NewGuid()).First();
            nodesToTarget = SetPath(targetNode.x, targetNode.y);
            pathNode = nodesToTarget.Dequeue();
        }

        internal Queue<PNode> SetPath(int targetX, int targetY)
        {
            grid.GetXY(transform.position, out int xPos, out int yPos);
            Queue<PNode> returnQueue = new Queue<PNode>(controller.FindPath(xPos, yPos, targetX, targetY, false, DistanceCostType.Euclidean, MovementType.AllDirections));

            return returnQueue;
        }

        internal void Move()
        {
            SetMoveDirection();
            transform.rotation = Quaternion.LookRotation(Vector3.forward, moveDirection);
            transform.Translate(moveDirection * GuardBehaviourTree.speed * Time.deltaTime, Space.World);
        }

        internal void DequeueIfNeed()
        {
            if (nodesToTarget != null && nodesToTarget.Count > 0)
                if (pathNode == null || Vector3.Distance(transform.position, pathNode.Position) <= 0.4)
                    pathNode = nodesToTarget.Dequeue();
        }

        private void SetMoveDirection()
        {
            moveDirection = (pathNode.Position - transform.position).normalized;
        }

        public override Dictionary<string, object> GetEffects()
        {
            return new Dictionary<string, object> { { "close_to_food", true } };
        }
    }
}