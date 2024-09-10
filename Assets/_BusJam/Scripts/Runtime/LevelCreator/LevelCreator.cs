#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BusJam.Scripts.Runtime.LevelCreator
{
    public class LevelCreator : MonoBehaviour
    {
        [HideInInspector] public int gridWidth;
        [HideInInspector] public int gridHeight;
        [HideInInspector] public int levelIndex;
        [HideInInspector] public int matchAreaSize;
        [HideInInspector] public ColorType[] busColors;

        public GameColors gameColors;

        [SerializeField] private float spaceModifier = 1f;
        [SerializeField] private Vector3 gridOffset;

        public GameObject gridPrefab, stickmanPrefab, spawnerPrefab;
        public GameObject inGameWallPrefab;
        public GameObject cornerSharpWall, fullWall, cornerBevelWall, straightWall;

        [Header("Level Settings")]
        [SerializeField] private GridDirectionType directionType;
        [SerializeField] private ColorType colorType;

        public bool isQuestionMark;

        private LevelData _levelData;

        private List<GameObjectList> allBackgroundGridGameObjects = new List<GameObjectList>();

        public void GenerateLevel()
        {
            if (_levelData != null)
                return;
            //Debug.Log(_levelData.GetGridCount() + " 1");
            _levelData = LevelSaveSystem.LoadLevel(levelIndex);
            if (_levelData == null)
            {
                _levelData = new LevelData(gridWidth, gridHeight, busColors);
                SaveLevel();
            }
            //GenerateGridManagerList();
        }

        public void GridButtonAction(int x, int y)
        {
            if (colorType == ColorType.Spawner)
            {
                if (_levelData.GridCells[x, y].colorType == ColorType.Spawner)
                {
                    _levelData.GridCells[x, y].mySpawnerData.myDirection = directionType;
                    SpawnerData spawnerToChangeDir;
                    /* foreach (var spawner in _levelData.SpawnerDataList)
                     {
                         if (spawner.x == x && spawner.y == y)
                         {
                             spawnerToChangeDir = spawner;
                         }
                     }*/
                    spawnerToChangeDir.myDirection = directionType;
                }
                else
                {
                    _levelData.CreateSpawnerDoor(x, y, directionType);
                }
            }
            else
            {
                _levelData.SetCellStackColors(x, y, colorType, isQuestionMark);
            }
        }

        public void GridRemoveButtonAction(int x, int y)
        {
            _levelData.RemoveCellStackColors(x, y, colorType);
        }

        public void ResetSpawnerCellLists(int x, int y, int count)
        {
            _levelData.InitSpawnerCells(x, y, count);
        }

        public void SpawnerQueueButtonAction(int x, int y, int orderAtQueue)
        {
            _levelData.SetSpawnerQueueCellColor(x, y, orderAtQueue, colorType);
        }

        public void SpawnerQueueRemoveButtonAction(int x, int y, int orderAtQueue)
        {
            _levelData.RemoveSpawnerQueueCellColor(x, y, orderAtQueue);
        }

        public void SaveLevel() => LevelSaveSystem.SaveLevel(_levelData, levelIndex);

        public void LoadLevel()
        {
            _levelData = LevelSaveSystem.LoadLevel(levelIndex);

            busColors = _levelData.BusColors;
        }

        public void ResetLevel()
        {
            _levelData = new LevelData(gridWidth, gridHeight, busColors);
            SaveLevel();
        }

        public LevelData GetLevelData() => _levelData;

        public void SetColorEnum(ColorType colorType)
        {
            this.colorType = colorType;
        }

        public void SetDirectionEnum(GridDirectionType direction)
        {
            directionType = direction;
        }

        public void SpawnGrid()
        {
            SaveLevel();

            GridManager gridManager = FindObjectOfType<GridManager>();
            gridManager.Init(gridWidth, gridHeight + 1, spaceModifier);


            GameObject oldParentObject = GameObject.FindGameObjectWithTag("LevelParent");

            if (oldParentObject)
            {
                DestroyImmediate(oldParentObject);
            }

            GameObject newParentObject = new GameObject("LevelParent");
            newParentObject.transform.tag = "LevelParent";

            GameObject gridParentObject = new GameObject("GridParent");
            gridParentObject.transform.SetParent(newParentObject.transform);

            GameObject wallParentObject = new GameObject("WallParent");
            wallParentObject.transform.SetParent(newParentObject.transform);

            GameObject inGameWallParentObject = new GameObject("InGameWallParent");
            inGameWallParentObject.transform.SetParent(newParentObject.transform);

            GameObject matchAreaParentObject = new GameObject("Match Area Parent");
            matchAreaParentObject.transform.SetParent(newParentObject.transform);

            GameObject spawnerParentObject = new GameObject("Spawner Parent");
            spawnerParentObject.transform.SetParent(newParentObject.transform);

            GameObject stickmanParentObject = new GameObject("Stickman Parent");
            stickmanParentObject.transform.SetParent(newParentObject.transform);


            allBackgroundGridGameObjects = new List<GameObjectList>();

            for (int i = 0; i < gridWidth; i++)
            {
                GameObjectList row = new GameObjectList();
                for (int j = 0; j < gridHeight + 1; j++)
                {
                    row.list.Add(null);
                }

                allBackgroundGridGameObjects.Add(row);
            }

            for (int y = 0; y < _levelData.Height + 1; y++)
            {
                for (int x = 0; x < _levelData.Width; x++)
                {
                    Vector3 position = transform.position + GridSpaceToWorldSpace(x, y);
                    GameObject gridObject = PrefabUtility.InstantiatePrefab(gridPrefab.gameObject) as GameObject;
                    gridObject.transform.SetParent(gridParentObject.transform);

                    allBackgroundGridGameObjects[x].list[y] = gridObject;
                    var gridObjectPosition = position;
                    gridObject.transform.position = gridObjectPosition;
                    if (y == _levelData.Height)
                    {
                        gridObject.GetComponentInChildren<MeshRenderer>().enabled = false;
                    }

                    try
                    {
                        _levelData.GetGridCell(x, y);
                    }
                    catch (Exception e)
                    {
                        continue;
                    }

                    var cell = _levelData.GetGridCell(x, y);

                    if (cell.colorType.Equals(ColorType.None) ||
                        cell.colorType.Equals(ColorType.Wall)) continue;



                    gridManager.Fill(new Vector2Int(x, y), 1);

                    if (cell.colorType == ColorType.Spawner)
                    {

                        SpawnSpawnerDoor(x, y, cell, position, spawnerParentObject, stickmanParentObject);

                    }
                    else
                    {
                        SpawnStickman(x, y, cell.colorType, gridManager, stickmanParentObject, position, cell.isQuestionMark);
                    }
                }
            }

            gridManager.UpdateBackgroundGridGameObjects(allBackgroundGridGameObjects);
            GameObject matchArea = SpawnMatchAreas(matchAreaParentObject.transform);
            SpawnWalls(wallParentObject);
            SpawnInGameWalls(inGameWallParentObject);
            gridManager.SetGridMeshes();
            newParentObject.transform.localPosition = new Vector3(1.5f, 0, 5.15f - matchArea.transform.position.z);
            EditorUtility.SetDirty(gridManager);
        }

        private Vector3 CalculateUpperCenter(int width, int height)
        {
            return new Vector3(width * spaceModifier * 1 - (gridWidth * spaceModifier / 2) + 0.5f,
                0, height * spaceModifier);
        }

        private Vector3 GridSpaceToWorldSpace(int x, int y)
        {
            return new Vector3(x * spaceModifier * 1 - (gridWidth * spaceModifier / 2) + 0.375f,
                0, y * spaceModifier);
        }
        private StickmanParent SpawnStickman(int x, int y, ColorType colorType, GridManager gridManager, GameObject newParentObject, Vector3 position, bool isQuestionMark)
        {

            Material activeMaterial = gameColors.ActiveMaterials[(int)colorType - 2];
            GameObject obj =
                           PrefabUtility.InstantiatePrefab(stickmanPrefab.gameObject) as GameObject;
            obj.transform.SetParent(newParentObject.transform);
            obj.transform.position = position;
            StickmanParent stickmanParent = obj.GetComponent<StickmanParent>();
            stickmanParent.Init(colorType, activeMaterial, x, y, isQuestionMark, gridManager);
            return stickmanParent;
        }
        private GameObject SpawnMatchAreas(Transform matchAreaParentObject)
        {
            var upperCenter = new Vector3(0, 0, 0);
            upperCenter.z = allBackgroundGridGameObjects[0].list[^1].transform.position.z + 1;

            int totalMatchAreas = matchAreaSize;
            bool isEven = totalMatchAreas % 2 == 0;
            int pairsToSpawn = isEven ? totalMatchAreas / 2 : (totalMatchAreas - 1) / 2;
            GameObject matchArea = null;
            // Create central match area if the number is odd
            if (!isEven)
            {
                matchArea = CreateMatchArea(upperCenter, matchAreaParentObject);
            }

            // Create pairs of match areas
            for (int i = 0; i < pairsToSpawn; i++)
            {
                float offset = spaceModifier * (i + 1);
                Vector3 rightPosition = new Vector3(upperCenter.x + offset, upperCenter.y, upperCenter.z);
                Vector3 leftPosition = new Vector3(upperCenter.x - offset, upperCenter.y, upperCenter.z);

                CreateMatchArea(rightPosition, matchAreaParentObject);
                CreateMatchArea(leftPosition, matchAreaParentObject);
            }


            GridManager gridManager = FindObjectOfType<GridManager>();
            EditorUtility.SetDirty(gridManager);
            return matchArea;
        }

        private GameObject CreateMatchArea(Vector3 position, Transform parent)
        {
            GameObject matchArea = PrefabUtility.InstantiatePrefab(gridPrefab.gameObject) as GameObject;
            matchArea.transform.SetParent(parent);
            matchArea.transform.position = new Vector3(position.x, 0, position.z);
            FindObjectOfType<GridManager>().AddMatchAreaToList(matchArea);
            return matchArea;
        }

        private void SpawnWalls(GameObject wallParent)
        {
            float zMultiplier = 0.8f;

            // bottom
            for (int i = 0; i < gridWidth + 6; i++)
            {
                GameObject obj = PrefabUtility.InstantiatePrefab(straightWall.gameObject) as GameObject;
                obj.transform.position = new Vector3(
                    allBackgroundGridGameObjects[0].list[0].transform.position.x + i - 3, -0.72f,
                    allBackgroundGridGameObjects[0].list[0].transform.position.z - 1.2f);
                obj.transform.SetParent(wallParent.transform);


                obj = PrefabUtility.InstantiatePrefab(fullWall.gameObject) as GameObject;
                obj.transform.position = new Vector3(
                    allBackgroundGridGameObjects[0].list[0].transform.position.x + i - 3, -0.72f,
                    allBackgroundGridGameObjects[0].list[0].transform.position.z - 1.2f - 1f);
                obj.transform.SetParent(wallParent.transform);


                obj = PrefabUtility.InstantiatePrefab(fullWall.gameObject) as GameObject;
                obj.transform.position = new Vector3(
                    allBackgroundGridGameObjects[0].list[0].transform.position.x + i - 3, -0.72f,
                    allBackgroundGridGameObjects[0].list[0].transform.position.z - 1.2f - 2f);
                obj.transform.SetParent(wallParent.transform);


                obj = PrefabUtility.InstantiatePrefab(fullWall.gameObject) as GameObject;
                obj.transform.position = new Vector3(
                    allBackgroundGridGameObjects[0].list[0].transform.position.x + i - 3, -0.72f,
                    allBackgroundGridGameObjects[0].list[0].transform.position.z - 1.2f - 3f);
                obj.transform.SetParent(wallParent.transform);

                obj = PrefabUtility.InstantiatePrefab(fullWall.gameObject) as GameObject;
                obj.transform.position = new Vector3(
                    allBackgroundGridGameObjects[0].list[0].transform.position.x + i - 3, -0.72f,
                    allBackgroundGridGameObjects[0].list[0].transform.position.z - 1.2f - 4f);
                obj.transform.SetParent(wallParent.transform);
                obj.transform.localScale = new Vector3(obj.transform.localScale.x, obj.transform.localScale.y, 3);
            }

            GameObject objBevel = PrefabUtility.InstantiatePrefab(cornerBevelWall.gameObject) as GameObject;
            objBevel.transform.position = new Vector3(
                allBackgroundGridGameObjects[0].list[0].transform.position.x - 1.1f,
                -0.72f,
                allBackgroundGridGameObjects[0].list[0].transform.position.z - 1.2f);
            objBevel.transform.SetParent(wallParent.transform);
            objBevel.transform.localEulerAngles = Vector3.zero;
            // left
            for (int i = 0; i < gridHeight; i++)
            {
                GameObject obj = PrefabUtility.InstantiatePrefab(straightWall.gameObject) as GameObject;
                obj.transform.position = new Vector3(
                    allBackgroundGridGameObjects[0].list[0].transform.position.x - 1.1f, -0.72f,
                    allBackgroundGridGameObjects[0].list[0].transform.position.z - 1.2f + i * zMultiplier);
                obj.transform.SetParent(wallParent.transform);
                obj.transform.localEulerAngles = Vector3.zero;


                obj = PrefabUtility.InstantiatePrefab(fullWall.gameObject) as GameObject;
                obj.transform.position = new Vector3(
                    allBackgroundGridGameObjects[0].list[0].transform.position.x - 1.1f - 1f, -0.72f,
                    allBackgroundGridGameObjects[0].list[0].transform.position.z - 1.2f + i * zMultiplier);
                obj.transform.SetParent(wallParent.transform);


                obj = PrefabUtility.InstantiatePrefab(fullWall.gameObject) as GameObject;
                obj.transform.position = new Vector3(
                    allBackgroundGridGameObjects[0].list[0].transform.position.x - 1.1f - 2f, -0.72f,
                    allBackgroundGridGameObjects[0].list[0].transform.position.z - 1.2f + i * zMultiplier);
                obj.transform.SetParent(wallParent.transform);
            }

            GameObject objCornerSharp = PrefabUtility.InstantiatePrefab(cornerSharpWall.gameObject) as GameObject;
            objCornerSharp.transform.position = new Vector3(
                allBackgroundGridGameObjects[0].list[0].transform.position.x - 1.1f, -0.72f,
                allBackgroundGridGameObjects[0].list[0].transform.position.z - 1.2f + gridHeight * zMultiplier);
            objCornerSharp.transform.SetParent(wallParent.transform);
            objCornerSharp.transform.localEulerAngles = Vector3.zero;


            GameObject objFull = PrefabUtility.InstantiatePrefab(straightWall.gameObject) as GameObject;
            objFull.transform.position = new Vector3(
                allBackgroundGridGameObjects[0].list[0].transform.position.x - 1.1f - 1f, -0.72f,
                allBackgroundGridGameObjects[0].list[0].transform.position.z - 1.2f + gridHeight * zMultiplier);
            objFull.transform.SetParent(wallParent.transform);


            objFull = PrefabUtility.InstantiatePrefab(straightWall.gameObject) as GameObject;
            objFull.transform.position = new Vector3(
                allBackgroundGridGameObjects[0].list[0].transform.position.x - 1.1f - 2f, -0.72f,
                allBackgroundGridGameObjects[0].list[0].transform.position.z - 1.2f + gridHeight * zMultiplier);
            objFull.transform.SetParent(wallParent.transform);


            objBevel = PrefabUtility.InstantiatePrefab(cornerBevelWall.gameObject) as GameObject;
            objBevel.transform.position = new Vector3(
                allBackgroundGridGameObjects[^1].list[0].transform.position.x + 1.1f,
                -0.72f,
                allBackgroundGridGameObjects[^1].list[0].transform.position.z - 1.2f);
            objBevel.transform.SetParent(wallParent.transform);
            objBevel.transform.localEulerAngles = Vector3.zero;

            objBevel.transform.localScale = new Vector3(-1, 1, 1);

            // right
            for (int i = 0; i < gridHeight; i++)
            {
                GameObject obj = PrefabUtility.InstantiatePrefab(straightWall.gameObject) as GameObject;
                obj.transform.position = new Vector3(
                    allBackgroundGridGameObjects[^1].list[0].transform.position.x + 1.1f, -0.72f,
                    allBackgroundGridGameObjects[^1].list[0].transform.position.z - 1.2f + i * zMultiplier);
                obj.transform.SetParent(wallParent.transform);
                obj.transform.localEulerAngles = new Vector3(0, 180, 0);


                obj = PrefabUtility.InstantiatePrefab(fullWall.gameObject) as GameObject;
                obj.transform.position = new Vector3(
                    allBackgroundGridGameObjects[^1].list[0].transform.position.x + 1.1f + 1f, -0.72f,
                    allBackgroundGridGameObjects[^1].list[0].transform.position.z - 1.2f + i * zMultiplier);
                obj.transform.SetParent(wallParent.transform);


                obj = PrefabUtility.InstantiatePrefab(fullWall.gameObject) as GameObject;
                obj.transform.position = new Vector3(
                    allBackgroundGridGameObjects[^1].list[0].transform.position.x + 1.1f + 2f, -0.72f,
                    allBackgroundGridGameObjects[^1].list[0].transform.position.z - 1.2f + i * zMultiplier);
                obj.transform.SetParent(wallParent.transform);
            }

            objCornerSharp = PrefabUtility.InstantiatePrefab(cornerSharpWall.gameObject) as GameObject;
            objCornerSharp.transform.position = new Vector3(
                allBackgroundGridGameObjects[^1].list[0].transform.position.x + 1.1f, -0.72f,
                allBackgroundGridGameObjects[^1].list[0].transform.position.z - 1.2f + gridHeight * zMultiplier);
            objCornerSharp.transform.SetParent(wallParent.transform);
            objCornerSharp.transform.localEulerAngles = new Vector3(0, 270, 0);


            objFull = PrefabUtility.InstantiatePrefab(straightWall.gameObject) as GameObject;
            objFull.transform.position = new Vector3(
                allBackgroundGridGameObjects[^1].list[0].transform.position.x + 1.1f + 1f, -0.72f,
                allBackgroundGridGameObjects[^1].list[0].transform.position.z - 1.2f + gridHeight * zMultiplier);
            objFull.transform.SetParent(wallParent.transform);


            objFull = PrefabUtility.InstantiatePrefab(straightWall.gameObject) as GameObject;
            objFull.transform.position = new Vector3(
                allBackgroundGridGameObjects[^1].list[0].transform.position.x + 1.1f + 2f, -0.72f,
                allBackgroundGridGameObjects[^1].list[0].transform.position.z - 1.2f + gridHeight * zMultiplier);
            objFull.transform.SetParent(wallParent.transform);
        }

        private void SpawnSpawnerDoor(int x, int y, GridCell cell, Vector3 position,
           GameObject parent, GameObject stickmanParentObject)
        {
            GridManager gridManager = FindObjectOfType<GridManager>();
            GameObject obj =
                PrefabUtility.InstantiatePrefab(spawnerPrefab.gameObject) as GameObject;
            obj.transform.SetParent(parent.transform);
            obj.transform.position = position + new Vector3(0, .25f, 0);

            Spawner spawner = obj.GetComponent<Spawner>();

            Vector2Int spawnerTargetIndex = new Vector2Int(x, y);
            switch (cell.mySpawnerData.myDirection)
            {
                case GridDirectionType.Up:
                    spawner.transform.eulerAngles = new Vector3(0, 0, 0);
                    spawnerTargetIndex.y++;
                    break;
                case GridDirectionType.Right:
                    spawner.transform.eulerAngles = new Vector3(0, 90, 0);
                    spawnerTargetIndex.x++;
                    break;
                case GridDirectionType.Down:
                    spawner.transform.eulerAngles = new Vector3(0, 180, 0);
                    spawnerTargetIndex.y--;
                    break;
                case GridDirectionType.Left:
                    spawnerTargetIndex.x--;
                    spawner.transform.eulerAngles = new Vector3(0, 270, 0);
                    break;
                default:
                    break;
            }

            List<StickmanParent> stickmenToSpawn = new();
            for (int i = 0; i < cell.mySpawnerData.stickmanInSpawner.Count; i++)
            {
                StickmanInSpawner stickmanInSpawner = cell.mySpawnerData.stickmanInSpawner[i];
                StickmanParent stickmanGO = SpawnStickman(x, y, stickmanInSpawner.colorType, gridManager, stickmanParentObject, position, false);
                stickmanGO.gameObject.SetActive(false);
                stickmenToSpawn.Add(stickmanGO);
            }

            spawner.Init(x, y, spawnerTargetIndex, stickmenToSpawn, cell.mySpawnerData.myDirection,
                spawnerTargetIndex);

            gridManager.AddSpawner(spawner);
        }
        private void SpawnInGameWalls(GameObject wallParent)
        {
            /*for (int i = 0; i < wallData.Length; i++)
            {
                GameObject wall = SpawnWall(wallParent, wallData[i], gridSizeX, gridSizeY);
                gridManager.gridObjectList[(gridSizeY - wallData[i].y - 2) + 4].list[(wallData[i].x - 1) + 3] =
                    wall.GetComponent<GridObject>();
            }*/

            GridManager gridManager = FindObjectOfType<GridManager>();
            gridManager.inGameWallList.Clear();
            for (int j = 0; j < gridHeight; j++)
            {
                InGameWallList wallList = new InGameWallList();
                for (int i = 0; i < gridWidth; i++)
                {
                    wallList.list.Add(null);
                }

                gridManager.inGameWallList.Add(wallList);
            }


            for (int y = 0; y < _levelData.Height; y++)
            {
                for (int x = 0; x < _levelData.Width; x++)
                {
                    var cell = _levelData.GetGridCell(x, y);

                    if (cell.colorType.Equals(ColorType.None) ||
                        !cell.colorType.Equals(ColorType.Wall)) continue;


                    gridManager.Fill(new Vector2Int(x, y), 1);
                    GameObject obj =
                        PrefabUtility.InstantiatePrefab(inGameWallPrefab.gameObject) as GameObject;
                    obj.transform.SetParent(wallParent.transform);
                    Vector3 position = transform.position +
                                       GridSpaceToWorldSpace(x, y);
                    obj.transform.position = position;
                    if (obj.TryGetComponent(out InGameWall inGameWall))
                    {
                        inGameWall.x = x;
                        inGameWall.y = y;
                        gridManager.inGameWallList[y].list[x] = inGameWall;
                    }
                }
            }
        }
    }
}
#endif