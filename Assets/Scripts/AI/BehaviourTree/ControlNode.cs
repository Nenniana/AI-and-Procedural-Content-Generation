using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public abstract class ControlNode : Node
    {
        public ControlNode() : base() { }
        public ControlNode(List<Node> children) : base(children) { }

        protected Node currentChild;
        protected Node CurrentChild { set { if (currentChild != value) { if (currentChild != null) currentChild.OnExit(); currentChild = value; currentChild.OnEnter(); } } }
    }
}