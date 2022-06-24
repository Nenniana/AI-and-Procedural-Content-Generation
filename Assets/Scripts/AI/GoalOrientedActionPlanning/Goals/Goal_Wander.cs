using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goap
{
    public class Goal_Wander : Goal_Base
    {
        [SerializeField] int wanderPriority = 30;

        [SerializeField] float priorityBuildRate = 1f;
        [SerializeField] float priorityDecayRate = 0.5f;

        private float currentPriority = 0f;
        private bool isMoving; // TODO SET SOMEWHERE!

        public override void OnTickGoal()
        {
            if (isMoving)
                currentPriority -= priorityDecayRate * Time.deltaTime;
            else
                currentPriority += priorityBuildRate * Time.deltaTime;

            //Debug.Log("Wander Priority: " + currentPriority);
        }

        public override void OnGoalActivated()
        {
            Debug.Log("Goal_Wander activated.");
            currentPriority = wanderPriority;
            isMoving = true;
        }

        public override void OnGoalDeactivated()
        {
            base.OnGoalDeactivated();
            currentPriority = 0;
            isMoving = false;
        }

        public override int CalculatePriority()
        {
            return Mathf.FloorToInt(currentPriority);
        }

        public override bool CanRun()
        {
            return true;
        }

    }
}