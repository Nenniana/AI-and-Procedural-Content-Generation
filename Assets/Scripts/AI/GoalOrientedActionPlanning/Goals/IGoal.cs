using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goap
{
    public interface IGoal
    {
        int CalculatePriority();

        bool CanRun();

        void OnTickGoal();

        void OnGoalActivated();

        void OnGoalDeactivated();
    }
}