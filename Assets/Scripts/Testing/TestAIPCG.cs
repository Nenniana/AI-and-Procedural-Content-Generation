using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridSystem;
using Goap;
using System.Linq;
using System;
using Random = UnityEngine.Random;

public class TestAIPCG : MonoBehaviour
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
    int cellularFillPercent = 45;
    [SerializeField]
    string cellularSeed = "Seed";
    [SerializeField]
    bool showDebug = true;
    [SerializeField] 
    PCGType pCGType;
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

    private float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        CreateGridCore();
        CreateMapByPCGMethod();
        SetWalkableNodes();
        Queue<PNode> availablePositions = GetAvaliablePositions();
        InstantiateEnemyAtAvaliablePosition(availablePositions);
        InstantiateAIAtAvailablePositions(availablePositions);
    }

    private void SetWalkableNodes()
    {
        foreach (var node in gridCore.GridList)
        {
            node.SetWalkableBasedOnFill(sprite, nodeStartColor);
        }
    }

    private void CreateGridCore()
    {
        gridCore = new GridCore<PNode>(width, height, cellSizeX, (GridCore<PNode> g, int x, int y, Vector3 position, float width, float height) => new PNode(g, x, y, position, width, height));
        gridCore.DebugText = showDebug;
        
    }

    private void CreateMapByPCGMethod()
    {
        if (pCGType == PCGType.CellularAutomate)
            CellularAutomateCreation.CreateMap(gridCore, cellularFillPercent, 5, 4, NeighbourhoodType.Moore, cellularSeed);
        else
            BinaryPartitionCreation.CreateMap(gridCore, 5, 5, 1, true);
    }

    private Queue<PNode> GetAvaliablePositions()
    {
        return new Queue<PNode>(gridCore.GridList.Where(x => x.IsWalkable == true).OrderBy(x => Guid.NewGuid()));
    }

    private void InstantiateEnemyAtAvaliablePosition(Queue<PNode> availablePositions)
    {
        Instantiate(enemyPrefab, availablePositions.Dequeue().Position, Quaternion.identity);
    }

    private void InstantiateAIAtAvailablePositions(Queue<PNode> availablePositions)
    {
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
        InstantiateEnemyAtInterval();
    }

    private void InstantiateEnemyAtInterval()
    {
        timer += Time.deltaTime;

        if (timer > (120 / aiToGenerate))
        {
            timer = 0;
            InstantiateEnemyAtAvaliablePosition(GetAvaliablePositions());
        }
    }
}