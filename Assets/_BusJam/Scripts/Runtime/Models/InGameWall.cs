using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class InGameWall : MonoBehaviour
{
    public int x, y;
    public MeshRenderer meshRenderer;
    public GameObject alone, allFull, doubleSided, oneSided, threeSided, adjacentDoubleSided;
    public bool isRightFull = false, isLeftFull = false, isUpFull = false, isDownFull = false;
#if UNITY_EDITOR

    public void SetMesh(List<InGameWallList> gridObjectList)
    {
        transform.localPosition = new Vector3(transform.localPosition.x, 0, transform.localPosition.z);
        meshRenderer.enabled = false;

        if (x == 0)
            isLeftFull = false;
        if (y == 0)
            isDownFull = false;
        if (y + 1 == gridObjectList.Count)
            isUpFull = false;
        if (x + 1 == gridObjectList[0].list.Count)
            isRightFull = false;

        try
        {
            if (gridObjectList[y + 1].list[x] != null)
                isUpFull = true;
        }
        catch (Exception e)
        {
        }

        try
        {
            if (gridObjectList[y - 1].list[x] != null)
                isDownFull = true;
        }
        catch (Exception e)
        {
        }

        try
        {
            if (gridObjectList[y].list[x + 1] != null)
                isRightFull = true;
        }
        catch (Exception e)
        {
        }

        try
        {
            if (gridObjectList[y].list[x - 1] != null)
                isLeftFull = true;
        }
        catch (Exception e)
        {
        }


        if (isUpFull && isDownFull && isRightFull && isLeftFull)
            PrefabUtility.InstantiatePrefab(allFull, transform);
        else if (!isUpFull && !isDownFull && !isRightFull && !isLeftFull)
            PrefabUtility.InstantiatePrefab(alone, transform);
        else if (isUpFull && isDownFull && isRightFull && !isLeftFull)
        {
            PrefabUtility.InstantiatePrefab(oneSided, transform);
            transform.localEulerAngles = new Vector3(0, 180, 0);
        }
        else if (isUpFull && isDownFull && !isRightFull && isLeftFull)
        {
            PrefabUtility.InstantiatePrefab(oneSided, transform);
            transform.localEulerAngles = new Vector3(0, 0, 0);
        }
        else if (isUpFull && !isDownFull && isRightFull && isLeftFull)
        {
            PrefabUtility.InstantiatePrefab(oneSided, transform);
            transform.localEulerAngles = new Vector3(0, 90, 0);
        }
        else if (!isUpFull && isDownFull && isRightFull && isLeftFull)
        {
            PrefabUtility.InstantiatePrefab(oneSided, transform);
            transform.localEulerAngles = new Vector3(0, -90, 0);
        }
        else if (isUpFull && isDownFull && !isRightFull && !isLeftFull)
        {
            PrefabUtility.InstantiatePrefab(doubleSided, transform);
            transform.localEulerAngles = new Vector3(0, 0, 0);
        }
        else if (!isUpFull && !isDownFull && isRightFull && isLeftFull)
        {
            PrefabUtility.InstantiatePrefab(doubleSided, transform);
            transform.localEulerAngles = new Vector3(0, 90, 0);
        }
        else if (isUpFull && !isDownFull && !isRightFull && isLeftFull)
        {
            PrefabUtility.InstantiatePrefab(adjacentDoubleSided, transform);
            transform.localEulerAngles = new Vector3(0, 90, 0);
        }
        else if (isUpFull && !isDownFull && isRightFull && !isLeftFull)
        {
            PrefabUtility.InstantiatePrefab(adjacentDoubleSided, transform);
            transform.localEulerAngles = new Vector3(0, 180, 0);
        }
        else if (!isUpFull && isDownFull && !isRightFull && isLeftFull)
        {
            PrefabUtility.InstantiatePrefab(adjacentDoubleSided, transform);
            transform.localEulerAngles = new Vector3(0, 0, 0);
        }
        else if (!isUpFull && isDownFull && isRightFull && !isLeftFull)
        {
            PrefabUtility.InstantiatePrefab(adjacentDoubleSided, transform);
            transform.localEulerAngles = new Vector3(0, -90, 0);
        }
        else if (isUpFull && !isDownFull && !isRightFull && !isLeftFull)
        {
            PrefabUtility.InstantiatePrefab(threeSided, transform);
            transform.localEulerAngles = new Vector3(0, 180, 0);
        }
        else if (!isUpFull && isDownFull && !isRightFull && !isLeftFull)
        {
            PrefabUtility.InstantiatePrefab(threeSided, transform);
            transform.localEulerAngles = new Vector3(0, 0, 0);
        }
        else if (!isUpFull && !isDownFull && isRightFull && !isLeftFull)
        {
            PrefabUtility.InstantiatePrefab(threeSided, transform);
            transform.localEulerAngles = new Vector3(0, -90, 0);
        }
        else if (!isUpFull && !isDownFull && !isRightFull && isLeftFull)
        {
            PrefabUtility.InstantiatePrefab(threeSided, transform);
            transform.localEulerAngles = new Vector3(0, 90, 0);
        }

        return;
        if (x + 1 < gridObjectList.Count)
        {
        }
        else if (x + 1 >= gridObjectList.Count)
        {
            if (gridObjectList[x - 1].list[y] != null)
                isUpFull = true;
            if (gridObjectList[x].list[y + 1] != null)
                isRightFull = true;
            if (gridObjectList[x].list[y - 1] != null)
                isLeftFull = true;

            if (!isUpFull && !isRightFull && !isLeftFull)
                PrefabUtility.InstantiatePrefab(alone, transform);
            else if (isUpFull && isRightFull && isLeftFull)
            {
                PrefabUtility.InstantiatePrefab(oneSided, transform);
                transform.localEulerAngles = new Vector3(0, 0, 0);
            }
            else if (!isUpFull && isRightFull && isLeftFull)
            {
                PrefabUtility.InstantiatePrefab(doubleSided, transform);
                transform.localEulerAngles = new Vector3(0, 0, 0);
            }
            else if (isUpFull && !isRightFull && isLeftFull)
            {
                PrefabUtility.InstantiatePrefab(adjacentDoubleSided, transform);
                transform.localEulerAngles = new Vector3(0, 0, 0);
            }
            else if (isUpFull && isRightFull && !isLeftFull)
            {
                PrefabUtility.InstantiatePrefab(adjacentDoubleSided, transform);
                transform.localEulerAngles = new Vector3(0, 90, 0);
            }
            else if (isUpFull && !isRightFull && !isLeftFull)
            {
                PrefabUtility.InstantiatePrefab(threeSided, transform);
                transform.localEulerAngles = new Vector3(0, 90, 0);
            }
            else if (!isUpFull && isRightFull && !isLeftFull)
            {
                PrefabUtility.InstantiatePrefab(threeSided, transform);
                transform.localEulerAngles = new Vector3(0, 180, 0);
            }
            else if (!isUpFull && !isRightFull && isLeftFull)
            {
                PrefabUtility.InstantiatePrefab(threeSided, transform);
                transform.localEulerAngles = new Vector3(0, 0, 0);
            }
        }
    }

#endif
}