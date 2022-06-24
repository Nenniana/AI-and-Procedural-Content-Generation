using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class Sequence : ControlNode
    {
        public Sequence() : base() { }
        public Sequence(List<Node> children) : base(children) { }

        public override NodeState Evaluate()
        {
            bool anyChildRunning = false;

            for (int i = 0; i < children.Count; i++)
            {
                switch (children[i].Evaluate())
                {
                    case NodeState.RUNNING:
                        anyChildRunning = true;
                        CurrentChild = children[i];
                        continue;
                    case NodeState.SUCCESS:
                        continue;
                    case NodeState.FAILURE:
                        state = NodeState.FAILURE;
                        CurrentChild = children[i];
                        return state;
                    default:
                        state = NodeState.SUCCESS;
                        CurrentChild = children[i];
                        return state;
                }
            }

            state = anyChildRunning ? NodeState.RUNNING : NodeState.SUCCESS;
            return state;
        }
    }
}