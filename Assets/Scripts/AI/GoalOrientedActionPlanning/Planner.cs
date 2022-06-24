using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GridSystem;

namespace Goap
{
    public class Planner : MonoBehaviour
    {
        Goal_Base[] goals;
        Action_Base[] actions;

        Goal_Base activeGoal;
        Action_Base activeAction;

        Queue<Action_Base> currentGoalQueue;

        private PathfindingController controller;
        private GridCore<PNode> grid;
        private bool isInitialized = false;

        public Goal_Base ActiveGoal 
        { 
            get 
            { 
                return activeGoal; 
            } 
            set 
            {
                if (value != null)
                {
                    if (activeGoal != null) 
                        activeGoal.OnGoalDeactivated();

                    activeGoal = value;
                    activeGoal.OnGoalActivated();
                }
            } 
        }
        public Action_Base ActiveAction
        {
            get
            {
                return activeAction;
            }
            set
            {
                if (value != null)
                {
                    if (activeAction != null)
                        activeAction.OnDeactivated();

                    activeAction = value;
                    activeAction.OnActivated(new Dictionary<string, object>());
                }
            }
        }

        private void Awake()
        {
            goals = GetComponents<Goal_Base>();
            actions = GetComponents<Action_Base>();
        }

        public void Initialize(GridCore<PNode> grid, Color color)
        {
            this.grid = grid;
            controller = new PathfindingController(grid);
            SetColor(color);

            foreach (var action in actions)
            {
                action.Initialize(this.grid, controller);
            }

            isInitialized = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (isInitialized)
                FindBestSet();
        }

        private void FindBestSet()
        {
            Goal_Base bestGoal = null;
            Action_Base bestAction = null;

            foreach (var goal in goals)
            {
                // Tick goal
                goal.OnTickGoal();

                // Can goal run?
                if (!goal.CanRun())
                    continue;

                // Is it a worse goal?
                if (!(bestGoal == null || goal.CalculatePriority() > bestGoal.CalculatePriority()))
                    continue;

                Action_Base candiateAction = null;
                foreach (var action in actions)
                {
                    if (!action.GetSupportedGoals().Where(x => x == goal.GetType()).Any())
                        continue;

                    if (candiateAction == null || action.Cost(new Dictionary<string, object>()) < action.Cost(new Dictionary<string, object>()))
                        candiateAction = action;
                }

                if (candiateAction != null)
                {
                    bestGoal = goal;
                    bestAction = candiateAction;
                }
            }

            UpdateSets(bestGoal, bestAction);

            if (ActiveAction != null)
                ActiveAction.OnTick();
        }

        private void UpdateSets(Goal_Base bestGoal, Action_Base bestAction)
        {
            if (ActiveGoal == null || ActiveGoal != bestGoal) {
                ActiveGoal = bestGoal;
                ActiveAction = bestAction;
            } 
            
            else if (ActiveGoal == bestGoal) {
                if (ActiveAction != bestAction)
                    ActiveAction = bestAction;
            }
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
    }
}