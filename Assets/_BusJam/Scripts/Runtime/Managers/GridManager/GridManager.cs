using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using BusJam.Scripts.Runtime;
using BusJam.Scripts.Runtime.Managers.ViewManager;
using BusJam.Scripts.Runtime.Presenters.GameplayPresenter;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusJam.Scripts.Runtime.Managers.GameManager;
using UnityEngine;
using BusJam.Scripts.Runtime.LevelCreator;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    [Header("Match Area Color Parameters")]
    [SerializeField]
    private Color matchAreaTileDefaultColor;

    [SerializeField] private Color matchAreaTileHighlightedColor;
    [SerializeField] private Color matchAreaTileRedColor;
    [SerializeField] private float matchAreaTileColorChangeTime;

    [Header("Lists - No Touch")] public List<IntList> gridList = new List<IntList>();
    public List<GameObject> matchAreaList = new List<GameObject>();
    public List<StickmanParent> questionMarkedStickmen = new List<StickmanParent>();
    public List<MeshRenderer> matchAreaMeshRendererList = new List<MeshRenderer>();
    public List<StickmanParent> stickmanParentsOnMatchAreaList = new List<StickmanParent>();
    public List<GameObjectList> backgroundGrids = new List<GameObjectList>();
    public List<StickmanParent> allStickmenInGame = new List<StickmanParent>();
    public List<InGameWallList> inGameWallList = new List<InGameWallList>();
    public List<Spawner> spawners = new List<Spawner>();

    public bool isNewBusChecking;
    [Header("Grid Parameters - No Touch")] public int width, height;
    public float spaceModifier;

    private IViewManager _viewManager;

    private void Awake()
    {
        MakeSingleton();
    }

    private void MakeSingleton()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private IEnumerator Start()
    {
        matchAreaList.Sort((a, b) => a.transform.position.x.CompareTo(b.transform.position.x));
        foreach (var matchArea in matchAreaList)
            matchAreaMeshRendererList.Add(matchArea.GetComponentInChildren<MeshRenderer>());

        _viewManager = Locator.Instance.Resolve<IViewManager>();
        _viewManager.LoadView(new LoadViewParams<GameplayPresenter>("GameplayView"));

        yield return new WaitForEndOfFrame();
        CheckOutlines();
    }

    public void Init(int width, int height, float spaceModifier)
    {
        this.width = width;
        this.height = height;
        this.spaceModifier = spaceModifier;
        gridList = new List<IntList>();
        for (int x = 0; x < width; x++)
        {
            IntList row = new IntList();
            for (int y = 0; y < height; y++)
            {
                row.list.Add(0);
            }

            gridList.Add(row);
        }

        matchAreaList.Clear();
        questionMarkedStickmen.Clear();
        spawners.Clear();
    }


    public void SetGridMeshes()
    {
        for (int j = 0; j < inGameWallList.Count; j++)
        {
            for (int i = 0; i < inGameWallList[j].list.Count; i++)
            {
#if UNITY_EDITOR
                if (inGameWallList[j].list[i] != null)
                    inGameWallList[j].list[i].SetMesh(inGameWallList);
#endif
            }
        }
    }

    public void UpdateBackgroundGridGameObjects(List<GameObjectList> allBackgroundGridGameObjects)
    {
        backgroundGrids = allBackgroundGridGameObjects;
    }

    public void Fill(Vector2Int position, int id)
    {
        gridList[position.x].list[position.y] = id;
    }

    public void AddQuestionMarkedStickman(StickmanParent questionedStickman)
    {
        if (!questionMarkedStickmen.Contains(questionedStickman))
            questionMarkedStickmen.Add(questionedStickman);
    }

    public void RemoveQuestionMarkedStickman(StickmanParent questionedStickman)
    {
        if (questionMarkedStickmen.Contains(questionedStickman))
            questionMarkedStickmen.Remove(questionedStickman);
    }

    public async Task CheckLevelFailed()
    {
        if (IsThereAnyAvailableMatchGrid())
            return;
        for (int i = 0; i < stickmanParentsOnMatchAreaList.Count; i++)
        {
            await WaitForStickmanToStopMoving(i);
        }

        if (IsThereAnyAvailableMatchGrid())
            return;


        while (isNewBusChecking)
        {
            await UniTask.Delay(100);
        }

        if (IsThereAnyAvailableMatchGrid())
            return;

        bool isFilled = BusManager.Instance.activeBus.currentStickmanAmount ==
                        BusManager.Instance.stickmanCapacity;

        bool isNextCanTake = BusManager.Instance.CanNextBusTakeStickmanFromMatchArea();

        if ((isFilled && !isNextCanTake) || !isFilled)
        {

            Locator.Instance.Resolve<IGameManager>().FailLevel();
        }
    }

    public async Task WaitForStickmanToStopMoving(int index)
    {
        // Check if the index is within bounds
        if (index < 0 || index >= stickmanParentsOnMatchAreaList.Count)
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");

        // Loop until isMovingToMatchArea is false
        while (stickmanParentsOnMatchAreaList[index] != null && stickmanParentsOnMatchAreaList[index].isMovingToMatchArea)
        {
            // Wait a short time before checking again to reduce CPU usage
            await Task.Delay(100); // 100 milliseconds delay
        }
    }

    public void AddMatchAreaToList(GameObject go)
    {
        matchAreaList.Add(go);
    }

    public bool IsThereAnyAvailableMatchGrid()
    {
        for (int i = 0; i < matchAreaList.Count; i++)
        {
            if (matchAreaList[i].transform.childCount == 1)
                return true;
        }

        return false;
    }

    public void CheckSpawners(StickmanParent sourceStickman)
    {
        for (int i = 0; i < spawners.Count; i++)
        {
            spawners[i].CheckToSpawn(sourceStickman);
        }
    }

    public GameObject GetAvailableGrid()
    {
        //ClearStickmanParents();
        for (int i = 0; i < matchAreaList.Count; i++)
        {
            if (matchAreaList[i].transform.childCount == 1)
            {
                return matchAreaList[i].gameObject;
            }
        }

        return null;
    }

    public void AddStickmanToMatchAreaList(StickmanParent stickmanParent)
    {
        if (!stickmanParentsOnMatchAreaList.Contains(stickmanParent))
            stickmanParentsOnMatchAreaList.Add(stickmanParent);
    }

    public void RemoveStickmanFromMatchAreaList(StickmanParent stickmanParent)
    {
        if (stickmanParentsOnMatchAreaList.Contains(stickmanParent))
            stickmanParentsOnMatchAreaList.Remove(stickmanParent);
    }
    public void AddSpawner(Spawner spawner)
    {
        if (!spawners.Contains(spawner))
            spawners.Add(spawner);
    }

    public void RemoveSpawner(Spawner spawner)
    {
        if (spawners.Contains(spawner))
            spawners.Remove(spawner);
    }

    public async void HighlightTile(int tileIndex)
    {
        if (tileIndex == matchAreaList.Count - 2) MakeLastTileRedWithLoop(tileIndex + 1);

        await matchAreaMeshRendererList[tileIndex].material
            .DOColor(matchAreaTileHighlightedColor, matchAreaTileColorChangeTime);
        matchAreaMeshRendererList[tileIndex].material
            .DOColor(matchAreaTileDefaultColor, matchAreaTileColorChangeTime);
    }

    private async void MakeLastTileRedWithLoop(int tileIndex)
    {
        while (stickmanParentsOnMatchAreaList.Count == matchAreaList.Count - 1)
        {
            await matchAreaMeshRendererList[tileIndex].material
                .DOColor(matchAreaTileRedColor, matchAreaTileColorChangeTime);
            await matchAreaMeshRendererList[tileIndex].material
                .DOColor(matchAreaTileDefaultColor, matchAreaTileColorChangeTime);
        }
    }


    public async void TryToMoveStickmanToBus(ColorType colorType)
    {
        isNewBusChecking = true;
        List<StickmanParent> tempStickmanParentsOnMatchAreaList = stickmanParentsOnMatchAreaList.ToList();
        for (int i = 0; i < tempStickmanParentsOnMatchAreaList.Count; i++)
        {
            tempStickmanParentsOnMatchAreaList[i].TryToGoBusFromMatchArea();
            await UniTask.WaitForSeconds(.1f);
        }

        //ReorderMatchArea();

        isNewBusChecking = false;
        CheckLevelFailed();
    }


    public (bool, Dictionary<Vector2Int, Vector2Int?>) FindShortestPath(Vector2Int start, Vector2Int end)
    {
        if (gridList.Count == 0 || !IsWithinGrid(start) ||
            !IsWithinGrid(end) || //gridList[start.x].list[start.y] > 0 ||
            gridList[end.x].list[end.y] > 0)
        {
            return
                (false,
                    new Dictionary<Vector2Int, Vector2Int
                        ?>()); // Return false with an empty dictionary if start or end is not within the grid or is blocked
        }

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int?> predecessors = new Dictionary<Vector2Int, Vector2Int?>();

        queue.Enqueue(start);
        predecessors[start] = null;

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (current == end)
            {
                return (true, predecessors); // Path found, return true with the predecessors
            }

            foreach (var neighbor in GetNeighbors(current))
            {
                if (!predecessors.ContainsKey(neighbor) && gridList[neighbor.x].list[neighbor.y] == 0)
                {
                    queue.Enqueue(neighbor);
                    predecessors[neighbor] = current;
                }
            }
        }

        return (false, predecessors); // No path found, return false with the predecessors
    }

    private bool IsWithinGrid(Vector2Int point)
    {
        return point.x >= 0 && point.x < width && point.y >= 0 && point.y < height;
    }

    private List<Vector2Int> GetNeighbors(Vector2Int point)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>
        {
            new Vector2Int(point.x - 1, point.y),
            new Vector2Int(point.x + 1, point.y),
            new Vector2Int(point.x, point.y - 1),
            new Vector2Int(point.x, point.y + 1)
        };

        neighbors.RemoveAll(p => !IsWithinGrid(p));
        return neighbors;
    }

    private List<Vector2Int> BuildPath(Dictionary<Vector2Int, Vector2Int?> predecessors, Vector2Int end)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        for (var p = end; predecessors[p].HasValue; p = predecessors[p].Value)
        {
            path.Add(p);
        }

        path.Reverse(); // Reverse the path to start from the beginning
        return path;
    }

    private void PrintPath(List<Vector2Int> path)
    {
        foreach (var point in path)
        {
            Debug.Log($"Point: {point.x}, {point.y}");
        }
    }

    public List<GameObject> GetGameObjectPath(List<Vector2Int> path)
    {
        List<GameObject> gameObjectPath = new List<GameObject>();
        foreach (var point in path)
        {
            // Assuming you have a method to get the GameObject at a specific grid coordinate
            GameObject gridObject = backgroundGrids[point.x].list[point.y];
            if (gridObject != null)
            {
                gameObjectPath.Add(gridObject);
            }
        }

        return gameObjectPath;
    }

    public void ClearAnElementFromList(int x, int y)
    {
        gridList[x].list[y] = 0;
    }

    public void AddStickmanFromGameList(StickmanParent stickmanParent)
    {
        if (!allStickmenInGame.Contains(stickmanParent))
        {
            allStickmenInGame.Add(stickmanParent);
        }

    }
    public void RemoveStickmanFromGameList(StickmanParent stickmanParent)
    {
        if (allStickmenInGame.Contains(stickmanParent))
        {
            allStickmenInGame.Remove(stickmanParent);
        }

        CheckOutlines();
    }

    private void CheckOutlines()
    {
        foreach (StickmanParent stickman in allStickmenInGame)
        {
            stickman.CheckOutline();
        }
    }
}

[System.Serializable]
public class IntList
{
    public List<int> list = new List<int>();
}


[System.Serializable]
public class GameObjectList
{
    public List<GameObject> list = new List<GameObject>();
}

[System.Serializable]
public class InGameWallList
{
    public List<InGameWall> list = new List<InGameWall>();
}