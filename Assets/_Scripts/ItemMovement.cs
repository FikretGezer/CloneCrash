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
    private bool isDrawing = true;
    private List<GameObject> colLook = new List<GameObject>();
    private int x, y;
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
                    isDrawing = false;
                    var xLength = ItemPlacement._instance.placementAvailablity.GetLength(0);
                    var yLength = ItemPlacement._instance.placementAvailablity.GetLength(1);
                    if ((angle < 45f && angle >= 0f) || (angle > 315f && angle < 360f))
                    {
                        #region First Method
                        if (x + 1 < xLength && ItemPlacement._instance.placementAvailablity[x + 1, y] != 2)
                        {
                            x = x + 1;
                            //var endObjPos = (Vector2)startObject.transform.position + new Vector2(1.28f, 0f);
                            //Collider2D[] col = Physics2D.OverlapCircleAll(endObjPos, 0.1f);
                            //endObject = col[0].gameObject;                            
                            MovementCheck(1.28f, 0f);
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
                            y = y + 1;
                            //var endObjPos = (Vector2)startObject.transform.position + new Vector2(0f, 1.28f);
                            //Collider2D[] col = Physics2D.OverlapCircleAll(endObjPos, 0.1f);
                            //endObject = col[0].gameObject;                            
                            MovementCheck(0f, 1.28f);
                        }
                    }
                    else if (angle > 135f && angle < 225f)
                    {
                        if (x - 1 >= 0 && ItemPlacement._instance.placementAvailablity[x - 1, y] != 2)
                        {
                            x = x - 1;
                            //var endObjPos = (Vector2)startObject.transform.position + new Vector2(-1.28f, 0f);
                            //Collider2D[] col = Physics2D.OverlapCircleAll(endObjPos, 0.1f);
                            //endObject = col[0].gameObject;
                            MovementCheck(-1.28f, 0f);
                        }
                    }
                    else if (angle > 225f && angle < 315f)
                    {
                        if (y - 1 >= 0 && ItemPlacement._instance.placementAvailablity[x, y - 1] != 2)
                        {
                            y = y - 1;
                            //var endObjPos = (Vector2)startObject.transform.position + new Vector2(0f, -1.28f);
                            //Collider2D[] col = Physics2D.OverlapCircleAll(endObjPos, 0.1f);
                            //endObject = col[0].gameObject;
                            MovementCheck(0f, -1.28f);
                        }
                    }
                    if (endObject != null)
                    {
                        current = 0f;
                        tempStartPos = startObject.transform.position;
                        tempEndPos = endObject.transform.position;

                        var tempName = startObject.name;
                        startObject.name = endObject.name;
                        endObject.name = tempName;

                        //var tempIndex = 
                            //ItemPlacement._instance.placedObjects[x, y];
                        //ItemPlacement._instance.placedObjects[x, y] = 
                    }
                        
                }
                
            }
        }
        if (startObject != null && endObject != null)
        {
            isDrawing = true;
            objectsMoving = true;

            MoveThem();

            //var tempPos = startObject.transform.position;
            //var tempName = startObject.name;

            //startObject.transform.position = endObject.transform.position;
            //startObject.name = endObject.name;

            //endObject.transform.position = tempPos;
            //endObject.name = tempName;
            //startObject = endObject = null;
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
    bool tryAgain;
    private void MoveThem()
    {      
        current = Mathf.MoveTowards(current, target, itemMovingSpeed * Time.deltaTime);

        startObject.transform.position = Vector3.Lerp(startObject.transform.position, tempEndPos, current);
        endObject.transform.position = Vector3.Lerp(endObject.transform.position, tempStartPos, current);
        
        if(current == target)
        {            
            if(tryAgain)
            {
                objectsMoving = false;
                startObject = endObject = null;
                tryAgain = false;
            }
            else
            {
                current = 0f;
                tempStartPos = startObject.transform.position;
                tempEndPos = endObject.transform.position;

                var tempName = startObject.name;
                startObject.name = endObject.name;
                endObject.name = tempName;
                tryAgain = true;
            }
        }
    }
    private void CheckAround(GameObject obj)
    {
        var name = obj.name.Split(',');
        int xIndex = Convert.ToInt32(name[0]);
        int yIndex = Convert.ToInt32(name[1]);

        int xLength = ItemPlacement._instance.placementAvailablity.GetLength(0);
        int yLength = ItemPlacement._instance.placementAvailablity.GetLength(1);

        int xCount = 0;
        int yCount = 0;
        int lastIndexXRight = xIndex;
        int lastIndexXLeft = xIndex;
        
        for (int x = xIndex + 1; x < xLength; x++)
        {
            var currentObj = ItemPlacement._instance.placedObjects[x, yIndex];
            if (currentObj != null && currentObj.GetComponent<SpriteRenderer>().sprite == obj.GetComponent<SpriteRenderer>().sprite)
            {
                xCount++;
                lastIndexXRight = x;
            }
            else break;
        }
        for (int x = xIndex - 1; x > 0; x--)
        {
            var currentObj = ItemPlacement._instance.placedObjects[x, yIndex];
            if (currentObj != null && currentObj.GetComponent<SpriteRenderer>().sprite == obj.GetComponent<SpriteRenderer>().sprite)
            {
                xCount++;
                lastIndexXLeft = x;
            }
            else break;
        }
        if(xCount >= 3)
        {
            for (int x = lastIndexXLeft; x < lastIndexXRight; x++)
            {
                Destroy(ItemPlacement._instance.placedObjects[x, yIndex]);
            }
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
