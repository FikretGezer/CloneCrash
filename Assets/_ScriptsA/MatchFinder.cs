using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MatchFinder : MonoBehaviour
{
    public List<GameObject> currentMatches = new List<GameObject>();

    private Board board;

    private void Awake()
    {
        board = FindObjectOfType<Board>();
    }
    public void FindMatches()
    {
        StartCoroutine(nameof(FindAllMatches));
    }
    private List<GameObject> IsAdjacentBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        var currentDots = new List<GameObject>();

        if (dot1.isAdjacentBomb)
            currentMatches.Union(GetAdjacentPieces(dot1.column, dot1.row));
        if (dot2.isAdjacentBomb)
            currentMatches.Union(GetAdjacentPieces(dot2.column, dot2.row));
        if (dot3.isAdjacentBomb)
            currentMatches.Union(GetAdjacentPieces(dot3.column, dot3.row));

        return currentDots;
    }
    private List<GameObject> IsRowBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        var currentDots = new List<GameObject>();

        if (dot1.isRowBomb)
            currentMatches.Union(GetRowPieces(dot1.row));
        if (dot2.isRowBomb)
            currentMatches.Union(GetRowPieces(dot2.row));
        if (dot3.isRowBomb)
            currentMatches.Union(GetRowPieces(dot3.row));

        return currentDots;
    }
    private List<GameObject> IsColumnBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        var currentDots = new List<GameObject>();

        if (dot1.isColumnBomb)
            currentMatches.Union(GetColumnPieces(dot1.column));
        if (dot2.isColumnBomb)
            currentMatches.Union(GetColumnPieces(dot2.column));
        if (dot3.isColumnBomb)
            currentMatches.Union(GetColumnPieces(dot3.column));

        return currentDots;
    }

    private void GetNearbyPieces(GameObject dot1, GameObject dot2, GameObject dot3)
    {
        AddMatchToTheList(dot1);        
        AddMatchToTheList(dot2);        
        AddMatchToTheList(dot3);        
    }
    private IEnumerator FindAllMatches()
    {
        yield return new WaitForSeconds(0.2F);
        for (int x = 0; x < board.width; x++)
        {
            for (int y = 0; y < board.height; y++)
            {
                GameObject currentDot = board.allDots[x, y];
                if(currentDot != null)
                {
                    Dot currentDotDot = currentDot.GetComponent<Dot>();
                    if(x > 0 && x < board.width - 1)
                    {
                        GameObject leftDot = board.allDots[x - 1, y];
                        GameObject rightDot = board.allDots[x + 1, y];

                        if(leftDot != null && rightDot != null)
                        {
                            Dot leftDotDot = leftDot.GetComponent<Dot>();
                            Dot rightDotDot = rightDot.GetComponent<Dot>();

                            if (leftDot.tag == currentDot.tag && currentDot.tag == rightDot.tag)
                            {                                
                                currentMatches.Union(IsRowBomb(leftDotDot, currentDotDot, rightDotDot));

                                currentMatches.Union(IsColumnBomb(leftDotDot, currentDotDot, rightDotDot));

                                currentMatches.Union(IsAdjacentBomb(leftDotDot, currentDotDot, rightDotDot));

                                GetNearbyPieces(leftDot, currentDot, rightDot);                                
                            }
                        }
                    }
                    if (y > 0 && y < board.height - 1)
                    {
                        GameObject downDot = board.allDots[x, y - 1];
                        GameObject upDot = board.allDots[x, y + 1];

                        if (downDot != null && upDot != null)
                        {
                            Dot downDotDot = downDot.GetComponent<Dot>();
                            Dot upDotDot = upDot.GetComponent<Dot>();

                            if (downDot.tag == currentDot.tag && currentDot.tag == upDot.tag)
                            {    
                                currentMatches.Union(IsColumnBomb(upDotDot, currentDotDot, downDotDot));

                                currentMatches.Union(IsRowBomb(upDotDot, currentDotDot, downDotDot));

                                currentMatches.Union(IsAdjacentBomb(upDotDot, currentDotDot, downDotDot));

                                GetNearbyPieces(upDot, currentDot, downDot);                                  
                            }
                        }
                    }
                }
            }
        }
    }
    private void AddMatchToTheList(GameObject matchObj)
    {
        if(!currentMatches.Contains(matchObj))
        { 
            currentMatches.Add(matchObj); 
        }
        matchObj.GetComponent<Dot>().isMatched = true;
    }

    public void MatchPiecesOfColor(string color)
    {
        for (int x = 0; x < board.width; x++)
        {
            for (int y = 0; y < board.height; y++)
            {
                if (board.allDots[x, y] != null)
                {
                    if (board.allDots[x, y].tag == color)
                    {
                        board.allDots[x, y].GetComponent<Dot>().isMatched = true;
                    }
                }
            }
        }
    }

    private List<GameObject> GetAdjacentPieces(int column, int row)
    {
        var dots = new List<GameObject>();
        for (int x = column - 1; x <= column + 1; x++)
        {
            for (int y = row - 1; y <= row + 1; y++)
            {
                if(x >= 0 && x < board.width && y >= 0 && y < board.height)
                {
                    dots.Add(board.allDots[x, y]);
                    board.allDots[x, y].GetComponent<Dot>().isMatched = true;
                }
            }
        }
        return dots;
    }

    private List<GameObject> GetColumnPieces(int column)
    {
        var dots = new List<GameObject>();
        for (int y = 0; y < board.height; y++)
        {
            if (board.allDots[column, y] != null)
            {
                dots.Add(board.allDots[column, y]);
                board.allDots[column, y].GetComponent<Dot>().isMatched = true;
            }
        }
        return dots;
    }
    private List<GameObject> GetRowPieces(int row)
    {
        var dots = new List<GameObject>();
        for (int x = 0; x < board.width; x++)
        {
            if (board.allDots[x, row] != null)
            {
                dots.Add(board.allDots[x, row]);
                board.allDots[x, row].GetComponent<Dot>().isMatched = true;
            }
        }
        return dots;
    }
    public void CheckBombs()
    {
        if(board.currentDot != null)
        {
            var curDot = board.currentDot;
            
            if (curDot.isMatched)
            {
                curDot.isMatched = false;

                if((curDot.swipeAngle < 45f && curDot.swipeAngle >= 0f) || (curDot.swipeAngle > 315f && curDot.swipeAngle < 360f)
                || (curDot.swipeAngle > 135f && curDot.swipeAngle <= 225f))
                {
                    curDot.MakeRowBomb();
                }
                else
                {
                    curDot.MakeColumnBomb();
                }
            }
            else if (board.currentDot.otherDot != null)
            {
                var otherDot = board.currentDot.otherDot.GetComponent<Dot>();
                if (otherDot.isMatched)
                {
                    otherDot.isMatched = false;

                    //if ((otherDot.swipeAngle < 45f && otherDot.swipeAngle >= 0f) || (otherDot.swipeAngle > 315f && otherDot.swipeAngle < 360f)
                    //|| (otherDot.swipeAngle > 135f && otherDot.swipeAngle <= 225f))
                    if ((curDot.swipeAngle < 45f && curDot.swipeAngle >= 0f) || (curDot.swipeAngle > 315f && curDot.swipeAngle < 360f)
                    || (curDot.swipeAngle > 135f && curDot.swipeAngle <= 225f))
                    {
                        otherDot.MakeRowBomb();
                    }
                    else
                    {
                        otherDot.MakeColumnBomb();
                    }
                }
            }
        }
    }
}
