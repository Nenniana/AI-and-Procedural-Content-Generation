using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


namespace BehaviourTree
{
    public class TaskPatrol : Node
    {
        internal Transform _transform;
        private Vector2 moveDirection;

        private PNode[] waypoints;
        private int waypointIndex = 0;
        internal Queue<PNode> nodesToTarget = new Queue<PNode>();
        internal PNode target;

        public TaskPatrol(Transform transform)
        {
            _transform = transform;
            waypoints = GuardBehaviourTree.grid.GridList.Where(x => x.IsWalkable).OrderBy(x => Guid.NewGuid()).Take(Random.Range(2, 6)).ToArray();
        }

        public override NodeState Evaluate()
        {
            if (waypoints != null)
            {
                if (nodesToTarget == null || nodesToTarget.Count == 0)
                    nodesToTarget = SetPath(waypoints[waypointIndex].x, waypoints[waypointIndex].y);

                if (target == null || Vector3.Distance(_transform.position, target.Position) <= 0.4)
                    target = nodesToTarget.Dequeue();

                Move();

                state = NodeState.RUNNING;
                return state;
            }

            state = NodeState.FAILURE;
            return state;
        }

        internal void Move()
        {
            SetMoveDirection();
            _transform.rotation = Quaternion.LookRotation(Vector3.forward, moveDirection);
            _transform.Translate(moveDirection * GuardBehaviourTree.speed * Time.deltaTime, Space.World);
        }

        private void SetMoveDirection()
        {
            moveDirection = (target.Position - _transform.position).normalized;
        }

        internal Queue<PNode> SetPath(int targetX, int targetY)
        {
            GuardBehaviourTree.grid.GetXY(_transform.position, out int xPos, out int yPos);
            Queue<PNode> returnQueue = new Queue<PNode>(GuardBehaviourTree.controller.FindPath(xPos, yPos, targetX, targetY, false, DistanceCostType.Euclidean, MovementType.AllDirections));

            if (waypointIndex < waypoints.Length - 1)
                waypointIndex++;
            else
                waypointIndex = 0;

            return returnQueue;
        }

        public override void OnExit()
        {
            //Debug.Log("Exiting TaskPatrol.");
            nodesToTarget.Clear();
            target = null;
        }
    }
}