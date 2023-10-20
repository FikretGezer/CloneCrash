using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemMovementSecond : MonoBehaviour
{
    [SerializeField] private float itemMovingSpeed = 1f;

    private Camera mainCam;
    private float current, target;
    private Vector3 tempStartPos, tempEndPos;
    
    private Vector3 startPos, endPos;
    private GameObject startObject, endObject;
    private int x, y;
    private int endY, endX;
    public bool ObjectsMoving { get; private set; }

    public static ItemMovementSecond Instance;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        mainCam = Camera.main;
        target = 1f;
    }
    private void Update()
    {
        var pos = mainCam.ScreenPointToRay(Input.mousePosition);
        if(!ObjectsMoving)
        {            
            if (Input.GetMouseButtonDown(0))
            {
                startPos = pos.origin;
                Collider2D[] col = Physics2D.OverlapCircleAll(startPos, 0.1f);               

                if (col.Length > 0)
                {
                    startObject = col[0].gameObject;
                    if (startObject != null)
                    {
                        var coordinates = startObject.name.Split(',');
                        x = Convert.ToInt32(coordinates[0]);
                        y = Convert.ToInt32(coordinates[1]);
                    }
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                endPos = pos.origin;
                var dir = (endPos - startPos).normalized;
                var angle = DirToAngleDeg(dir);
                if (startObject != null)
                {
                    var xLength = ItemPlacement._instance.placedObjects.GetLength(0);
                    var yLength = ItemPlacement._instance.placedObjects.GetLength(1);
                    if ((angle < 45f && angle >= 0f) || (angle > 315f && angle < 360f))
                    {
                        if (x + 1 < xLength && ItemPlacement._instance.placedObjects[x + 1, y] != null)
                        {
                            SetEndObject(x + 1, y);
                        }
                    }
                    else if (angle > 45f && angle < 135f)
                    {
                        if (y + 1 < yLength && ItemPlacement._instance.placedObjects[x, y + 1] != null)
                        {
                            SetEndObject(x, y + 1);
                        }
                    }
                    else if (angle > 135f && angle < 225f)
                    {
                        if (x - 1 >= 0 && ItemPlacement._instance.placedObjects[x - 1, y] != null)
                        {
                            SetEndObject(x - 1, y);
                        }
                    }
                    else if (angle > 225f && angle < 315f)
                    {
                        if (y - 1 >= 0 && ItemPlacement._instance.placedObjects[x, y - 1] != null)
                        {
                            SetEndObject(x, y - 1);
                        }
                    }
                    if(endObject != null)
                        SetValuesForMovement();                        
                }                
            }
        }
        if (startObject != null && endObject != null)
        {
            ObjectsMoving = true;           
            MoveThem();
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(startPos, endPos);
    }
    private void CheckLines()
    {
        var placeObjs = ItemPlacement._instance.placedObjects;
        Sprite currentItem = null;

        List<GameObject> objs = new List<GameObject>();
        int currentIndex = 0;
        int count = 0;

        for (int y = 0; y < placeObjs.GetLength(1); y++)
        {
            for (int x = 0; x < placeObjs.GetLength(0); x++)
            {
                if(currentItem == null)
                {
                    currentItem = placeObjs[x, y].GetComponent<SpriteRenderer>().sprite;
                    objs.Add(placeObjs[x, y]);
                    currentIndex = 0;
                    count = 1;
                }
                else
                {
                    if (currentItem == placeObjs[x,y].GetComponent<SpriteRenderer>().sprite)
                    {
                        objs.Add(placeObjs[x, y]);
                        count++;
                    }
                    else
                    {

                    }
                    currentIndex++;
                }
            }
        }
    }
    private void SetEndObject(int xSet, int ySet)
    {
        endX = xSet;
        endY = ySet;
        endObject = ItemPlacement._instance.placedObjects[endX, endY];
    }
    public void SetValuesForMovement()
    {
        current = 0;

        tempStartPos = startObject.transform.position;
        tempEndPos = endObject.transform.position;

        ItemPlacement._instance.placedObjects[x, y] = endObject;
        ItemPlacement._instance.placedObjects[endX, endY] = startObject;

        ItemPlacement._instance.placementAvailablity[x, y] = 1;
        ItemPlacement._instance.placementAvailablity[endX, endY] = 1;

        var tempName = startObject.name;
        startObject.name = endObject.name;
        endObject.name = tempName;
    }
    private void MoveThem()
    {      
        current = Mathf.MoveTowards(current, target, itemMovingSpeed * Time.deltaTime);

        startObject.transform.position = Vector3.Lerp(startObject.transform.position, tempEndPos, current);
        endObject.transform.position = Vector3.Lerp(endObject.transform.position, tempStartPos, current);
        
        if(current == target)
        {
            //if (changesDone)
            //{
            //    ObjectsMoving = false;
            //    startObject = endObject = null;
            //    changesDone = false;
            //}
            //else
            //{
            //    SetValuesForMovement();
            //}
            ObjectsMoving = false;
            startObject = endObject = null;
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
