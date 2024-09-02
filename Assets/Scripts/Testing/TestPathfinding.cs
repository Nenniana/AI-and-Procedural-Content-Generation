using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridSystem;

public class TestPathfinding : MonoBehaviour
{
    [SerializeField]
    int height = 5;
    [SerializeField]
    int width = 8;
    [SerializeField]
    float cellSizeX = 10;
    [SerializeField]
    float cellSizeY = 10;
    [SerializeField]
    Vector3 originPosition = Vector3.zero;
    [SerializeField]
    Sprite sprite;
    [SerializeField]
    Color startColor;
    [SerializeField]
    Color clickColor;

    [SerializeField]
    DistanceCostType distanceCostType;
    [SerializeField]
    MovementType moveType;
    [SerializeField]
    bool Animate = true;

    [SerializeField]
    bool aStar = true;

    PathfindingController aStarController;
    private GridCore<PNode> gridCore;

    private void Start()
    {
        gridCore = new GridCore<PNode>(width, height, cellSizeX, (GridCore<PNode> g, int x, int y, Vector3 position, float width, float height) => new PNode(g, x, y, position, width, height));
        aStarController = new PathfindingController(gridCore, gameObject);

        foreach (var node in gridCore.GridList)
        {
            node.SetUpRenderer(sprite, Color.white);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            foreach (var node in gridCore.GridList)
            {
                node.SetUpRenderer(sprite, Color.white);
            }

            Vector3 pos = MouseWorldPosition.GetMouseWorldPosition();
            aStarController.GetGrid().GetXY(pos, out int x, out int y);
            List<PNode> path = aStarController.FindPath(0, 0, x, y, Animate, distanceCostType, moveType, aStar);

            if (path != null)
            {
                int visitedCounter = 0;

                for (int i = 0; i < path.Count - 1; i++)
                {
                    Debug.DrawLine(new Vector3(path[i].x - (width * 0.5f) + (cellSizeX * 0.5f), path[i].y - (height * 0.5f) + (cellSizeY * 0.5f)), 
                        new Vector3(path[i + 1].x - (width * 0.5f) + (cellSizeX * 0.5f), path[i + 1].y - (height * 0.5f) + (cellSizeY * 0.5f)), Color.white, 100f);
                }

                foreach (var node in gridCore.GridList)
                {
                    if (node != null && node.visited)
                        visitedCounter++;
                }

                Debug.Log("Path length is: " + path.Count);
                Debug.Log("Nodes visited is: " + visitedCounter);
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector3 pos = MouseWorldPosition.GetMouseWorldPosition();
            aStarController.GetGrid().GetGridObject(pos).IsWalkable = false;
        }
    }
}
