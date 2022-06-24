using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System;

namespace GridSystem
{
    public class GridCore<TGridObject>
    {
        public Action<int, int> OnGridValueChanged;

        private int width;
        private int height;
        private float cellSizeX;
        private float cellSizeY;
        private Vector3 originPosition;
        private bool showDebugText = true;
        private bool showDebugGrid = true;

        private TGridObject[,] gridArray;
        private List<TGridObject> gridList;
        private TextMeshPro[,] debugTextArray;
        private Transform textParent;

        public List<TGridObject> GridList { get => gridList; }
        public int Width { get => width; }
        public int Height { get => height; }
        public float CellSizeX { get => cellSizeX; }
        public float CellSizeY { get => cellSizeY; }
        public bool DebugText { get => showDebugText; set { showDebugText = value; ToggleDebugText(value); } }
        public Vector3 OriginPosition { get => originPosition; }

        public GridCore(int width, int height, Vector3 originPosition, float cellSizeX,
            Func<GridCore<TGridObject>, int, int, Vector3, float, float, TGridObject> createGridObject,
            bool showDebugGrid = true, float cellSizeY = 0, bool centerOrigin = true)
        {
            Construct(width, height, originPosition, cellSizeX, createGridObject, showDebugGrid, cellSizeY, centerOrigin);
        }

        public GridCore(int width, int height, float cellSize,
            Func<GridCore<TGridObject>, int, int, Vector3, float, float, TGridObject> createGridObject)
        {
            Construct(width, height, Vector3.zero, cellSize, createGridObject, true, cellSizeX, true);
        }

        private void Construct(int width, int height, Vector3 originPosition, float cellSizeX,
            Func<GridCore<TGridObject>, int, int, Vector3, float, float, TGridObject> createGridObject,
            bool showDebugGrid = true, float cellSizeY = 0, bool centerOrigin = true)
        {
            this.width = width;
            this.height = height;
            this.cellSizeX = cellSizeX;
            this.cellSizeY = cellSizeY;

            if (centerOrigin)
                this.originPosition = new Vector3(-width * 0.5f, -height * 0.5f);
            else
                this.originPosition = originPosition;

            if (this.cellSizeY == 0)
                this.cellSizeY = this.cellSizeX;

            gridArray = new TGridObject[width, height];
            gridList = new List<TGridObject>();
            debugTextArray = new TextMeshPro[width, height];
            textParent = new GameObject("Grid Text Parent").transform;

            if (showDebugGrid)
                ConstructDebugGrid();

            ConstructArray(createGridObject);
            OnGridValueChanged += UpdateDebugText;
        }

        private void ConstructArray(Func<GridCore<TGridObject>, int, int, Vector3, float, float, TGridObject> createGridObject)
        {
            for (int row = 0; row < gridArray.GetLength(0); row++)
            {
                for (int col = 0; col < gridArray.GetLength(1); col++)
                {
                    gridArray[row, col] = createGridObject(this, row, col, GetWorldPosition(row, col), cellSizeX, cellSizeY);
                    gridList.Add(gridArray[row, col]);

                    debugTextArray[row, col] = WorldText.CreateWorldText(gridArray[row, col].ToString(),
                        textParent, GetWorldPosition(row, col) + new Vector3(cellSizeX, cellSizeY) * 0.5f,
                        10, Color.black, TMPro.TextAlignmentOptions.Center, true, cellSizeX, cellSizeY);
                }
            }
        }

        private void ConstructDebugGrid()
        {
            for (int row = 0; row < gridArray.GetLength(0); row++)
            {
                for (int col = 0; col < gridArray.GetLength(1); col++)
                {
                    Debug.DrawLine(GetWorldPosition(row, col), GetWorldPosition(row, col + (int)cellSizeY), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(row, col), GetWorldPosition(row + (int)cellSizeX, col), Color.white, 100f);
                }
            }
            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
        }

        public void SetGridObject(int x, int y, TGridObject value)
        {
            if (CheckGridBounds(x, y))
            {
                gridArray[x, y] = value;
                TriggerOnGridValueChanged(x, y);

                debugTextArray[x, y].text = gridArray[x, y].ToString();
            }
        }

        public void TriggerOnGridValueChanged(int x, int y)
        {
            OnGridValueChanged?.Invoke(x, y);
        }

        public TGridObject GetGridObject(int x, int y)
        {
            if (CheckGridBounds(x, y))
                return gridArray[x, y];

            return default(TGridObject);
        }

        public void SetGridObject(Vector3 worldPosition, TGridObject value)
        {
            GetXY(worldPosition, out int x, out int y);

            SetGridObject(x, y, value);
        }

        public TGridObject GetGridObject(Vector3 worldPosition)
        {
            GetXY(worldPosition, out int x, out int y);

            return GetGridObject(x, y);
        }

        public List<TGridObject> GetStraightNeighbours(int x, int y)
        {
            List<TGridObject> straight = new List<TGridObject>();

            if (GetGridObject(x + 1, y) != null)
                straight.Add(GetGridObject(x + 1, y));
            if (GetGridObject(x - 1, y) != null)
                straight.Add(GetGridObject(x - 1, y));
            if (GetGridObject(x, y + 1) != null)
                straight.Add(GetGridObject(x, y + 1));
            if (GetGridObject(x, y - 1) != null)
                straight.Add(GetGridObject(x, y - 1));

            return straight;
        }

        public List<TGridObject> GetDiagonalNeighbours(int x, int y)
        {
            List<TGridObject> diagonal = new List<TGridObject>();

            if (GetGridObject(x + 1, y + 1) != null)
                diagonal.Add(GetGridObject(x + 1, y + 1));
            if (GetGridObject(x - 1, y + 1) != null)
                diagonal.Add(GetGridObject(x - 1, y + 1));
            if (GetGridObject(x - 1, y - 1) != null)
                diagonal.Add(GetGridObject(x - 1, y - 1));
            if (GetGridObject(x + 1, y - 1) != null)
                diagonal.Add(GetGridObject(x + 1, y - 1));

            return diagonal;
        }

        public List<TGridObject> GetAllNeighbours(int x, int y)
        {
            List<TGridObject> fullList = GetStraightNeighbours(x, y);
            fullList.AddRange(GetDiagonalNeighbours(x, y));

            return fullList;
        }

        public void GetXY(Vector3 worldPosition, out int x, out int y)
        {
            x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSizeX);
            y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSizeY);
        }

        private void ToggleDebugText (bool show)
        {
            if (show)
                textParent.gameObject.SetActive(true);
            else
                textParent.gameObject.SetActive(false);
        }

        private bool CheckGridBounds(int x, int y)
        {
            return (x >= 0 && y >= 0 && x < width && y < height);
        }

        private Vector3 GetWorldPosition (int x, int y)
        {
            return new Vector3(x * cellSizeX, y * cellSizeY, 0) + originPosition;
        }

        private void UpdateDebugText(int x, int y)
        {
            debugTextArray[x, y].text = gridArray[x, y].ToString();
        }
    }
}