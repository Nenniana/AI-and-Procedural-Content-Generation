using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goap
{
    public class Goal_Idle : Goal_Base
    {
        [SerializeField] int priority = 10;

        public override int CalculatePriority()
        {
            return priority;
        }

        public override bool CanRun()
        {
            return true;
        }

        public override void OnGoalActivated()
        {
            base.OnGoalActivated();
            Debug.Log("Goal_Idle activated.");
        }

        public override Dictionary<string, object> GetDesiredState()
        {
            return new Dictionary<string, object> { { "do_nothing", true } };
        }
    }
}