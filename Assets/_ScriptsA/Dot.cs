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
    private MatchFinder matchFinder;
    public GameObject otherDot;
    private Vector2 firstTouchPos;
    private Vector2 lastTouchPos;
    private Camera cam;
    private SpriteRenderer mySprite;

    [Header("Powerup Stuff")]
    public bool isColorBomb;
    public bool isColumnBomb;
    public bool isRowBomb;
    public bool isAdjacentBomb;
    public GameObject rowArrow;
    public GameObject columnArrow;
    public GameObject colorBomb;
    public GameObject adjacentMarker;
    private void Start()
    {
        cam = Camera.main;
        mySprite = GetComponent<SpriteRenderer>();

        board = FindObjectOfType<Board>();
        matchFinder = FindObjectOfType<MatchFinder>();

        //targetX = (int)transform.position.x;
        //targetY = (int)transform.position.y;

        //column = targetX;
        //row = targetY;

        //prevRow = row;
        //prevColumn = column;

        isColumnBomb = false;
        isRowBomb = false;
        isColorBomb = false;
        isAdjacentBomb = false;        
    }


    private void OnMouseOver()
    {
        if(Input.GetMouseButtonDown(1))
        {
            isAdjacentBomb = true;
            GameObject market = Instantiate(adjacentMarker, transform.position, Quaternion.identity);
            market.transform.parent = this.transform;

            //isColumnBomb = true;
            //GameObject arrow = Instantiate(columnArrow, transform.position, Quaternion.identity);
            //arrow.transform.parent = this.transform;
        }
    }

    private void Update()
    {      
        targetX = column;
        targetY = row;
        if(Mathf.Abs(targetX - transform.position.x) > 0.1f)
        {
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPos, board.lerpSpeed * Time.deltaTime);
            if (board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
            }
            matchFinder.FindMatches();
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
            if (board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
            }
            matchFinder.FindMatches();
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
        if(isColorBomb)
        {
            matchFinder.MatchPiecesOfColor(otherDot.tag);
            isMatched = true;
        }
        else if(otherDot.GetComponent<Dot>().isColorBomb)
        {
            matchFinder.MatchPiecesOfColor(gameObject.tag);
            otherDot.GetComponent<Dot>().isMatched = true;
        }

        yield return new WaitForSeconds(0.5f);
        if(otherDot != null)
        {
            if(!isMatched && !otherDot.GetComponent<Dot>().isMatched)
            {
                otherDot.GetComponent<Dot>().row = row;
                otherDot.GetComponent<Dot>().column = column;

                row = prevRow;
                column = prevColumn;
                yield return new WaitForSeconds(0.5f);
                board.currentDot = null;
                board.gameState = GameState.move;
            }
            else
            {
                board.DestroyMatches();
            }
            otherDot = null;
        }
        
    }
    private float dirLength;
    private void OnMouseDown()
    {     
        if(board.gameState == GameState.move)
            firstTouchPos = cam.ScreenPointToRay(Input.mousePosition).origin;
    }
    private void OnMouseUp()
    {
        if(board.gameState == GameState.move)
        { 
            lastTouchPos = cam.ScreenPointToRay(Input.mousePosition).origin;

            var dir = (lastTouchPos - firstTouchPos);
            dirLength = dir.magnitude;
            swipeAngle = DirToAngleDeg(dir);

            if (dirLength > 0.011f)
            {
                MovePieces();
                board.gameState = GameState.wait;
                board.currentDot = this;
            }
        }
        else
        {
            board.gameState = GameState.move;
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
    private void MovePieces()
    {
        if((swipeAngle < 45f && swipeAngle >= 0f) || (swipeAngle > 315f && swipeAngle < 360f) && column < board.width - 1)//Swipe RIGHT
        {
            otherDot = board.allDots[column + 1, row];
            if (otherDot != null)
            {
                prevRow = row;
                prevColumn = column;
                otherDot.GetComponent<Dot>().column -= 1;
                column += 1;
            }
        }
        else if (swipeAngle > 45f && swipeAngle <= 135f && row < board.height - 1)//Swipe UP
        {
            otherDot = board.allDots[column, row + 1];
            if (otherDot != null)
            {
                prevRow = row;
                prevColumn = column;
                otherDot.GetComponent<Dot>().row -= 1;
                row += 1;
            }
        }
        else if (swipeAngle > 135f && swipeAngle <= 225f && column > 0)//Swipe LEFT
        {
            otherDot = board.allDots[column - 1, row];
            if(otherDot != null)
            {
                prevRow = row;
                prevColumn = column;
                otherDot.GetComponent<Dot>().column += 1;
                column -= 1;
            }
        }
        else if (swipeAngle > 225f && swipeAngle <= 315f && row > 0)//Swipe DOWN
        {
            otherDot = board.allDots[column, row - 1];
            if(otherDot != null)
            {
                prevRow = row;
                prevColumn = column;
                otherDot.GetComponent<Dot>().row += 1;
                row -= 1;
            }
        }
        StartCoroutine(nameof(CheckMoveCor));
    }
    private void FindMatches()
    {
        if(column > 0 && column < board.width - 1)
        {
            if(board.allDots[column - 1, row] != null && board.allDots[column + 1, row] != null)
            {
                GameObject leftDot1 = board.allDots[column - 1, row];
                GameObject rightDot1 = board.allDots[column + 1, row];
                if (leftDot1 != null && rightDot1 != null && leftDot1.tag == gameObject.tag && rightDot1.tag == this.gameObject.tag)
                {
                    leftDot1.GetComponent<Dot>().isMatched = true;
                    rightDot1.GetComponent<Dot>().isMatched = true;
                    isMatched = true;
                }
            }            
        }
        if (row > 0 && row < board.height - 1)
        {
            if(board.allDots[column, row - 1] != null && board.allDots[column, row + 1] != null)
            {
                GameObject downDot1 = board.allDots[column, row - 1];
                GameObject upDot1 = board.allDots[column, row + 1];
                if (downDot1 != null && upDot1 != null && downDot1.tag == gameObject.tag && upDot1.tag == this.gameObject.tag)
                {
                    downDot1.GetComponent<Dot>().isMatched = true;
                    upDot1.GetComponent<Dot>().isMatched = true;
                    isMatched = true;
                }
            }
        }
    }
    public void MakeRowBomb()
    {
        isRowBomb = true;
        GameObject arrow = Instantiate(rowArrow, transform.position, Quaternion.identity);
        arrow.transform.parent = this.transform;        
    }
    public void MakeColumnBomb()
    {
        isColumnBomb = true;
        GameObject arrow = Instantiate(columnArrow, transform.position, Quaternion.identity);
        arrow.transform.parent = this.transform;
    }
    public void MakeColorBomb()
    {
        isColorBomb = true;
        GameObject bomb = Instantiate(colorBomb, transform.position, Quaternion.identity);
        bomb.transform.parent = this.transform;
    }
    public void MakeAdjacentBomb()
    {
        isAdjacentBomb = true;
        GameObject marker = Instantiate(adjacentMarker, transform.position, Quaternion.identity);
        marker.transform.parent = this.transform;
    }
}
