using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GridSystem
{
    public interface IGridCell
    {
        public string DebugValue ();
        public void SetPosition (Vector3 position, float width, float height);
        public void UpdateCell();
    }
}