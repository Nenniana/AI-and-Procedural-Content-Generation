using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class Selector : ControlNode
    {
        public Selector() : base() { }
        public Selector(List<Node> children) : base(children) { }

        public override NodeState Evaluate()
        {
            foreach (Node child in children)
            {
                switch (child.Evaluate())
                {
                    case NodeState.RUNNING:
                        state = NodeState.RUNNING;
                        CurrentChild = child;
                        return state;
                    case NodeState.SUCCESS:
                        state = NodeState.SUCCESS;
                        CurrentChild = child;
                        return state;
                    case NodeState.FAILURE:
                        continue;
                    default:
                        continue;
                }
            }

            state = NodeState.FAILURE;
            return state;
        }
    }
}