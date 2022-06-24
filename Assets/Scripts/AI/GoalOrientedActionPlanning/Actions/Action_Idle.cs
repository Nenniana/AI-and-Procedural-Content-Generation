using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goap
{
    public class Action_Idle : Action_Base
    {
        Type[] supportedGoals = new Type[] { typeof(Goal_Idle) };

        public override Type[] GetSupportedGoals()
        {
            return supportedGoals;
        }

        public override int Cost(Dictionary<string, object> blackboard)
        {
            return 0;
        }

        public override void OnActivated(Dictionary<string, object> blackboard)
        {
            //Debug.Log("Action_Idle activated.");
            base.OnActivated(blackboard);
        }

        public override Dictionary<string, object> GetEffects()
        {
            return new Dictionary<string, object> { { "do_nothing", true } };
        }
    }
}