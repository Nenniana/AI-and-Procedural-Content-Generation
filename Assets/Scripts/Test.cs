using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridSystem;
using Goap;
using System.Linq;
using System;
using Random = UnityEngine.Random;

public class Test : MonoBehaviour
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
    int fillPercent = 45;
    [SerializeField]
    bool showDebug = true;
    [SerializeField] 
    AIType aIType;
    [SerializeField] 
    int aiToGenerate = 10;

    [SerializeField]
    Sprite sprite;
    [SerializeField]
    Color nodeStartColor;

    [SerializeField]
    GameObject aiPrefab;

    [SerializeField]
    GameObject btAIPrefab;

    [SerializeField]
    GameObject goapAIPrefab;

    [SerializeField]
    GameObject enemyPrefab;

    private GridCore<PNode> gridCore;
    private FloodMap map;
    private float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        gridCore = new GridCore<PNode>(width, height, cellSizeX, (GridCore<PNode> g, int x, int y, Vector3 position, float width, float height) => new PNode(g, x, y, position, width, height));
        gridCore.DebugText = showDebug;

        //CellularAutomateCreation.CreateMap(gridCore, fillPercent, 5, 4, NeighbourhoodType.Moore, "Toto");
        BinaryPartitionCreation.CreateMap(gridCore, 5, 5, 1, true);

        foreach (var node in gridCore.GridList)
        {
            node.SetWalkableBasedOnFill(sprite, nodeStartColor);
        }

        //float spawnRadius = (width > height) ? height : width;

        //ObjectSpawnerHelper objectSpawner = new ObjectSpawnerHelper();

        //foreach (var item in objectSpawner.SpawnObjects(aiPrefab, aiToGenerate, Vector2.zero, new Vector2(width * (cellSizeX / 2), height * (cellSizeX / 2)), Quaternion.identity, SpawnContainerType.Cube))
        //{
        //    Color color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        //    item.GetComponent<AIController>().Initialize(gridCore, color);
        //}

        Queue<PNode> availablePositions = new Queue<PNode>(gridCore.GridList.Where(x => x.IsWalkable == true).OrderBy(x => Guid.NewGuid()));

        map = new FloodMap();

        PNode floodPosition = availablePositions.Dequeue();

        map.InitializeDjikstraMap(gridCore, floodPosition.x, floodPosition.y, 0);

        foreach (var item in map.Map)
        {
            if (item != null)
                gridCore.GetGridObject(item.x, item.y).Denotion = item.Denotion;
        }
        Instantiate(enemyPrefab, availablePositions.Dequeue().Position, Quaternion.identity);

        for (int i = 0; i < aiToGenerate; i++)
        {
            //Vector3 position = Random.insideUnitCircle * new Vector2(width * (cellSizeX / 2), height * (cellSizeX / 2));
            Color color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            if (aIType == AIType.BehaviourTree)
                Instantiate(btAIPrefab, availablePositions.Dequeue().Position, Quaternion.identity).GetComponent<GuardBehaviourTree>().Initialize(gridCore, color);
            else if (aIType == AIType.GoalOrientedActionPlanning)
                Instantiate(goapAIPrefab, availablePositions.Dequeue().Position, Quaternion.identity).GetComponent<GoalPlanner>().Initialize(gridCore, color);
            else
                Instantiate(aiPrefab, availablePositions.Dequeue().Position, Quaternion.identity).GetComponent<AIController>().Initialize(gridCore, color);
        }
    }

    private void Update()
    {
        /*if (Input.GetMouseButtonDown(1))
        {
            MapCoord[,] coords = map.GetMap(map.Map, (x, y) => map.Map[x, y].Denotion == 'C', (x, y) => map.Map[x, y].Denotion == '#', x => 1);

            //Vector3 pos = MouseWorldPosition.GetMouseWorldPosition();
            //gridCore.GetXY(pos, out int x, out int y);
            //map.UpdateMap(x, y, 0);
            //map.Test();

            foreach (var item in coords)
            {
                if (item != null)
                    gridCore.GetGridObject(item.x, item.y).Denotion = item.Denotion;
            }
        }*/

        timer += Time.deltaTime;

        if (timer > (120 / aiToGenerate))
        {
            timer = 0;
            Queue<PNode> availablePositions = new Queue<PNode>(gridCore.GridList.Where(x => x.IsWalkable == true).OrderBy(x => Guid.NewGuid()));
            Instantiate(enemyPrefab, availablePositions.Dequeue().Position, Quaternion.identity);
        }
    }


}