using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ItemMovementSecond : MonoBehaviour
{
    [SerializeField] private float itemMovingSpeed = 1f;

    private Camera mainCam;
    private float current, target;
    private Vector3 startObjTargetPos, endObjTargetPos;
    
    private Vector3 startPos, endPos;
    private GameObject startObject, endObject;
    private int x, y;
    private int endY, endX;
    private int xLength, yLength;
    public bool ObjectsMoving { get; private set; }

    public static ItemMovementSecond Instance;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        mainCam = Camera.main;
        target = 1f;
    }
    private void Start()
    {
        xLength = ItemPlacement._instance.placedObjects.GetLength(0);
        yLength = ItemPlacement._instance.placedObjects.GetLength(1);
    }
    private void Update()
    {
        //Get mouse position
        var pos = mainCam.ScreenPointToRay(Input.mousePosition);

        if(!ObjectsMoving)
        {            
            if (Input.GetMouseButtonDown(0))
            {
                resetCount = 0;
                testPassed = false;

                //Set mouse pos for detecting startObject 
                startPos = pos.origin;
                //Check is there any object at the position that is clicked
                Collider2D[] col = Physics2D.OverlapCircleAll(startPos, 0.1f);

                //If there is set it as a startObjet
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
                endPos = pos.origin;//Set mouse pos for detecting endObject

                //Detect movement angle
                var dir = (endPos - startPos).normalized;
                var angle = DirToAngleDeg(dir);

                if (startObject != null)
                {                    
                    if ((angle < 45f && angle >= 0f) || (angle > 315f && angle < 360f)) //Finger movement to the RIGHT
                    {
                        if (x + 1 < xLength && ItemPlacement._instance.placedObjects[x + 1, y] != null)
                        {
                            AssignEndObject(x + 1, y);
                        }
                    }
                    else if (angle > 45f && angle < 135f)//Finger movement to the UP
                    {
                        if (y + 1 < yLength && ItemPlacement._instance.placedObjects[x, y + 1] != null)
                        {
                            AssignEndObject(x, y + 1);
                        }
                    }
                    else if (angle > 135f && angle < 225f)//Finger movement to the LEFT
                    {
                        if (x - 1 >= 0 && ItemPlacement._instance.placedObjects[x - 1, y] != null)
                        {
                            AssignEndObject(x - 1, y);
                        }
                    }
                    else if (angle > 225f && angle < 315f)//Finger movement to the DOWN
                    {
                        if (y - 1 >= 0 && ItemPlacement._instance.placedObjects[x, y - 1] != null)
                        {
                            AssignEndObject(x, y - 1);
                        }
                    }
                    if(endObject != null)
                    {
                        SetLerpValues();
                        ObjectsMoving = true;
                    }
                }                
            }
        }
        if (startObject != null && endObject != null && ObjectsMoving)
        {            
            MoveSelectedObjects();
        }
    }    
    private void AssignEndObject(int xSet, int ySet) //Set endObject and its indexes for placeObject array
    {
        endX = xSet;
        endY = ySet;
        endObject = ItemPlacement._instance.placedObjects[endX, endY];
    }
    public void SetLerpValues()
    {
        //If lerp used previously reset it
        current = 0;

        //Set target posses for lerping
        startObjTargetPos = startObject.transform.position;
        endObjTargetPos = endObject.transform.position;

        //We detecting x and y using object's name so change them too
        var tempName = startObject.name;
        startObject.name = endObject.name;
        endObject.name = tempName;

        //Exchange placement in placedObject array
        //RearrangeObjectArray();
    }
    public void RearrangeObjectArray()
    {
        ItemPlacement._instance.placedObjects[x, y] = endObject;
        ItemPlacement._instance.placedObjects[endX, endY] = startObject;

        //ItemPlacement._instance.placementAvailablity[x, y] = 1;
        //ItemPlacement._instance.placementAvailablity[endX, endY] = 1;
    }
    //Let item script know items movement done
    public bool LerpingDone;
    public bool testPassed;
    private int resetCount = 0;
    private void MoveSelectedObjects()
    {        
        current = Mathf.MoveTowards(current, target, itemMovingSpeed * Time.deltaTime);

        startObject.transform.position = Vector3.Lerp(startObject.transform.position, endObjTargetPos, current);
        endObject.transform.position = Vector3.Lerp(endObject.transform.position, startObjTargetPos, current);
        
        if(current == target)
        {
            
            if(resetCount < 1)
            {
                resetCount++;
                LerpingDone = true;
                //startObject.GetComponent<ItemScript>().Check();
                //endObject.GetComponent<ItemScript>().Check();
                //startObject.GetComponent<ItemScript>().objectSelected = true;
                //endObject.GetComponent<ItemScript>().objectSelected = true;                

                endObject.GetComponent<ItemScript>().CheckLines();
            }
            //else if(resetCount == 1)
            //{
            //    if (!LerpingDone)
            //    {
            //        if(testPassed)//Match found
            //        {
            //            RearrangeObjectArray();
            //            ObjectsMoving = false;
            //            startObject = endObject = null;
            //        }
            //        else //Match not found
            //        {
            //            SetLerpValues();
            //            resetCount++;
            //        }
            //    }
            //}
            //else
            //{
            //    ObjectsMoving = false;
            //    startObject = endObject = null;
            //}
        }
    }
    
    private float DirToAngleDeg(Vector2 v)//Returns the angle to check the direction of movement
    {
        var angle = Mathf.Rad2Deg * Mathf.Atan2(v.y, v.x);
        if (angle < 0f)
        {
            angle += 360f;
        }
        return angle;
    }
}
