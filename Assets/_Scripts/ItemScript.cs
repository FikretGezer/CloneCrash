using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : MonoBehaviour
{
    private int xLength, yLength;
    private void Awake()
    {
        xLength = ItemPlacement._instance.placedObjects.GetLength(0);
        yLength = ItemPlacement._instance.placedObjects.GetLength(1);
    }
    private void LateUpdate()
    {
        if (ItemMovementSecond.Instance.LerpingDone)
        {
            ItemMovementSecond.Instance.LerpingDone = false;
            ItemMovementSecond.Instance.testPassed = false;
        }
    }
    public void Check()
    {
        
    }
    List<GameObject> objs = new List<GameObject>();
    public void CheckLines()
    {
        var placeObjs = ItemPlacement._instance.placedObjects;
        Sprite currentSprite = null;

        objs.Clear();
        
        int currentIndex = 0;        
        int count = 0;

        for (int y = 0; y < placeObjs.GetLength(1); y++)
        {
            for (int x = 0; x < placeObjs.GetLength(0); x++)
            {
                if (currentSprite == null)
                {
                    currentSprite = placeObjs[x, y].GetComponent<SpriteRenderer>().sprite;
                    objs.Add(placeObjs[x, y]);
                    if (objs.Count > 0)
                        currentIndex = objs.Count - 1;
                    else
                        currentIndex = 0;
                    count = 1;
                }
                else
                {
                    if (currentSprite == placeObjs[x, y].GetComponent<SpriteRenderer>().sprite)
                    {
                        objs.Add(placeObjs[x, y]);
                        count++;
                    }
                    else
                    {
                        if(count < 3 && objs.Count >= count)
                        {
                            objs.RemoveRange(currentIndex, count);
                        }
                        currentSprite = null;                        
                    }
                    currentIndex++;
                }
            }
        }
    }

}

//using System;
//using System.Collections.Generic;
//using UnityEngine;

//public class ItemScript : MonoBehaviour
//{
//    [SerializeField] private float itemMovingSpeed = 1f;

//    private int xLength;
//    private int yLength;

//    private float current;
//    private float target;

//    private Vector3 pos;
//    private bool canMoveable;

//    private bool canDeleteableHor;
//    private bool canDeleteableVer;

//    public bool objectSelected;
//    private void Awake()
//    {
//        current = 0f;
//        target = 1f;

//        xLength = ItemPlacement._instance.placedObjects.GetLength(0);
//        yLength = ItemPlacement._instance.placedObjects.GetLength(1);
//    }
//    private void Start()
//    {
//        CheckMatches();
//    }
//    private void Update()
//    {
//        //if (canMoveable)
//        //{
//        //    MoveIt();
//        //}
//    }
//    private void LateUpdate()
//    {
//        if (ItemMovementSecond.Instance.LerpingDone)
//        {
//            CheckMatches();
//            //MoveDown();
//        }

//    }
//    public void Check()
//    {
//        CheckMatches();
//        //MoveDown();
//    }
//    private void CheckMatches()
//    {
//        var currentObjs = ItemPlacement._instance.placedObjects;

//        var indexString = gameObject.name.Split(',');
//        var x = Convert.ToInt32(indexString[0]);
//        var y = Convert.ToInt32(indexString[1]);

//        //Checks horizontally
//        if (x - 1 >= 0 && x + 1 < xLength &&
//            currentObjs[x - 1, y] != null && currentObjs[x + 1, y] != null)
//        {
//            var firstSprite = currentObjs[x - 1, y].GetComponent<SpriteRenderer>();
//            var secondSprite = currentObjs[x + 1, y].GetComponent<SpriteRenderer>();
//            var currentSprite = GetComponent<SpriteRenderer>();

//            if (firstSprite.sprite == currentSprite.sprite && currentSprite.sprite == secondSprite.sprite)
//            {

//                firstSprite.color = Color.black;
//                secondSprite.color = Color.black;
//                currentSprite.color = Color.black;

//                //Destroy(firstSprite.gameObject);
//                //Destroy(secondSprite.gameObject);
//                //Destroy(gameObject);

//                canDeleteableHor = true;
//            }
//            else
//            {
//                canDeleteableHor = false;
//            }

//        }

//        //Checks Vertically
//        if (y - 1 >= 0 && y + 1 < yLength &&
//            currentObjs[x, y - 1] != null && currentObjs[x, y + 1] != null)
//        {
//            var firstSprite = currentObjs[x, y - 1].GetComponent<SpriteRenderer>();
//            var secondSprite = currentObjs[x, y + 1].GetComponent<SpriteRenderer>();
//            var currentSprite = GetComponent<SpriteRenderer>();

//            if (firstSprite.sprite == currentSprite.sprite && currentSprite.sprite == secondSprite.sprite)
//            {
//                firstSprite.color = Color.black;
//                secondSprite.color = Color.black;
//                currentSprite.color = Color.black;

//                //Destroy(firstSprite.gameObject);
//                //Destroy(secondSprite.gameObject);
//                //Destroy(gameObject);

//                canDeleteableVer = true;

//            }
//            else
//            {
//                canDeleteableVer = false;
//            }           
//        }
//        ItemMovementSecond.Instance.testPassed = true;
//        ItemMovementSecond.Instance.LerpingDone = false;
//        //if(objectSelected)
//        //{
//        //    if (canDeleteableHor || canDeleteableVer)
//        //    {
//        //        ItemMovementSecond.Instance.testPassed = true;
//        //        ItemMovementSecond.Instance.LerpingDone = false;
//        //    }
//        //    else
//        //    {
//        //        ItemMovementSecond.Instance.testPassed = false;
//        //        ItemMovementSecond.Instance.LerpingDone = false;
//        //    }
//        //    objectSelected = false;
//        //}

//    }

//    private void MoveDown()
//    {
//        var currentObjs = ItemPlacement._instance.placedObjects;
//        var currentAvailablity = ItemPlacement._instance.placementAvailablity;

//        var indexString = gameObject.name.Split(',');
//        var x = Convert.ToInt32(indexString[0]);
//        var y = Convert.ToInt32(indexString[1]);

//        current = 0f;

//        if (y - 1 >= 0)
//        {
//            if (currentAvailablity[x, y - 1] != 2 && currentAvailablity[x, y - 1] != 1)
//            {
//                gameObject.name = $"{x},{y - 1}";

//                currentObjs[x, y] = null;
//                currentObjs[x, y - 1] = gameObject;

//                pos = gameObject.transform.position;
//                pos = new Vector2(pos.x, pos.y - 1.3f);

//                canMoveable = true;
//                //gameObject.transform.position = pos;
//            }
//            else if (x - 1 >= 0 && currentAvailablity[x - 1, y - 1] != 2 && currentAvailablity[x - 1, y - 1] != 1)
//            {
//                gameObject.name = $"{x - 1},{y - 1}";

//                currentObjs[x, y] = null;
//                currentObjs[x - 1, y - 1] = gameObject;

//                pos = gameObject.transform.position;
//                pos = new Vector2(pos.x - 1.3f, pos.y - 1.3f);

//                canMoveable = true;
//                //gameObject.transform.position = pos;
//            }
//            else if (x + 1 < xLength && currentAvailablity[x + 1, y - 1] != 2 && currentAvailablity[x + 1, y - 1] != 1)
//            {
//                gameObject.name = $"{x},{y - 1}";

//                currentObjs[x, y] = null;
//                currentObjs[x + 1, y - 1] = gameObject;

//                //currentAvailablity[x, y] = 0;
//                currentAvailablity[x + 1, y - 1] = 1;

//                pos = gameObject.transform.position;
//                pos = new Vector2(pos.x + 1.3f, pos.y - 1.3f);

//                canMoveable = true;
//                //gameObject.transform.position = pos;
//            }
//        }
//    }

//    private void MoveIt()
//    {
//        current = Mathf.MoveTowards(current, target, itemMovingSpeed * Time.deltaTime);

//        transform.position = Vector3.Lerp(transform.position, pos, current);

//        if (current == target)
//        {
//            canMoveable = false;
//        }
//    }
//}