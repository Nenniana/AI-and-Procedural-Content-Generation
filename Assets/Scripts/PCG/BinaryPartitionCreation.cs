using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridSystem;
using System;
using System.Linq;
using Random = UnityEngine.Random;

public static class BinaryPartitionCreation
{
    public static void CreateMap(GridCore<PNode> core, int minSizeX, int minSizeY, int boundsPadding = 0, bool onlyCreateFullPartitions = true, string seedString = "")
    {
        if (string.IsNullOrEmpty(seedString))
            seedString = DateTime.Now.ToString();

        System.Random psuedoRandom = new System.Random(seedString.GetHashCode());

        List<BoundsInt> partitions = CreatePartitions(core, minSizeX, minSizeY, onlyCreateFullPartitions, psuedoRandom);

        foreach (PNode node in core.GridList)
        {
            node.Fill = 1;

            foreach (BoundsInt bounds in partitions)
            {
                if (node.x >= bounds.min.x + boundsPadding && node.x <= bounds.max.x - boundsPadding && node.y >= bounds.min.y + boundsPadding && node.y <= bounds.max.y - boundsPadding)
                    node.Fill = 0;
            }
        }

        CreateCorridors(partitions, 2, core, psuedoRandom);
    }

    private static void CreateCorridors(List<BoundsInt> partitions, int corridorWidth, GridCore<PNode> core,  System.Random psuedoRandom)
    {
        Queue<BoundsInt> partitionQueue = new Queue<BoundsInt>(partitions);
        partitionQueue.OrderBy(x => psuedoRandom.Next());
        BoundsInt currentPartition = partitionQueue.Dequeue();
        PathfindingController controller = new PathfindingController(core);
        
        while(partitionQueue.Any())
        {
            partitionQueue = new Queue<BoundsInt>(partitionQueue.OrderBy(x => Vector3.Distance(x.center, currentPartition.center)));
            BoundsInt nextPartition = partitionQueue.Dequeue();

            CreatePath(core, currentPartition.center - new Vector3(core.Width / 2, core.Height / 2, 0), nextPartition.center - new Vector3(core.Width / 2, core.Height / 2, 0), controller);

            Debug.DrawLine(currentPartition.center - new Vector3(core.Width / 2, core.Height / 2, 0), nextPartition.center - new Vector3(core.Width / 2, core.Height / 2, 0), Color.green, 100f);

            currentPartition = nextPartition;
        }
    }

    private static void CreatePath(GridCore<PNode> core, Vector3 start, Vector3 end, PathfindingController controller)
    {
        core.GetXY(core.GridList.OrderBy(x => Vector3.Distance(x.Position, start)).ToArray()[0].Position, out int startX, out int startY);
        PNode endNode = core.GridList.OrderBy(x => Vector3.Distance(x.Position, end)).ToArray()[0];

        foreach (var item in controller.FindPath(startX, startY, endNode, false, DistanceCostType.Manhantten, MovementType.Straight))
        {
            item.Fill = 0;
        }
    }

    private static List<BoundsInt> CreatePartitions(GridCore<PNode> core, int minSizeX, int minSizeY, bool onlyCreateFullPartitions, System.Random psuedoRandom)
    {
        BoundsInt rootNode = new BoundsInt(new Vector3Int((int)core.OriginPosition.x - 1 + (int)(core.Width * 0.5f), (int)core.OriginPosition.y - 1 + (int)(core.Height * 0.5f), 0), 
            new Vector3Int(core.Width, core.Height, 0));

        Queue<BoundsInt> currentPartitions = new Queue<BoundsInt>();
        List<BoundsInt> finalPartitions = new List<BoundsInt>();
        currentPartitions.Enqueue(rootNode);

        while (currentPartitions.Any())
        {
            BoundsInt currentPartition = currentPartitions.Dequeue();

            if (currentPartition.size.x > minSizeX && currentPartition.size.y > minSizeY)
            {
                if (currentPartition.size.y >= minSizeY * 2 && currentPartition.size.x >= minSizeX * 2)
                {
                    int choice = psuedoRandom.Next(1);

                    if (choice > 0.5f)
                        SplitHorizontally(minSizeY, currentPartitions, currentPartition, onlyCreateFullPartitions, psuedoRandom);
                    else
                        SplitVertially(minSizeX, currentPartitions, currentPartition, onlyCreateFullPartitions, psuedoRandom);
                }

                else if (currentPartition.size.x >= minSizeX * 2)
                    SplitVertially(minSizeX, currentPartitions, currentPartition, onlyCreateFullPartitions, psuedoRandom);

                else if (currentPartition.size.y >= minSizeY * 2)
                    SplitHorizontally(minSizeY, currentPartitions, currentPartition, onlyCreateFullPartitions, psuedoRandom);

                else
                    finalPartitions.Add(currentPartition);
            }

            //else
            //    finalPartitions.Add(currentPartition);
        }

        return finalPartitions;
    }

    private static void SplitVertially(int minSizeX, Queue<BoundsInt> currentPartitions, BoundsInt currentPartition, bool onlyCreateFullPartitions, System.Random psuedoRandom)
    {
        int xSplit;

        if (onlyCreateFullPartitions)
            xSplit = psuedoRandom.Next(minSizeX, currentPartition.size.x - minSizeX);
        else
            xSplit = psuedoRandom.Next(1, currentPartition.size.x);

        currentPartitions.Enqueue(new BoundsInt(currentPartition.min, new Vector3Int(xSplit, currentPartition.size.x, currentPartition.size.z)));
        currentPartitions.Enqueue(new BoundsInt(new Vector3Int(currentPartition.min.x + xSplit, currentPartition.min.y, currentPartition.min.z),
            new Vector3Int(currentPartition.size.x - xSplit, currentPartition.size.y, currentPartition.size.z)));
    }

    private static void SplitHorizontally(int minSizeY, Queue<BoundsInt> currentPartitions, BoundsInt currentPartition, bool onlyCreateFullPartitions, System.Random psuedoRandom)
    {
        int ySplit;

        if (onlyCreateFullPartitions)
            ySplit = psuedoRandom.Next(minSizeY, currentPartition.size.y - minSizeY);
        else
            ySplit = psuedoRandom.Next(1, currentPartition.size.x);

        currentPartitions.Enqueue(new BoundsInt(currentPartition.min, new Vector3Int(currentPartition.size.x, ySplit, currentPartition.size.z)));
        currentPartitions.Enqueue(new BoundsInt(new Vector3Int(currentPartition.min.x, currentPartition.min.y + ySplit, currentPartition.min.z),
            new Vector3Int(currentPartition.size.x, currentPartition.size.y - ySplit, currentPartition.size.z)));
    }
}