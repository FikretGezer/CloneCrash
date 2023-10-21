using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    public float swipeAngle = 0;

    public int column;
    public int row;

    public int prevColumn;
    public int prevRow;

    public int targetX;
    public int targetY;

    public bool isMatched;

    private Vector2 tempPos;

    private Board board;
    private GameObject otherDot;
    private Vector2 firstTouchPos;
    private Vector2 lastTouchPos;
    private Camera cam;
    private SpriteRenderer mySprite;
    private void Start()
    {
        cam = Camera.main;
        board = FindObjectOfType<Board>();
        mySprite = GetComponent<SpriteRenderer>();

        targetX = (int)transform.position.x;
        targetY = (int)transform.position.y;

        column = targetX;
        row = targetY;

        prevRow = row;
        prevColumn = column;
    }
    private void Update()
    {
        FindMatches();
        if(isMatched)
        {
            mySprite.color = Color.black;
        }
        
        targetX = column;
        targetY = row;
        if(Mathf.Abs(targetX - transform.position.x) > 0.1f)
        {
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPos, board.lerpSpeed * Time.deltaTime);
        }
        else
        {
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = tempPos;
            board.allDots[column, row] = gameObject;
        }
        if (Mathf.Abs(targetY - transform.position.y) > 0.1f)
        {
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPos, board.lerpSpeed * Time.deltaTime);
        }
        else
        {
            tempPos = new Vector2(transform.position.x, targetY);            
            transform.position = tempPos;
            board.allDots[column, row] = gameObject;
        }
    }
    private IEnumerator CheckMoveCor()
    {
        yield return new WaitForSeconds(0.5f);
        if(otherDot != null)
        {
            if(!isMatched && !otherDot.GetComponent<Dot>().isMatched)
            {
                otherDot.GetComponent<Dot>().row = row;
                otherDot.GetComponent<Dot>().column = column;

                row = prevRow;
                column = prevColumn;
            }
            otherDot = null;
        }
    }
    private void OnMouseDown()
    {
        firstTouchPos = cam.ScreenPointToRay(Input.mousePosition).origin;
    }
    private void OnMouseUp()
    {
        lastTouchPos = cam.ScreenPointToRay(Input.mousePosition).origin;

        var dir = (lastTouchPos - firstTouchPos);
        swipeAngle = DirToAngleDeg(dir);
        MovePieces();
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
    private void MovePieces()
    {
        if((swipeAngle < 45f && swipeAngle >= 0f) || (swipeAngle > 315f && swipeAngle < 360f) && column < board.width - 1)//Swipe RIGHT
        {
            otherDot = board.allDots[column + 1, row];
            otherDot.GetComponent<Dot>().column -= 1;
            column += 1;
        }
        else if (swipeAngle > 45f && swipeAngle <= 135f && row < board.height - 1)//Swipe UP
        {
            otherDot = board.allDots[column, row + 1];
            otherDot.GetComponent<Dot>().row -= 1;
            row += 1;
        }
        else if (swipeAngle > 135f && swipeAngle <= 225f && column > 0)//Swipe LEFT
        {
            otherDot = board.allDots[column - 1, row];
            otherDot.GetComponent<Dot>().column += 1;
            column -= 1;
        }
        else if (swipeAngle > 225f && swipeAngle <= 315f && row > 0)//Swipe DOWN
        {
            otherDot = board.allDots[column, row - 1];
            otherDot.GetComponent<Dot>().row += 1;
            row -= 1;
        }
        StartCoroutine(nameof(CheckMoveCor));
    }
    private void FindMatches()
    {
        if(column > 0 && column < board.width - 1)
        {
            GameObject leftDot1 = board.allDots[column - 1, row];
            GameObject rightDot1 = board.allDots[column + 1, row];
            if(leftDot1.tag == gameObject.tag && rightDot1.tag == this.gameObject.tag)
            {
                leftDot1.GetComponent<Dot>().isMatched = true;
                rightDot1.GetComponent<Dot>().isMatched = true;
                isMatched = true;
            }
        }
        if (row > 0 && row < board.height - 1)
        {
            GameObject downDot1 = board.allDots[column, row - 1];
            GameObject upDot1 = board.allDots[column, row + 1];
            if (downDot1.tag == gameObject.tag && upDot1.tag == this.gameObject.tag)
            {
                downDot1.GetComponent<Dot>().isMatched = true;
                upDot1.GetComponent<Dot>().isMatched = true;
                isMatched = true;
            }
        }
    }
}
