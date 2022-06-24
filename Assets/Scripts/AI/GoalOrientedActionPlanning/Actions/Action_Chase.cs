using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goap
{
    public class Action_Chase : Action_Wander
    {
        System.Type[] supportedGoals = new System.Type[] { typeof(Goal_Chase) };
        Goal_Chase Goal_Chase;

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
            DequeueIfNeed();
            Move();

            if (Vector3.Distance(targetNode.Position, transform.position) <= 0.4)
                Debug.Log("Found food!");
        }

        public override bool Perform()
        {
            DequeueIfNeed();
            Move();

            if (Vector3.Distance(targetNode.Position, transform.position) <= 0.4)
                return true;

            return false;
        }

        public override bool CanRun()
        {
            if (blackboard.ContainsKey("target_food_gameObject"))
                return blackboard["target_food_gameObject"] as GameObject ? true : false; ;

            return false;
        }

        public override void OnActivated(Dictionary<string, object> blackboard)
        {
            //Debug.Log("Action_Chase activated.");
            base.OnActivated(blackboard);
            GameObject newTarget = blackboard["target_food_gameObject"] as GameObject;
            if (newTarget != null)
            {
                targetNode = grid.GetGridObject((newTarget).transform.position);
                nodesToTarget = SetPath(targetNode.x, targetNode.y);
                pathNode = nodesToTarget.Dequeue();
            }
            
        }

        public override void OnDeactivated()
        {
            base.OnDeactivated();

            Goal_Chase = null;
            targetNode = null;
            nodesToTarget = null;
        }

        public override Dictionary<string, object> GetEffects()
        {
            return new Dictionary<string, object> { { "is_hungry", false } };
        }

        public override Dictionary<string, object> GetRequirements()
        {
            return new Dictionary<string, object> { { "close_to_food", true } };
        }
    }
}