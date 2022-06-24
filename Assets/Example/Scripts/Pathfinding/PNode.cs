using GridSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PNode
{
    public int x, y;
    private float width, height;
    private Vector3 position;
    private GridCore<PNode> gridCore;

    private SpriteRenderer spriteRenderer;
    private GameObject spriteObject;
    private string blackColor = "#FFEB00"; //Black = 424242
    private Color black;

    public int gCost;
    public int hCost;
    public int fCost;
    public bool visited;
    public bool visitedByEnd;
    public PNode cameFromNode;

    private bool isWalkable = true;
    private int fill = 0;
    private char denotion = '#';

    public bool IsWalkable { get => isWalkable; set { isWalkable = value; if (!value) { SetSpriteRenderer(); SetColor(black); } else SetColor(Color.white); } }

    public Vector3 Position { get { return position; } }

    public int Fill { get => fill; set => fill = value; }
    public char Denotion { get => denotion; set { denotion = value; gridCore.TriggerOnGridValueChanged(x, y); } }

    public PNode(GridCore<PNode> gridCore, int x, int y, Vector3 position, float height, float width)
    {
        ColorUtility.TryParseHtmlString(blackColor, out black);
        this.gridCore = gridCore;
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
        this.position = position + new Vector3(width * 0.5f, height * 0.5f, 0);
    }

    private void SetSpriteRenderer()
    {
        if (spriteRenderer == null)
        {
            spriteObject = new GameObject("Node", typeof(SpriteRenderer));
            spriteRenderer = spriteObject.GetComponent<SpriteRenderer>();
            spriteObject.transform.localPosition = position;
        }
    }

    public void SetUpRenderer(Sprite sprite, Color color)
    {
        SetSpriteRenderer();

        spriteRenderer.sprite = sprite;
        spriteRenderer.color = color;

        if (!isWalkable)
            spriteRenderer.color = black;
    }

    public void SetWalkableBasedOnFill(Sprite sprite, Color color)
    {
        if (fill == 1)
            IsWalkable = false;

        SetUpRenderer(sprite, color);
    }

    public void ResetNode()
    {
        visited = false;
        visitedByEnd = false;
        gCost = int.MaxValue;
        hCost = 0;
        fCost = 0;
        cameFromNode = null;
        CalculateFCost();
    }

    public void SetColor (Color color)
    {
        spriteRenderer.color = color;
    }

    private string CalculateCostString ()
    {
        int tempG = gCost;
        int tempF = fCost;

        if (tempG > 1000)
        {
            tempG = 0;
            tempF = 0;
        }

        return "G: " + tempG + "\nF: " + tempF + "\nH: " + hCost;
    }

    public override string ToString()
    {
        return denotion.ToString();
    }

    internal void CalculateFCost()
    {
        fCost = gCost + hCost;
        gridCore.TriggerOnGridValueChanged(x, y);
    }
}