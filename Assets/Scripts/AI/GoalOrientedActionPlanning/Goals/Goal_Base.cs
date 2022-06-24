using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridSystem;

namespace Goap
{
    public class Goal_Base : MonoBehaviour, IGoal
    {
        protected Action_Base linkedAction;

        private void Update() { OnTickGoal(); }

        public virtual int CalculatePriority() { return -1; }

        public virtual bool CanRun() { return false; }

        public virtual void OnGoalActivated() { }

        public virtual void OnGoalDeactivated() { }

        public virtual void OnTickGoal() { }

        public virtual Dictionary<string, object> GetDesiredState() { return new Dictionary<string, object>(); }
    }
}