using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridSystem;
using System;

namespace Goap
{
    public class ActionPlanner : MonoBehaviour
    {
        private PathfindingController controller;
        private GridCore<PNode> grid;


        Action_Base[] actions;

        private void Awake()
        {
            actions = GetComponents<Action_Base>();
        }

        public void Initialize(GridCore<PNode> grid)
        {
            this.grid = grid;
            controller = new PathfindingController(grid);

            foreach (var action in actions)
            {
                action.Initialize(this.grid, controller);
            }
        }

        internal Action_Base[] GetPlan(Goal_Base goal, Dictionary<string, object> blackboard)
        {
            //Debug.Log("Goal: " + goal.GetType().ToString());

            Dictionary<string, object> desiredState = new Dictionary<string, object>(goal.GetDesiredState());

            if (desiredState.Count == 0)
            {
                //Debug.Log("No desired state found, returning null action.");
                return new Action_Base[0];
            }

            return FindBestPlan(goal, desiredState, blackboard);
        }

        private Action_Base[] FindBestPlan(Goal_Base goal, Dictionary<string, object> desiredState, Dictionary<string, object> blackboard)
        {
            Plan root = new Plan(goal, desiredState, new List<Plan>());

            if (BuildPlans(root, new Dictionary<string, object>(blackboard)))
            {
                //Debug.Log("Builds plan.");
                List<PlanListEstimate> plans = TransformTreeIntoArray(root, blackboard);
                return GetCheapestPlan(plans);
            }

            return null;
        }

        private Action_Base[] GetCheapestPlan(List<PlanListEstimate> plans)
        {
            PlanListEstimate bestPlan = null;

            foreach (PlanListEstimate plan in plans)
            {
                //Debug.Log("Plan size of random plan is: " + plan.actions.Count);
                if (bestPlan == null || plan.cost < bestPlan.cost)
                    bestPlan = plan;
            }

            if (bestPlan != null)
            {
                /*Debug.Log("Current Plan:");
                foreach (Action_Base action in bestPlan.actions)
                {
                    Debug.Log(action.GetType());
                }*/
            }
            
            return bestPlan.actions.ToArray();
        }

        private List<PlanListEstimate> TransformTreeIntoArray(Plan plan, Dictionary<string, object> blackboard)
        {
            List<PlanListEstimate> plans = new List<PlanListEstimate>();
            //Debug.Log("TransformTree was run.");

            if (plan.children.Count == 0)
            {
                Action_Base action = (Action_Base)plan.action;
                plans.Add(new PlanListEstimate(action, action.Cost(blackboard)));
                return plans;
            }

            foreach (Plan child in plan.children)
            {
                foreach (var child_plan in TransformTreeIntoArray(child, blackboard))
                {
                    //Debug.Log("There's a child action by type: " + plan.action.GetType().FullName);

                    if (plan.action is Action_Base)
                    {
                        //Debug.Log("Action type found.");
                        Action_Base action = (Action_Base)plan.action;
                        child_plan.actions.Add(action);
                        child_plan.cost += action.Cost(blackboard);
                    }

                    plans.Add(child_plan);
                }
            }

            return plans;
        }

        private bool BuildPlans(Plan root, Dictionary<string, object> blackboard)
        {
            bool hasFollowUp = false;

            Dictionary<string, object> state = new Dictionary<string, object>(root.state);

            foreach (var desire in state)
                if (blackboard.ContainsKey(desire.Key) && desire.Value.Equals(blackboard[desire.Key]))
                    state.Remove(desire.Key);


            if (state.Count == 0)
                return true;

            foreach (Action_Base action in actions)
            {
                if (!action.isValid())
                {
                    //Debug.Log("Action couldn't run, discounting it.");
                    continue;
                }

                bool shouldUseEffect = false;
                var effects = action.GetEffects();
                Dictionary<string, object> desiredState = new Dictionary<string, object>(state);

                if (desiredState.Count > 0 && effects.Count > 0)
                {
                    //Debug.Log("Both state and effects were found.");

                    //foreach (var effect in effects)
                    //{
                    //    Debug.Log("Effect found is: " + effect.Key);
                    //    Debug.Log("Effect object is:" + effect.Value);
                    //}

                    foreach (var desire in root.state)
                    {
                        //Debug.Log("Desire found is: " + desire.Key);
                        //Debug.Log("Desire object is:" + desire.Value);

                        if (effects.ContainsKey(desire.Key) && desire.Value.Equals(effects[desire.Key]))
                        {
                            //Debug.Log("Desire and effect match on " + desire.Key + " and " + desire.Value);
                            desiredState.Remove(desire.Key);
                            shouldUseEffect = true;
                        }
                    }
                }

                if (shouldUseEffect)
                {
                    var requirements = action.GetRequirements();

                    foreach (var requirement in requirements)
                        desiredState[requirement.Key] = requirement.Value;

                    Plan newPlan = new Plan(action, desiredState, new List<Plan>());

                    if (desiredState.Count == 0 || BuildPlans(newPlan, new Dictionary<string, object>(blackboard)))
                    {
                        root.children.Add(newPlan);
                        hasFollowUp = true;
                    }
                }
            }

            return hasFollowUp;
        }
    }

    public class Plan
    {
        public object action;
        public Dictionary<string, object> state = new Dictionary<string, object>();
        public List<Plan> children = new List<Plan>();

        public Plan(object action, Dictionary<string, object> state, List<Plan> children)
        {
            this.action = action;
            this.state = state;
            this.children = children;
        }
    }

    public class PlanListEstimate
    {
        public List<Action_Base> actions = new List<Action_Base>();
        public int cost;

        public PlanListEstimate(Action_Base action, int startCost)
        {
            actions.Add(action);
            cost = startCost;
        }
    }
}