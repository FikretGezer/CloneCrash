using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemMovement : MonoBehaviour
{
    [SerializeField] private float itemMovingSpeed = 1f;

    private Camera mainCam;
    private float current, target;
    private Vector3 tempStartPos, tempEndPos;
    
    private Vector3 startPos, endPos;
    private GameObject startObject, endObject;
    private List<GameObject> colLook = new List<GameObject>();
    private int x, y;
    private int endY, endX;
    private bool objectsMoving;
    private void Awake()
    {
        mainCam = Camera.main;
        target = 1f;
    }
    private void Update()
    {
        var pos = mainCam.ScreenPointToRay(Input.mousePosition);
        if(!objectsMoving)
        {
            
            if (Input.GetMouseButtonDown(0))
            {
                startPos = pos.origin;
                Collider2D[] col = Physics2D.OverlapCircleAll(startPos, 0.1f);
                colLook.Clear();

                if (col.Length > 0)
                {
                    startObject = col[0].gameObject;
                    if (startObject != null)
                    {
                        var coordinates = startObject.name.Split(',');
                        x = Convert.ToInt32(coordinates[0]);
                        y = Convert.ToInt32(coordinates[1]);
                        colLook.Add(startObject);
                        //Debug.Log(coordinates[0] + "-" + coordinates[1]);
                    }
                }
                //Debug.Log(ItemPlacement._instance.placementAvailablity[x,y]);

            }
            if (Input.GetMouseButtonUp(0))
            {
                endPos = pos.origin;
                var dir = (endPos - startPos).normalized;
                var angle = DirToAngleDeg(dir);                
                if (startObject != null)
                {
                    var xLength = ItemPlacement._instance.placementAvailablity.GetLength(0);
                    var yLength = ItemPlacement._instance.placementAvailablity.GetLength(1);
                    if ((angle < 45f && angle >= 0f) || (angle > 315f && angle < 360f))
                    {
                        #region First Method
                        if (x + 1 < xLength && ItemPlacement._instance.placementAvailablity[x + 1, y] != 2)
                        {

                            MovementCheck(1.28f, 0f);
                            endX = x + 1;                           
                        }
                        #endregion
                        #region Second Method
                        //var xLength = ItemPlacement._instance.placedObjects.GetLength(0);
                        //var yLength = ItemPlacement._instance.placedObjects.GetLength(1);
                        //if (x + 1 < xLength && ItemPlacement._instance.placedObjects[x + 1, y] != null)
                        //{
                        //    endObject = ItemPlacement._instance.placedObjects[x + 1, y];                        
                        //    Debug.Log(endObject.name);
                        //}
                        #endregion
                    }
                    else if (angle > 45f && angle < 135f)
                    {
                        if (y + 1 < yLength && ItemPlacement._instance.placementAvailablity[x, y + 1] != 2)
                        {
                            MovementCheck(0f, 1.28f);
                            endY = y + 1;                        
                        }
                    }
                    else if (angle > 135f && angle < 225f)
                    {
                        if (x - 1 >= 0 && ItemPlacement._instance.placementAvailablity[x - 1, y] != 2)
                        {
                            MovementCheck(-1.28f, 0f);
                            endX = x - 1;
                        }
                    }
                    else if (angle > 225f && angle < 315f)
                    {
                        if (y - 1 >= 0 && ItemPlacement._instance.placementAvailablity[x, y - 1] != 2)
                        {
                            MovementCheck(0f, -1.28f);
                            endY = y - 1;
                        }
                    }
                    if (endObject != null)
                    {
                        ResetObjectsMovement();
                    }
                        
                }
                
            }
        }
        if (startObject != null && endObject != null)
        {
            objectsMoving = true;

            var temp = ItemPlacement._instance.placedObjects[x, y];
            ItemPlacement._instance.placedObjects[x, y] = ItemPlacement._instance.placedObjects[endX, endY];
            ItemPlacement._instance.placedObjects[endX, endY] = temp;

            MoveThem();
        }
    }
    private void MovementCheck(float xValue, float yValue)
    {
        var endObjPos = (Vector2)startObject.transform.position + new Vector2(xValue, yValue);
        Collider2D[] col = Physics2D.OverlapCircleAll(endObjPos, 0.1f);
        endObject = col[0].gameObject;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(startPos, endPos);
    }
    private void ResetObjectsMovement()
    {
        current = 0f;
        tempStartPos = startObject.transform.position;
        tempEndPos = endObject.transform.position;

        var tempName = startObject.name;
        startObject.name = endObject.name;
        endObject.name = tempName;
    }
    bool tryAgain;
    private void MoveThem()
    {      
        current = Mathf.MoveTowards(current, target, itemMovingSpeed * Time.deltaTime);

        startObject.transform.position = Vector3.Lerp(startObject.transform.position, tempEndPos, current);
        endObject.transform.position = Vector3.Lerp(endObject.transform.position, tempStartPos, current);
        
        if(current == target)
        {
            CheckLines();
            //if(tryAgain)
            //{
            //    objectsMoving = false;
            //    startObject = endObject = null;
            //    tryAgain = false;
            //}
            //else
            //{
            //    ResetObjectsMovement();
            //    tryAgain = true;
            //}
            objectsMoving = false;
            startObject = endObject = null;
        }
    }
    private void CheckLines()
    {
        var objList = ItemPlacement._instance.placedObjects;
        var xLength = objList.GetLength(0);
        var yLength = objList.GetLength(1);

        List<List<int>> indices = new List<List<int>>();
        Sprite currentImage = null;

        for (int yCur = 0; yCur < yLength; yCur++)
        {
            int countX = 0;
            int startIndex = 0;
            for (int xCur = 0; xCur < xLength; xCur++)
            {
                if (objList[xCur, yCur] == null)
                {
                    currentImage = null;
                    continue;
                }
                if (currentImage == null)//Assign the sprite that is gonna be controlled
                {                    
                    currentImage = objList[xCur, yCur].GetComponent<SpriteRenderer>().sprite;
                    startIndex = xCur;
                    countX = 1;
                }
                else // Check if there is a trio or more as the current item
                {
                    if(currentImage == objList[xCur, yCur].GetComponent<SpriteRenderer>().sprite)
                    {
                        countX++;
                    }
                    else
                    {
                        currentImage = null;
                        Debug.Log($"Iteration: {xCur}{yCur} - Count: {countX}");
                        if (countX >= 3)
                        {
                            for (int i = 0; i < countX; i++)
                            {
                                indices.Add(new List<int> { startIndex, yCur });
                                startIndex++;
                            }
                        }
                    }
                }
            }            
        }
        for (int i = 0; i < indices.Count; i++)
        {
            int xInd = indices[i][0];
            int yInd = indices[i][1];
            Destroy(ItemPlacement._instance.placedObjects[xInd, yInd]);
        }
    }
    private float DirToAngleDeg(Vector2 v)
    {
        var angle = Mathf.Rad2Deg * Mathf.Atan2(v.y, v.x);
        if (angle < 0f)
        {
            angle += 360f;
        }
        return angle;
    }
}
