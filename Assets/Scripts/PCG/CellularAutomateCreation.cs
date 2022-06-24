using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridSystem;
using System;
using System.Linq;

public static class CellularAutomateCreation
{
    public static void CreateMap(GridCore<PNode> core, int fillPercent, int smoothAmount, int neighbourCount = 4, NeighbourhoodType neighbourhoodType = NeighbourhoodType.Moore, string seed = "")
    {
        RandomFillMap(core, fillPercent, seed);

        for (int i = 0; i < smoothAmount; i++)
        {
            SmoothMap(core, neighbourhoodType, neighbourCount);
        }
    }

    private static void SmoothMap(GridCore<PNode> core, NeighbourhoodType neighbourhoodType, int neighbourCount)
    {
        foreach (var node in core.GridList)
        {
            if (node.x == 0 || node.x == core.Width - 1 || node.y == 0 || node.y == core.Height - 1)
                node.Fill = 1;
            else
            {
                int NeighbourWallCount = 0;

                if (neighbourhoodType == NeighbourhoodType.Moore)
                    NeighbourWallCount = core.GetAllNeighbours(node.x, node.y).Where(x => x.Fill == 1).Count();
                else if (neighbourhoodType == NeighbourhoodType.Neumann)
                    NeighbourWallCount = core.GetStraightNeighbours(node.x, node.y).Where(x => x.Fill == 1).Count();

                if (NeighbourWallCount > neighbourCount)
                    node.Fill = 1;
                else if (NeighbourWallCount < neighbourCount)
                    node.Fill = 0;
            }
        }
    }

    private static void RandomFillMap(GridCore<PNode> core, int fillPercent, string seedString)
    {
        if (string.IsNullOrEmpty(seedString))
            seedString = DateTime.Now.ToString();

        System.Random psuedoRandom = new System.Random(seedString.GetHashCode());
            

        foreach (var node in core.GridList)
        {
            if (node.x == 0 || node.x == core.Width - 1 || node.y == 0 || node.y == core.Height - 1)
                node.Fill = 1;
            else
                node.Fill = (psuedoRandom.Next(0, 100) < fillPercent) ? 1 : 0;
        }
    }
}

public enum NeighbourhoodType
{
    Moore,
    Neumann
}