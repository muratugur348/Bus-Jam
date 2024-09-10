using DG.Tweening;
using System.Collections.Generic;
using BusJam.Scripts.Runtime.LevelCreator;
using TMPro;
using UnityEngine;


public class Spawner : MonoBehaviour
{
    public int x, y;
    public Vector2Int myTargetIndexes;
    public List<StickmanParent> stickmenToSpawn = new(); // 0th first 
    public Vector2Int spawnTargetIndex;
    public GridDirectionType myDirectionType;
    public GameObject spawnPosition;
    [Header("Spawner Parameters")] public float spawnedStickmanMoveTime;
    public float spawnerSizeGetBigTime;
    public float spawnerSizeGetSmallTime;
    public Vector3 spawnerSizeChangeValue;


    [Header("Counter Parameters")] public TextMeshPro myCounter;

    private Vector3 normalSpawnerSize;
    private GridManager _gridManager;


    private void Start()
    {
        _gridManager = GridManager.Instance;
        normalSpawnerSize = transform.localScale;


        foreach (StickmanParent stickman in stickmenToSpawn)
        {
            stickman.mySpawner = this;
        }
    }

    public void Init(int x, int y, Vector2Int spawnTargetIndex, List<StickmanParent> stickmenToSpawn,
        GridDirectionType directionType, Vector2Int targetIndexes)
    {
        this.x = x;
        this.y = y;
        this.myTargetIndexes = targetIndexes;
        this.spawnTargetIndex = spawnTargetIndex;
        this.stickmenToSpawn.AddRange(stickmenToSpawn);
        myDirectionType = directionType;
        SetCounter();
        SetCounterRotation();

        FindObjectOfType<GridManager>().Fill(new Vector2Int(x, y), -2);
    }

    public void CheckToSpawn(StickmanParent sourceStickmanParent)
    {
        if (new Vector2Int(sourceStickmanParent.x, sourceStickmanParent.y) == myTargetIndexes)
            SpawnStickman();
    }
    public void SpawnStickman()
    {
        if (stickmenToSpawn.Count == 0) return;

        StickmanParent stickman = stickmenToSpawn[0];
        stickman.isBlockedToClick = true;
        stickmenToSpawn.RemoveAt(0);
        Vector3 originalScale = Vector3.one;
        stickman.x = myTargetIndexes.x;
        stickman.y = myTargetIndexes.y;
        stickman.transform.localScale = Vector3.zero;
        stickman.transform.eulerAngles = new Vector3(0, 180, 0);
        stickman.gameObject.SetActive(true);
        stickman.transform.SetParent(null);
        _gridManager.Fill(myTargetIndexes, 1);
        if (stickman.isQuestionMark)
        {
            _gridManager.Fill(new Vector2Int(myTargetIndexes.x, myTargetIndexes.y), -3);
            _gridManager.AddQuestionMarkedStickman(stickman);
        }
        else
        {
            _gridManager.Fill(new Vector2Int(myTargetIndexes.x, myTargetIndexes.y), 1);

        }

        Sequence spawnSeq = DOTween.Sequence();

        spawnSeq.Append(transform.DOScale(spawnerSizeChangeValue, spawnerSizeGetBigTime));
        spawnSeq.Append(stickman.transform.DOLocalMove(_gridManager.backgroundGrids[myTargetIndexes.x].list[myTargetIndexes.y].transform.position, spawnedStickmanMoveTime));
        spawnSeq.Join(stickman.transform.DOLocalRotate(Vector3.zero, spawnedStickmanMoveTime));
        spawnSeq.Join(stickman.transform.DOScale(originalScale, spawnedStickmanMoveTime * 0.85f));
        spawnSeq.Join(transform.DOScale(normalSpawnerSize, spawnerSizeGetSmallTime));
        spawnSeq.AppendCallback(() => stickman.isBlockedToClick = false);
        stickman.OpenOutline();

        /*transform.DOScale(spawnerSizeChangeValue, spawnerSizeChangeTime).OnComplete(() =>
        {
            jello.transform.DOLocalMove(Vector3.zero, spawnedJelloMoveTime);
            jello.transform.DOScale(originalScale, spawnedJelloMoveTime * 0.85f);
            transform.DOScale(normalSpawnerSize, spawnerSizeChangeTime);
        });*/
        SetCounter();


    }

    public void SpawnAnimation()
    {
        Sequence spawnSeq = DOTween.Sequence();

        spawnSeq.Append(transform.DOScale(spawnerSizeChangeValue, spawnerSizeGetBigTime));
        spawnSeq.Append(transform.DOScale(normalSpawnerSize, spawnerSizeGetSmallTime));
        SetCounter();

    }


    private void SetCounter()
    {
        if (myCounter)
            myCounter.text = stickmenToSpawn.Count.ToString();
    }

    public void SetCounterRotation()
    {
        if (myCounter)
            myCounter.transform.eulerAngles = new Vector3(90, 0, 0);
    }


}