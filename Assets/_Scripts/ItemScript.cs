using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : MonoBehaviour
{
    [SerializeField] private float itemMovingSpeed = 1f;

    private int xLength;
    private int yLength;

    private float current;
    private float target;

    private Vector3 pos;
    private bool canMoveable;
    private void Awake()
    {
        current = 0f;
        target = 1f;

        xLength = ItemPlacement._instance.placedObjects.GetLength(0);
        yLength = ItemPlacement._instance.placedObjects.GetLength(1);
    }
    private void Start()
    {
        CheckMatches();        
    }
    private void Update()
    {
        if (canMoveable)
        {
            MoveIt();
        }
    }
    private void LateUpdate()
    {
        if(!ItemMovementSecond.Instance.ObjectsMoving)
        {
            CheckMatches();
            MoveDown();
        }        
    }
    private void CheckMatches()
    {
        var currentObjs = ItemPlacement._instance.placedObjects;
        var currentAvailablity = ItemPlacement._instance.placementAvailablity;

        var indexString = gameObject.name.Split(',');
        var x = Convert.ToInt32(indexString[0]);
        var y = Convert.ToInt32(indexString[1]);

        //Checks horizontally
        if (x - 1 >= 0 && x + 1 < xLength && 
            currentObjs[x - 1, y] != null && currentObjs[x + 1, y] != null)
        {
            var firstSprite = currentObjs[x - 1, y].GetComponent<SpriteRenderer>();
            var secondSprite = currentObjs[x + 1, y].GetComponent<SpriteRenderer>();
            var currentSprite = GetComponent<SpriteRenderer>();

            if (firstSprite.sprite == currentSprite.sprite && currentSprite.sprite == secondSprite.sprite)
            {
                firstSprite.color = secondSprite.color = currentSprite.color = Color.black;

                currentAvailablity[x - 1, y] = 0;
                currentAvailablity[x + 1, y] = 0;
                currentAvailablity[x, y] = 0;

                Destroy(firstSprite.gameObject);
                Destroy(secondSprite.gameObject);
                Destroy(gameObject);

            }
        }

        //Checks Vertically
        if (y - 1 >= 0 && y + 1 < yLength &&
            currentObjs[x, y - 1] != null && currentObjs[x, y + 1] != null)
        {
            var firstSprite = currentObjs[x, y - 1].GetComponent<SpriteRenderer>();
            var secondSprite = currentObjs[x, y + 1].GetComponent<SpriteRenderer>();
            var currentSprite = GetComponent<SpriteRenderer>();

            if (firstSprite.sprite == currentSprite.sprite && currentSprite.sprite == secondSprite.sprite)
            {
                firstSprite.color = secondSprite.color = currentSprite.color = Color.black;

                currentAvailablity[x, y - 1] = 0;
                currentAvailablity[x, y + 1] = 0;
                currentAvailablity[x, y] = 0;

                Destroy(firstSprite.gameObject);
                Destroy(secondSprite.gameObject);
                Destroy(gameObject);

            }            
        }
    }
    
    private void MoveDown()
    {
        var currentObjs = ItemPlacement._instance.placedObjects;
        var currentAvailablity = ItemPlacement._instance.placementAvailablity;

        var indexString = gameObject.name.Split(',');
        var x = Convert.ToInt32(indexString[0]);
        var y = Convert.ToInt32(indexString[1]);

        current = 0f;

        if (y - 1 >= 0)
        {                      
            if (currentAvailablity[x, y - 1] != 2 && currentAvailablity[x, y - 1] != 1)
            {
                gameObject.name = $"{x},{y-1}";

                currentObjs[x, y] = null;
                currentObjs[x, y - 1] = gameObject;

                currentAvailablity[x, y] = 0;
                currentAvailablity[x, y - 1] = 1;

                pos = gameObject.transform.position;
                pos = new Vector2(pos.x, pos.y - 1.3f);

                canMoveable = true;
                //gameObject.transform.position = pos;
            }
            else if(x - 1 >= 0 && currentAvailablity[x - 1, y - 1] != 2 && currentAvailablity[x - 1, y - 1] != 1)
            {
                gameObject.name = $"{x - 1},{y - 1}";

                currentObjs[x, y] = null;
                currentObjs[x - 1, y - 1] = gameObject;

                currentAvailablity[x, y] = 0;
                currentAvailablity[x - 1, y - 1] = 1;

                pos = gameObject.transform.position;
                pos = new Vector2(pos.x - 1.3f, pos.y - 1.3f);

                canMoveable = true;
                //gameObject.transform.position = pos;
            }
            else if(x + 1 < xLength && currentAvailablity[x + 1, y - 1] != 2 && currentAvailablity[x + 1, y - 1] != 1)
            {
                gameObject.name = $"{x},{y - 1}";

                currentObjs[x, y] = null;
                currentObjs[x + 1, y - 1] = gameObject;

                //currentAvailablity[x, y] = 0;
                currentAvailablity[x + 1, y - 1] = 1;

                pos = gameObject.transform.position;
                pos = new Vector2(pos.x + 1.3f, pos.y - 1.3f);

                canMoveable = true;
                //gameObject.transform.position = pos;
            }
        }
    }
    
    private void MoveIt()
    {
        current = Mathf.MoveTowards(current, target, itemMovingSpeed * Time.deltaTime);

        transform.position = Vector3.Lerp(transform.position, pos, current);

        if (current == target)
        {
            canMoveable = false;
        }
    }
}
