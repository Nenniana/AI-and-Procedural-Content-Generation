using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridSystem;
using BehaviourTree;
using Tree = BehaviourTree.Tree;

public class GuardBehaviourTree : Tree
{
    public static PathfindingController controller;
    public static GridCore<PNode> grid;
    internal static float speed = 2f;
    internal static float FOVRange = 6f;
    internal static float attackRange = 1f;

    public void Initialize(GridCore<PNode> _grid, Color color)
    {
        if (grid == null)
            grid = _grid;

        if (controller == null)
            controller = new PathfindingController(grid);

        SetColor(color);
    }

    protected override Node SetupTree()
    {
        Node root = new Selector(new List<Node>
        {
            new Sequence(new List<Node>
            {
                new CheckEnemyInAttackRange(transform),
                new TaskAttack(),
            }),

            new Sequence(new List<Node>
            {
                new CheckEnemyInRange(transform),
                new TaskGoToTarget(transform),
            }),

            new TaskPatrol(transform)
        });

        return root;
    }

    private void SetColor(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;

        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(color, 0.0f), new GradientColorKey(color, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
        );

        GetComponent<TrailRenderer>().colorGradient = gradient;
    }
}
