using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridSystem;
using System;
using System.Linq;

public class PathfindingController 
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    private GridCore<PNode> gridCore;
    private List<PNode> openList;
    private List<PNode> path;

    private string greenColor = "#26FFB9";
    private string yellowColor = "#FFEB00";
    private string redColor = "#F36D86";

    private Color green;
    private Color yellow;
    private Color red;

    private MonoBehaviour monoBehaviour;

    public PathfindingController(GridCore<PNode> gridCore, GameObject gameObject)
    {
        ColorUtility.TryParseHtmlString(greenColor, out green);
        ColorUtility.TryParseHtmlString(yellowColor, out yellow);
        ColorUtility.TryParseHtmlString(redColor, out red);
        this.gridCore = gridCore;
        monoBehaviour = gameObject.GetComponent<MonoBehaviour>();
    }

    public PathfindingController(GridCore<PNode> gridCore)
    {
        ColorUtility.TryParseHtmlString(greenColor, out green);
        ColorUtility.TryParseHtmlString(yellowColor, out yellow);
        ColorUtility.TryParseHtmlString(redColor, out red);
        this.gridCore = gridCore;
    }

    public List<PNode> FindPath (int startX, int startY, int endX, int endY, bool waitStep, DistanceCostType distanceType = DistanceCostType.NormalWeighted, MovementType moveType = MovementType.AllDirections, bool aStar = true)
    {
        PNode startNode = gridCore.GetGridObject(startX, startY);
        PNode endNode = gridCore.GetGridObject(endX, endY);
        
        if (!endNode.IsWalkable)
            return null;

        openList = new List<PNode> { startNode };
        if (waitStep)
            monoBehaviour.StartCoroutine(SearchEnumerator(0.2f, startNode, endNode, distanceType, moveType, aStar));
        else
            Search(startNode, endNode, distanceType, moveType, aStar);

        return path;
    }

    public List<PNode> FindPath(int startX, int startY, PNode endNode, bool waitStep, DistanceCostType distanceType = DistanceCostType.NormalWeighted, MovementType moveType = MovementType.AllDirections, bool aStar = true)
    {
        PNode startNode = gridCore.GetGridObject(startX, startY);

        if (!endNode.IsWalkable)
            return null;

        openList = new List<PNode> { startNode };
        if (waitStep)
            monoBehaviour.StartCoroutine(SearchEnumerator(0.2f, startNode, endNode, distanceType, moveType, aStar));
        else
            Search(startNode, endNode, distanceType, moveType, aStar);

        return path;
    }

    private IEnumerator SearchEnumerator (float time, PNode startNode, PNode endNode, DistanceCostType distanceType, MovementType moveType, bool aStar)
    {
        for (int x = 0; x < gridCore.Width; x++)
        {
            for (int y = 0; y < gridCore.Height; y++)
            {
                PNode aStarNode = gridCore.GetGridObject(x, y);

                if (aStarNode.IsWalkable)
                    aStarNode.SetColor(Color.white);

                aStarNode.ResetNode();
            }
        }

        if (aStar)
            startNode.hCost = CalculateDistanceCost(startNode, endNode, distanceType);
        startNode.gCost = 0;
        startNode.CalculateFCost();

        while (openList.Any())
        {
            openList = openList.OrderBy(x => x.fCost).ToList();
            PNode currentNode = openList.First();
            currentNode.SetColor(green);

            yield return new WaitForSeconds(time);

            if (currentNode == endNode)
            {
                path = CalculatePath(endNode);
                break;
            }

            openList.Remove(currentNode);
            currentNode.visited = true;
            currentNode.SetColor(red);

            foreach (PNode neighbourNode in GetNeighboursList(currentNode, moveType).OrderBy(x => x.fCost))
            {
                RunNeighbours(neighbourNode, currentNode, endNode, distanceType, aStar);
            }
        };
    }

    private void Search(PNode startNode, PNode endNode, DistanceCostType distanceType, MovementType moveType, bool aStar)
    {
        for (int x = 0; x < gridCore.Width; x++)
        {
            for (int y = 0; y < gridCore.Height; y++)
            {
                PNode aStarNode = gridCore.GetGridObject(x, y);

                //if (aStarNode.IsWalkable)
                //    aStarNode.SetColor(Color.white);

                aStarNode.ResetNode();
            }
        }

        
        if (aStar)
            startNode.hCost = CalculateDistanceCost(startNode, endNode, distanceType);
        startNode.gCost = 0;
        startNode.CalculateFCost();

        while (openList.Count > 0)
        {
            openList = openList.OrderBy(x => x.fCost).ToList();
            PNode currentNode = openList.First();
            //currentNode.SetColor(green);

            if (currentNode == endNode)
            {
                path = CalculatePath(endNode);
                break;
            }

            openList.Remove(currentNode);
            currentNode.visited = true;
            //currentNode.SetColor(red);

            foreach (PNode neighbourNode in GetNeighboursList(currentNode, moveType).OrderBy(x => x.fCost))
            {
                RunNeighbours(neighbourNode, currentNode, endNode, distanceType, aStar);
            }
        }
    }

    private void RunNeighbours (PNode neighbourNode, PNode currentNode, PNode endNode, DistanceCostType distanceType, bool aStar)
    {
        if (neighbourNode.visited)
            return;

        if (!neighbourNode.IsWalkable)
        {
            neighbourNode.visited = true;
            return;
        }

        //neighbourNode.SetColor(yellow);

        int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode, distanceType);

        if (tentativeGCost < neighbourNode.gCost)
        {
            neighbourNode.cameFromNode = currentNode;
            
            if (aStar)
                neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode, distanceType);

            neighbourNode.gCost = tentativeGCost;
            neighbourNode.CalculateFCost();

            if (!openList.Contains(neighbourNode))
                openList.Add(neighbourNode);
        }
    }

    public GridCore<PNode> GetGrid()
    {
        return gridCore;
    }

    private List<PNode> CalculatePath(PNode endNode)
    {
        List<PNode> path = new List<PNode>();
        PNode currentNode = endNode;
        path.Add(endNode);
        //currentNode.SetColor(green);


        while (currentNode.cameFromNode != null)
        {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
            //currentNode.SetColor(green);
        }

        path.Reverse();

        return path;
    }

    private List<PNode> GetNeighboursList (PNode currentNode, MovementType moveType)
    {
        if (moveType == MovementType.Straight)
            return gridCore.GetStraightNeighbours(currentNode.x, currentNode.y);

        if (moveType == MovementType.Diagonal)
            return gridCore.GetDiagonalNeighbours(currentNode.x, currentNode.y);

        return gridCore.GetAllNeighbours(currentNode.x, currentNode.y);
    }

    private int CalculateDistanceCost(PNode a, PNode b, DistanceCostType type)
    {
        int cost = 0;

        if (type == DistanceCostType.NormalWeighted)
        {
            int xDistance = Mathf.Abs(a.x - b.x);
            int yDistance = Mathf.Abs(a.y - b.y);
            int remaining = Mathf.Abs(xDistance - yDistance);
            cost = MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
        } 
        else if (type == DistanceCostType.Manhantten)
        {
            cost = DistanceCalculationUtility.ManhattanDistance(a.x, b.x, a.y, b.y);
        } 
        else if (type == DistanceCostType.Euclidean)
        {
            cost = DistanceCalculationUtility.EuclideanDistance(a.x, b.x, a.y, b.y);
        } 
        else if (type == DistanceCostType.Chebyshev)
        {
            cost = DistanceCalculationUtility.CalculateChebyshevDistance(a.x, b.x, a.y, b.y);
        }

        return cost;
    }
}
