using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class Node
    {
        protected NodeState state;
        protected List<Node> children = new List<Node>();
        public Node Parent;

        private Dictionary<string, object> dataContext = new Dictionary<string, object>();

        public Node()
        {
            Parent = null;
        }

        public Node(List<Node> children)
        {
            foreach (Node child in children)
                _Attach(child);
        }

        public virtual void OnEnter() { }
        public virtual void OnExit() { }
        public virtual NodeState Evaluate() => NodeState.FAILURE;

        private void _Attach(Node node)
        {
            node.Parent = this;
            children.Add(node);
        }

        public void SetData(string key, object value)
        {
            dataContext[key] = value;
        }

        public object GetDataRecursive(string key)
        {
            object value;

            if (dataContext.TryGetValue(key, out value))
                return value;

            Node parentNode = Parent;

            while (parentNode != null)
            {
                value = parentNode.GetDataRecursive(key);
                if (value != null)
                    return value;
                parentNode = parentNode.Parent;
            }

            return null;
        }

        public bool ClearDataRecursive(string key)
        {
            if (dataContext.ContainsKey(key))
            {
                dataContext.Remove(key);
                return true;
            }

            Node parentNode = Parent;

            while (parentNode != null)
            {
                bool cleared = parentNode.ClearDataRecursive(key);

                if (cleared)
                    return true;

                parentNode = parentNode.Parent;
            }

            return false;
        }
    }

    public enum NodeState
    {
        RUNNING,
        SUCCESS,
        FAILURE
    }
}