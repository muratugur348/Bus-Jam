using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using BusJam.Scripts.Runtime;
using BusJam.Scripts.Runtime.Managers.GameManager;
using BusJam.Scripts.Runtime.Managers.HapticManager;
using BusJam.Scripts.Runtime.Managers.SoundManager;
using UnityEngine;
using Random = UnityEngine.Random;
using BusJam.Scripts.Runtime.LevelCreator;

public class StickmanParent : MonoBehaviour
{
    public int x, y;
    public bool isOutlineOpened;

    public float speed;
    public float rotationSpeed;
    public float moveMatchAreaSpeed;
    public float moveBusTime;

    public float rotateValueOnWrongClick;
    public float rotateTimeOnWrongClick;

    public bool isBlockedToClick;
    public bool isQuestionMark;
    public GameObject questionMarkFace;

    public bool mySpawner;

    public ColorType colorType;
    public Material currentMaterial;
    public Material darkMaterial;
    public Material questionMarkMaterial;
    public bool isMovingToMatchArea;
    public GameColors gameColors;

    public Collider myCollider;
    public SkinnedMeshRenderer mySkinnedMeshRenderer;
    public Animator myAnimator;

    public GridManager gridManager;

    private IHapticManager _hapticManager;
    private ISoundManager _soundManager;


    public void Init(ColorType ColorType, Material startMaterial, int x, int y, bool isQuestionMark, GridManager gridManager)
    {
        this.x = x;
        this.y = y;
        this.isQuestionMark = isQuestionMark;
        this.gridManager = gridManager;
        colorType = ColorType;
        currentMaterial = startMaterial;

        if (this.isQuestionMark)
        {
            mySkinnedMeshRenderer.material = questionMarkMaterial;

            gridManager.AddQuestionMarkedStickman(this);
            questionMarkFace.SetActive(true);
        }

    }

    private void Start()
    {
        _hapticManager = Locator.Instance.Resolve<IHapticManager>();
        _soundManager = Locator.Instance.Resolve<ISoundManager>();

        transform.position = new Vector3(transform.position.x, 0.01f, transform.position.z);
        GridManager.Instance.AddStickmanFromGameList(this);
        List<Material> activeMaterials = gameColors.ActiveMaterials.ToList();

        int index = activeMaterials.IndexOf(currentMaterial);
        darkMaterial = gameColors.DeactiveMaterials[index];

    }
    public void Selected()
    {
        if (isBlockedToClick)
            return;

        _hapticManager.TriggerHaptic(HapticType.LightImpact);
        _soundManager.PlaySound("select");

        int nearestXValue = 0;
        GameObject availableMatchGrid = GridManager.Instance.GetAvailableGrid();

        if (availableMatchGrid == null)
        {
            ShakeOnWrongClick();
            return;
        }

        float distance = 999;

        for (int i = 0; i < GridManager.Instance.backgroundGrids.Count; i++)
        {
            GameObject go = GridManager.Instance.backgroundGrids[i].list[GridManager.Instance.height - 1];
            if (Vector3.Distance(go.transform.position, availableMatchGrid.transform.position) < distance)
            {
                distance = Vector3.Distance(go.transform.position, availableMatchGrid.transform.position);
                nearestXValue = i;
            }
        }

        bool isShortestPathFound;
        Dictionary<Vector2Int, Vector2Int?> pathDict;
        (isShortestPathFound, pathDict) = GridManager.Instance.FindShortestPath(new Vector2Int(x, y),
            new Vector2Int(nearestXValue, GridManager.Instance.height - 1));

        if (!isShortestPathFound)
        {
            ColorWithRedOutline();
            ShakeOnWrongClick();
            return;
        }

        myCollider.enabled = false;

        transform.DOKill();
        transform.DOLocalRotate(Vector3.zero, rotateTimeOnWrongClick);

        transform.SetParent(availableMatchGrid.transform);
        GridManager.Instance.AddStickmanToMatchAreaList(this);

        List<Vector2Int> path = ExtractPath(new Vector2Int(x, y),
            new Vector2Int(nearestXValue, GridManager.Instance.height - 1),
            pathDict);
        List<GameObject> gameObjectPath = GridManager.Instance.GetGameObjectPath(path);
        GridManager.Instance.ClearAnElementFromList(x, y);
        StartCoroutine(MoveAlongGameObjectPath(gameObject,
            gameObjectPath));

        GridManager.Instance.RemoveStickmanFromGameList(this);

        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.IncreaseTutorialPhaseValue();
        }

        CloseOutline();
    }

    private void ShakeOnWrongClick()
    {
        transform.DOKill();
        transform.DOLocalRotate(new Vector3(0, 0, rotateValueOnWrongClick), rotateTimeOnWrongClick / 2).OnComplete(() =>
        {
            transform.DOLocalRotate(new Vector3(0, 0, -rotateValueOnWrongClick), rotateTimeOnWrongClick)
                .OnComplete(() => { transform.DOLocalRotate(new Vector3(0, 0, 0), rotateTimeOnWrongClick / 2); });
        });
    }

    public void ColorWithRedOutline()
    {
        mySkinnedMeshRenderer.material.SetFloat("_OutlineSize", 75);
        mySkinnedMeshRenderer.material.SetColor("_OutlineColor", Color.red);
        float outlineSize = 75;
        DOTween.To(() => outlineSize, x => outlineSize = x, 0, 1f).OnUpdate(() =>
        {
            mySkinnedMeshRenderer.material.SetFloat("_OutlineSize", outlineSize);
        });
    }

    private List<Vector2Int> ExtractPath(Vector2Int start, Vector2Int end,
        Dictionary<Vector2Int, Vector2Int?> predecessors)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int? current = end;

        while (current.HasValue && current.Value != start)
        {
            path.Add(current.Value);
            current = predecessors[current.Value];
        }

        path.Add(start); // Add the start position
        path.Reverse(); // Reverse the list to start from the beginning

        return path;
    }

    IEnumerator MoveAlongGameObjectPath(GameObject objectToMove, List<GameObject> gameObjectPath)
    {
        transform.DOKill();
        isMovingToMatchArea = true;

        myAnimator.SetBool("isWalking", true);
        myAnimator.SetBool("isWaiting", false);

        gridManager.CheckSpawners(this);

        foreach (GameObject target in gameObjectPath)
        {
            if (target == null)
                continue;

            Vector3 targetPosition = target.transform.position;

            while (Vector3.Distance(objectToMove.transform.position, targetPosition) > 0.01f)
            {
                // Move towards the target
                objectToMove.transform.position = Vector3.MoveTowards(objectToMove.transform.position, targetPosition,
                    speed * Time.deltaTime);

                // Calculate the direction
                Vector3 direction = (targetPosition - objectToMove.transform.position).normalized;

                // Check if the direction is not zero before trying to rotate
                if (direction != Vector3.zero)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(direction);
                    objectToMove.transform.rotation = Quaternion.Slerp(objectToMove.transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
                }

                yield return null;
            }
        }
        myAnimator.SetBool("isWalking", false);
        myAnimator.SetBool("isWaiting", true);
        CheckTargetPositionAfterCloseAnimation();
    }



    private void CheckTargetPositionAfterCloseAnimation()
    {
        TryToGoMatchGrid();
    }

    private void MoveToBus()
    {
        StopAllCoroutines();
        transform.DOKill(false);
        myAnimator.SetBool("isWalking", true);
        myAnimator.SetBool("isWaiting", false);

        Bus bus = BusManager.Instance.activeBus;
        GameObject parentChild = bus.stickmanPositions[bus.currentStickmanAmount];
        transform.SetParent(parentChild.transform);
        /*transform.DOLocalJump(new Vector3(-0.025f, -0.3f, 0), 1, 1, moveBusTime);
        transform.DOLocalRotate(new Vector3(0, 0, 0), moveBusTime).OnComplete(() =>
        {
            myAnimator.SetBool("isWalking", false);
            myAnimator.SetBool("isWaiting", false);
            myAnimator.SetBool("isSitting", true);
        });*/
        transform.DOScale(Vector3.one * 0.8f, moveBusTime / 2).SetDelay(moveBusTime / 2);
        transform.DOLookAt(bus.doorEnterPoint.transform.position, moveBusTime / 3);
        transform.DOMove(bus.doorEnterPoint.transform.position, moveBusTime).OnComplete(() =>
        {
            transform.localScale = Vector3.one * 0.3f;
            transform.localPosition = new Vector3(-0.11f, .512f, 0f);
            transform.localEulerAngles = new Vector3(0, 180, 0);
            transform.DOScale(Vector3.one * 2f, 0.25f).SetEase(Ease.OutBack);
            myAnimator.SetBool("isWalking", false);
            myAnimator.SetBool("isWaiting", false);
            myAnimator.SetBool("isSitting", true);
        });
        bus.AddStickman(this);

        GridManager.Instance.RemoveStickmanFromMatchAreaList(this);

    }

    public bool IsStickmanCanMoveToBus()
    {

        if (BusManager.Instance.CheckStickmanCanGoToBus(colorType))
        {
            return true;
        }


        return false;
    }


    private void TryToGoMatchGrid()
    {
        transform.SetParent(null);
        if (GridManager.Instance.IsThereAnyAvailableMatchGrid())
        {
            myAnimator.SetBool("isWalking", true);
            myAnimator.SetBool("isWaiting", false);

            GameObject matchGrid = GridManager.Instance.GetAvailableGrid();
            transform.SetParent(matchGrid.transform);
            //GridManager.Instance.ReorderMatchAreaJustNoParents();
            transform.DOMove(matchGrid.transform.position,
                    Vector3.Distance(transform.position, matchGrid.transform.position) / moveMatchAreaSpeed)
                .SetEase(Ease.Linear).OnComplete(
                    () =>
                    {
                        myAnimator.SetBool("isWalking", false);
                        myAnimator.SetBool("isWaiting", true);

                        isMovingToMatchArea = false;
                        GridManager.Instance.HighlightTile(GridManager.Instance.matchAreaList.IndexOf(matchGrid));
                        TryToGoBusFromMatchArea();
                        //---LEVEL FAILED---//
                        //1- If there is no more space in match area, level must fail
                

                        GridManager.Instance.CheckLevelFailed();
                    });


            //
        }
    }


    public void TryToGoBusFromMatchArea()
    {
        if (BusManager.Instance.CheckStickmanCanGoToBus(colorType))
        {
            MoveToBus();
        }

    }

    public void CloseOutline()
    {
        mySkinnedMeshRenderer.material.SetFloat("_OutlineSize", 0);
    }

    public void OpenOutline()
    {
        isOutlineOpened = true;
        mySkinnedMeshRenderer.material = currentMaterial;
        mySkinnedMeshRenderer.material.SetColor("_OutlineColor", Color.black);
        mySkinnedMeshRenderer.material.SetFloat("_OutlineSize", 50);
    }

    public void MakeItDark()
    {
        mySkinnedMeshRenderer.material = darkMaterial;
    }



    public void CheckOutline()
    {
        bool isShortestPathFound;
        Dictionary<Vector2Int, Vector2Int?> pathDict;
        (isShortestPathFound, pathDict) = GridManager.Instance.FindShortestPath(new Vector2Int(x, y),
            new Vector2Int(0, GridManager.Instance.height - 1));

        if (!isShortestPathFound)
        {
            if (isQuestionMark)
                return;
            MakeItDark();
        }
        else
        {
            if (isQuestionMark)
            {
                isQuestionMark = false;
                questionMarkFace.SetActive(false);
            }
            OpenOutline();
        }
    }
}