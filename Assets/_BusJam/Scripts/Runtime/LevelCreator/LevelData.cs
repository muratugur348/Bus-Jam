using System;
using System.Collections.Generic;
using UnityEngine;

namespace BusJam.Scripts.Runtime.LevelCreator
{
    public enum ColorType
    {
        None = 0,
        Spawner = 1,
        Wall = 2,
        Red = 3,
        Green = 4,
        Blue = 5,
        Yellow = 6,
        Purple = 7,
        Orange = 8,
        Pink = 9
    }
    public enum GridDirectionType
    {
        Right,
        Left,
        Up,
        Down
    }

    [Serializable]
    public struct GridCell
    {
        public int X;
        public int Y;
        public ColorType colorType;
        public bool isQuestionMark;
        public SpawnerData mySpawnerData;
        // Add your serializable data here
    }

    public struct StickmanInSpawner
    {
        public int order;
        public ColorType colorType;


        public StickmanInSpawner(int order, ColorType colorType)
        {
            this.order = order;
            this.colorType = colorType;
        }
    }

    public struct SpawnerData
    {
        public int x, y;
        public List<StickmanInSpawner> stickmanInSpawner;
        public GridDirectionType myDirection;
    }

    [Serializable]
    public class LevelData
    {
        public int Width => GridCells.GetLength(0);
        public int Height => GridCells.GetLength(1);
        public GridCell[,] GridCells { get; set; }

        public ColorType[] BusColors;

        public LevelData(int width, int height, ColorType[] busColors)
        {
            BusColors = busColors;
            GridCells = new GridCell[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    GridCells[x, y] = new GridCell
                    {
                        X = x,
                        Y = y
                    };
                    GridCells[x, y].colorType = ColorType.None;
                }
            }

        }

        public void SetCellStackColors(int x, int y, ColorType stackColor, bool isQuestionMark)
        {
            GridCells[x, y].colorType = stackColor;
            GridCells[x, y].isQuestionMark = isQuestionMark;

        }

        public void RemoveCellStackColors(int x, int y, ColorType stackColorList)
        {

            GridCells[x, y].colorType = ColorType.None;

        }

        public GridCell[,] GetGrid() => GridCells;
        public GridCell GetGridCell(int x, int y) => GridCells[x, y];

        public void InitSpawnerCells(int x, int y, int count)
        {
            SpawnerData spawner = GetGridCell(x, y).mySpawnerData;

            if (spawner.stickmanInSpawner.Count == count)
            {
                // If the existing array length is equal to count, no need to modify
                return;
            }
            else if (spawner.stickmanInSpawner.Count > count)
            {
                // If the existing array length is greater than count, remove excess elements from the end
                List<StickmanInSpawner> newCells = new List<StickmanInSpawner>();

                newCells.AddRange(spawner.stickmanInSpawner);
                while (newCells.Count > count)
                {
                    newCells.RemoveAt(newCells.Count - 1);
                }
                spawner.stickmanInSpawner = newCells;
                GridCells[x, y].mySpawnerData = spawner;
            }
            else
            {
                List<StickmanInSpawner> newCells = new List<StickmanInSpawner>();
                newCells.AddRange(spawner.stickmanInSpawner);

                // Initialize new elements
                for (int i = spawner.stickmanInSpawner.Count; i < count; i++)
                {
                    newCells.Add(new StickmanInSpawner
                    {
                        order = i,
                        colorType = ColorType.None
                    });
                }
                spawner.stickmanInSpawner = newCells;
                GridCells[x, y].mySpawnerData = spawner;
            }
        }

        public void SetSpawnerQueueCellColor(int x, int y, int orderAtQueue, ColorType colorType)
        {
            Debug.Log("x " + x + " y " + y + " orderAtQueue " + orderAtQueue);
            if (GridCells[x, y].colorType != ColorType.Spawner)
            {
                Debug.Log("No Spawner Data On " + x + " " + y);
            }

            SpawnerData spawnerData = GridCells[x, y].mySpawnerData;

            if (spawnerData.stickmanInSpawner == null)
            {
                spawnerData.stickmanInSpawner = new List<StickmanInSpawner>();
            }

            while (spawnerData.stickmanInSpawner.Count < orderAtQueue)
            {
                spawnerData.stickmanInSpawner.Add(new StickmanInSpawner(0, ColorType.None));
            }

            StickmanInSpawner updatedJelloInSpawner = new StickmanInSpawner(orderAtQueue, colorType);
            spawnerData.stickmanInSpawner[orderAtQueue] = updatedJelloInSpawner;
        }


        public void RemoveSpawnerQueueCellColor(int x, int y, int orderAtQueue)
        {
            StickmanInSpawner stickmanInSpawner = GridCells[x, y].mySpawnerData.stickmanInSpawner[orderAtQueue];
            stickmanInSpawner.colorType = ColorType.None;
        }

        public void CreateSpawnerDoor(int x, int y, GridDirectionType gridDirectionType)
        {
            if (GridCells[x, y].colorType != ColorType.Spawner)
            {
                SpawnerData newSpawner = new SpawnerData();
                newSpawner.x = x;
                newSpawner.y = y;
                newSpawner.myDirection = gridDirectionType;
                newSpawner.stickmanInSpawner = new List<StickmanInSpawner>();
                newSpawner.myDirection = gridDirectionType;
                newSpawner.stickmanInSpawner.Add(new StickmanInSpawner(0, 0));
                newSpawner.stickmanInSpawner.Add(new StickmanInSpawner(1, 0));

                GridCells[x, y].colorType = ColorType.Spawner;
                GridCells[x, y].mySpawnerData = newSpawner;
            }
        }
    }
}