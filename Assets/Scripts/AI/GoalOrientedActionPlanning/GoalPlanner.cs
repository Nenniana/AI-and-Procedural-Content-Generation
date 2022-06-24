using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridSystem;
using TMPro;

namespace Goap
{
    public class GoalPlanner : MonoBehaviour
    {
        [SerializeField] GameObject textPrefab;

        TextMeshProUGUI text;

        private void Start()
        {
            text = Instantiate(textPrefab).GetComponent<TextMeshProUGUI>();
            text.transform.SetParent(GameObject.FindObjectOfType<Canvas>().transform);
        }

        ActionPlanner actionPlanner;
        Action_Base[] currentPlan;
        Goal_Base[] goals;
        Goal_Base activeGoal;
        Action_Base activeAction;
        Dictionary<string, object> blackboard;

        int currentPlanIndex;
        private bool isInitialized = false;
        private bool isDone;

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
                    activeAction.OnActivated(blackboard);
                }
            }
        }

        private void Awake()
        {
            goals = GetComponents<Goal_Base>();
            actionPlanner = GetComponent<ActionPlanner>();
        }

        public void Initialize(GridCore<PNode> grid, Color color)
        {
            SetColor(color);
            actionPlanner.Initialize(grid);
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

        // Update is called once per frame
        void Update()
        {
            if (isInitialized)
            {
                text.transform.position = transform.position + new Vector3(0, 1.7f);

                foreach (Goal_Base goal in goals)
                    goal.OnTickGoal();

                SetPlanBasedGoal();
            }
        }

        private void SetPlanBasedGoal()
        {
            Goal_Base goal = GetHighestPriorityGoal();

            if (ActiveGoal == null || ActiveGoal != goal)
                GenerateNewPlan(goal);
            else
                FollowPlan(currentPlan);
        }

        private void GenerateNewPlan(Goal_Base goal)
        {
            isDone = false;
            ActiveGoal = goal;
            currentPlanIndex = 0;
            blackboard = new Dictionary<string, object>
            {
                { "position", transform.position }
            };

            currentPlan = actionPlanner.GetPlan(ActiveGoal, blackboard);

            if (currentPlan != null && currentPlan.Length > 0)
                ActiveAction = currentPlan[currentPlanIndex];
        }

        private void FollowPlan(Action_Base[] plan)
        {
            if (plan == null || plan.Length == 0)
                return;

            if (!ActiveAction.CanRun())
                GenerateNewPlan(ActiveGoal);

            text.text = ActiveGoal.GetType().ToString().Replace("Goap.", "") + "\n" + ActiveAction.GetType().ToString().Replace("Goap.", "");

            isDone = ActiveAction.Perform();

            if (isDone && currentPlanIndex < plan.Length - 1)
            {
                currentPlanIndex++;
                blackboard = ActiveAction.Blackboard;
                ActiveAction = plan[currentPlanIndex];
            } 
        }

        private Goal_Base GetHighestPriorityGoal()
        {
            Goal_Base highestPriorityGoal = null;

            foreach (Goal_Base goal in goals)
                if (goal.CanRun() && (highestPriorityGoal == null || goal.CalculatePriority() > highestPriorityGoal.CalculatePriority()))
                    highestPriorityGoal = goal;

            return highestPriorityGoal;
        }
    }
}