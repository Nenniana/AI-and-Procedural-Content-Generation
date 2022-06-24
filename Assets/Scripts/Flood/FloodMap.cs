using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridSystem;
using System.Linq;
using System;

public class FloodMap 
{
    //Func<Vector2> targetDelegate(char denotion);
    public delegate bool BooleanPositionDelegate(int x, int y);
    public delegate int IntegerPositionDelegate(int index);

    MapCoord[,] map;
    GridCore<PNode> core;

    public MapCoord[,] Map { get => map; }

    //public void Test ()
    //{
    //    Debug.Log("This was called.");
    //    MapCoord mapCoord = new MapCoord(2, 4);
    //    SetMapBasedOnSource((x, y) => map[x, y].Denotion == 4, 0);
    //    GetMap(map, (x, y) => map[x, y].Denotion == 'C', (x, y) => map[x, y].Denotion == '#', x => 1);

    //}

    public void InitializeFloodMap(GridCore<PNode> gridCore, Vector2Int position, int fillType)
    {
        core = gridCore;
        CreateFloodMap(position.x, position.y, fillType);
    }

    public void InitializeDjikstraMap(GridCore<PNode> gridCore, int startX, int startY, int fillType)
    {
        core = gridCore;
        CreateFloodMap(startX, startY, fillType);
    }

    private void CreateFloodMap(int startX, int startY, int fillType)
    {
        //Array.Clear(map, 0, map.Length);
        int[,] mapFlags = new int[core.Width, core.Height];
        map = new MapCoord[core.Width, core.Height];

        for (int i = 0; i < core.Width; i++)
        {
            for (int j = 0; j < core.Height; j++)
            {
                PNode node = core.GetGridObject(i, j);
                map[i,j] = new MapCoord(node.x, node.y);
            }
        }

        Queue<MapCoord> mapCoords = new Queue<MapCoord>();
        mapCoords.Enqueue(new MapCoord(startX, startY, '@'));
        mapFlags[startX, startY] = 1;
        core.GetGridObject(startX, startY).SetColor(Color.black);

        while(mapCoords.Any())
        {
            MapCoord currentCoord = mapCoords.Dequeue();
            map[currentCoord.x, currentCoord.y] = currentCoord;

            foreach (var node in core.GetAllNeighbours(currentCoord.x, currentCoord.y))
            {
                if (mapFlags[node.x, node.y] == 0 && node.Fill == fillType)
                {
                    mapFlags[node.x, node.y] = 1;

                    mapCoords.Enqueue(new MapCoord(node.x, node.y, (char)(currentCoord.Denotion + 1)));
                }
            } 
        }
    }

    public MapCoord[,] GetMap(MapCoord[,] map, BooleanPositionDelegate isSource, BooleanPositionDelegate isBlock, IntegerPositionDelegate isValue)
    {
        if (core != null)
        {
            if (map == null)
                map = this.map;

            return CreateMapFromMap(map, isSource, isBlock, isValue);
        }

        return map;
    }

    private MapCoord[,] CreateMapFromMap(MapCoord[,] map, BooleanPositionDelegate isSource, BooleanPositionDelegate isBlock, IntegerPositionDelegate sourceValue)
    {
        Queue<MapCoord> frontier = new Queue<MapCoord>();

        int height = map.GetLength(0);
        int width = map.GetLength(1);

        MapCoord[,] newMap = new MapCoord[height, width];

        
        for (int i = 0; i < height; ++i)
        {
            for (int j = 0; j < width; ++j)
            {
                if (map[i, j] != null)
                {
                    MapCoord coord = new MapCoord(i, j);
                    newMap[i, j] = coord;

                    if (isSource(i, j))
                    {
                        //Debug.Log("Is source.");
                        coord.Denotion = '@';
                        frontier.Enqueue(coord);
                    }
                    else
                    {
                        coord.Denotion = (char)1000;
                    }
                }
                
            }
        }

        //Debug.Log("Frontier size: " + frontier.Count);

        return DijkstraScan(newMap, frontier, isBlock);
    }

    private MapCoord[,] DijkstraScan(MapCoord[,] map, Queue<MapCoord> frontier, BooleanPositionDelegate isBlock)
    {
        //Debug.Log("Scan begins.");

        while (frontier.Any())
        {
            //Debug.Log("Dequeuing");
            MapCoord coord = frontier.Dequeue();

            foreach (var node in core.GetAllNeighbours(coord.x, coord.y))
            {
                //Debug.Log("Neighbours.");

                if (!isBlock(node.x, node.y))
                {
                    int cost = 1;
                    if (map[coord.x, coord.y].Denotion + cost < map[node.x, node.y].Denotion)
                    {
                        //Debug.Log("Is not correct value.");
                        map[node.x, node.y].Denotion = (char)(map[coord.x, coord.y].Denotion + cost);
                        frontier.Enqueue(new MapCoord(node.x, node.y, (char)cost));
                    }
                } else
                {
                    map[node.x, node.y].Denotion = '#';
                }
            }
        }

        return map;
    }
}

public class MapCoord
{
    public int x;
    public int y;
    public char Denotion;

    public MapCoord(int x, int y)
    {
        this.x = x;
        this.y = y;

        Denotion = '#';
    }

    public MapCoord(int x, int y, char denotion)
    {
        this.x = x;
        this.y = y;

        Denotion = denotion;
    }
}