using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridSystem;
using System;

public class AIController : MonoBehaviour
{
    [SerializeField]
    internal SteeringType steeringType = SteeringType.Kinematic;

    [SerializeField]
    internal DistanceCostType distanceCostType = DistanceCostType.Euclidean;

    [SerializeField]
    internal MovementType moveType = MovementType.AllDirections;

    [SerializeField]
    internal float moveSpeed = 5;

    [SerializeField]
    internal float steeringStrength = 2;

    [SerializeField]
    internal bool debug = true;

    [SerializeField]
    internal float rayRange = 0.2f;

    [SerializeField]
    internal bool avoidOthers = true;

    internal StateMachine<AIController> behaviourState;
    internal StateMachine<AIController> movementState;

    internal Vector2 moveDirection;
    internal Vector3 currentTargetPosition;
    internal Vector3 steeringCorrection;
    internal PNode targetNode;
    internal Queue<PNode> path;

    internal PathfindingController pathfindingController;
    internal GridCore<PNode> gridCore;

    private bool isInitialized = false;

    internal void Initialize(GridCore<PNode> gridCore, Color color)
    {
        this.gridCore = gridCore;
        currentTargetPosition = transform.position;
        SetColor(color);

        pathfindingController = new PathfindingController(this.gridCore, gameObject);
        behaviourState = new StateMachine<AIController>();
        movementState = new StateMachine<AIController>();

        behaviourState.ChangeState(new PathWanderState(this, steeringType));

        isInitialized = true;
    }

    private void SetColor(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;

        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(color, 0.0f), new GradientColorKey(color, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
        );

        GetComponent<TrailRenderer>().colorGradient = gradient;
    }

    private void Update()
    {
        if (isInitialized)
        {
            if (Input.GetKeyDown(KeyCode.W) && behaviourState.GetState() != "PathWanderState")
                behaviourState.ChangeState(new PathWanderState(this, steeringType));

            if (Input.GetMouseButtonDown(0))
            {
                Vector3 pos = MouseWorldPosition.GetMouseWorldPosition();
                targetNode = pathfindingController.GetGrid().GetGridObject(pos);
                behaviourState.ChangeState(new SeekState(this, steeringType));
            }

            behaviourState.Update();
            movementState.Update();
        }
        
    }

    private void FixedUpdate()
    {
        if (isInitialized)
        {
            behaviourState.FixedUpdate();
            movementState.FixedUpdate();
        }
    }


    private void OnDrawGizmos()
    {
        if (debug)
        {
            if (path != null && path.Count >= 1)
            {
                Vector3 lastPos = transform.position;

                foreach (PNode node in path)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(lastPos, node.Position);
                    lastPos = node.Position;
                }
            }

            //if (drawRays)
            //    DrawRays();
        }
        
    }

    /*private void DrawRays()
    {
        float angleStep = (spread / numberOfRays) * 2;
        float angle = SetFireAngle(currentTargetPosition, angleStep);

        for (int i = 0; i < numberOfRays; i++)
        {
            if (i != numberOfRays / 2)
            {
                if (i - 1 >= numberOfRays / 2)
                    Gizmos.color = Color.green;

                Gizmos.DrawRay(this.transform.position, GetRayDirection(angle));
            }

            angle += angleStep;
        }
    }*/

    internal Vector3 GetRayDirection(float angle)
    {
        float directionXPostion = transform.position.x + Mathf.Sin((angle * Mathf.PI) / 180);
        float directionYPostion = transform.position.y + Mathf.Cos((angle * Mathf.PI) / 180);

        return (new Vector3(directionXPostion, directionYPostion) - transform.position).normalized;
    }

    /*internal float SetFireAngle(Vector2 targetPosition, float spreadDistance)
    {
        float degrees = Mathf.Atan2(targetPosition.y - transform.position.y, targetPosition.x - transform.position.x) * Mathf.Rad2Deg;

        if (degrees > 0)
            degrees -= 360;
        degrees = degrees * -1;

        float angle = (degrees + 90) - (spreadDistance * (numberOfRays / 2));

        if (numberOfRays % 2 != 0)
            return angle;
        else
            return angle + spreadDistance / 2;
    }*/
}
