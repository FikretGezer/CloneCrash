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
                    if(x > 0 && x < board.width - 1)
                    {
                        GameObject leftDot = board.allDots[x - 1, y];
                        GameObject rightDot = board.allDots[x + 1, y];

                        if(leftDot != null && rightDot != null)
                        {
                            if(leftDot.tag == currentDot.tag && currentDot.tag == rightDot.tag)
                            {
                                if(currentDot.GetComponent<Dot>().isRowBomb 
                                    || leftDot.GetComponent<Dot>().isRowBomb 
                                    || rightDot.GetComponent<Dot>().isRowBomb)
                                {
                                    currentMatches.Union(GetRowPieces(y));
                                }

                                if (currentDot.GetComponent<Dot>().isColumnBomb)
                                    currentMatches.Union(GetColumnPieces(x));
                                if (leftDot.GetComponent<Dot>().isColumnBomb)
                                    currentMatches.Union(GetColumnPieces(x - 1));
                                if (rightDot.GetComponent<Dot>().isColumnBomb)
                                    currentMatches.Union(GetColumnPieces(x + 1));


                                AddMatchToTheList(leftDot);
                                leftDot.GetComponent<Dot>().isMatched = true;
                                AddMatchToTheList(rightDot);
                                rightDot.GetComponent<Dot>().isMatched = true;
                                AddMatchToTheList(currentDot);
                                currentDot.GetComponent<Dot>().isMatched = true;
                            }
                        }
                    }
                    if (y > 0 && y < board.height - 1)
                    {
                        GameObject downDot = board.allDots[x, y - 1];
                        GameObject upDot = board.allDots[x, y + 1];

                        if (downDot != null && upDot != null)
                        {
                            if (downDot.tag == currentDot.tag && currentDot.tag == upDot.tag)
                            {
                                if (currentDot.GetComponent<Dot>().isColumnBomb 
                                    || upDot.GetComponent<Dot>().isColumnBomb 
                                    || downDot.GetComponent<Dot>().isColumnBomb)
                                {
                                    currentMatches.Union(GetColumnPieces(x));
                                }

                                if (currentDot.GetComponent<Dot>().isRowBomb)
                                    currentMatches.Union(GetRowPieces(y));
                                if (downDot.GetComponent<Dot>().isRowBomb)
                                    currentMatches.Union(GetRowPieces(y - 1));
                                if (upDot.GetComponent<Dot>().isRowBomb)
                                    currentMatches.Union(GetRowPieces(y + 1));


                                AddMatchToTheList(upDot);
                                downDot.GetComponent<Dot>().isMatched = true;
                                AddMatchToTheList(downDot);
                                upDot.GetComponent<Dot>().isMatched = true;
                                AddMatchToTheList(currentDot);
                                currentDot.GetComponent<Dot>().isMatched = true;
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

                int bombLuck = Random.Range(0, 100);
                if(bombLuck < 50)
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

                    int bombLuck = Random.Range(0, 100);
                    if (bombLuck < 50)
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
