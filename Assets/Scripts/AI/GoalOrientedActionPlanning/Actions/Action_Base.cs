using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridSystem;
using System;

namespace Goap
{
    public class Action_Base : MonoBehaviour
    {
        protected PathfindingController controller;
        protected GridCore<PNode> grid;
        protected Goal_Base linkedGoal;
        protected Dictionary<string, object> blackboard;

        public Dictionary<string, object> Blackboard { get => blackboard; set => blackboard = value; }

        public void Initialize(GridCore<PNode> grid, PathfindingController controller)
        {
            this.grid = grid;
            this.controller = controller;
            blackboard = new Dictionary<string, object>();
        }

        public virtual System.Type[] GetSupportedGoals() { return null; }

        public virtual int Cost(Dictionary<string, object> _blackboard) { blackboard = _blackboard; return 0; }

        public virtual void OnActivated(Dictionary<string, object> _blackboard) { blackboard = _blackboard; }

        public virtual void OnDeactivated() { linkedGoal = null; }

        public virtual void OnTick() { }

        public virtual Dictionary<string, object> GetEffects() { return new Dictionary<string, object>(); }

        public virtual Dictionary<string, object> GetRequirements() { return new Dictionary<string, object>(); }

        public virtual bool Perform() { return true; }

        public virtual bool CanRun() { return true; }

        public virtual bool isValid() { return true; }
    }
}