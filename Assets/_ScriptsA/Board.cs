using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum GameState
{
    wait,
    move
}

public class Board : MonoBehaviour
{
    public GameState gameState = GameState.move;
    public float lerpSpeed = 1f;
    public int width;
    public int height;
    public int offset;

    public GameObject destroyEffect;
    public GameObject tilePrefab;    
    public GameObject[] dotPrefabs;
    public Dot currentDot;

    public GameObject[,] allDots;
    private BackgroundTile[,] allTiles;

    private MatchFinder matchFinder;
    

    private void Start()
    {
        matchFinder = FindObjectOfType<MatchFinder>();

        allTiles = new BackgroundTile[width, height];
        allDots = new GameObject[width, height];
        Setup();
    }
    private void Setup()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var tempPos = new Vector2(x, y + offset);
                GameObject tile = Instantiate(tilePrefab, tempPos, Quaternion.identity);
                tile.transform.parent = transform;
                tile.name = $"( {x}, {y} )";

                GameObject selectedPrefab = dotPrefabs[Random.Range(0, dotPrefabs.Length)];
                int maxIterations = 0;
                while (MatchesAt(x,y,selectedPrefab) && maxIterations < 100)
                {
                    selectedPrefab = dotPrefabs[Random.Range(0, dotPrefabs.Length)];
                    maxIterations++;
                }
                Debug.Log(maxIterations);

                var dot = Instantiate(selectedPrefab, tempPos, Quaternion.identity);
                dot.GetComponent<Dot>().column = x;
                dot.GetComponent<Dot>().row = y;
                dot.transform.parent = transform;
                dot.name = tile.name;
                allDots[x, y] = dot;
            }
        }
    }
    private bool MatchesAt(int column, int row, GameObject piece)
    {
        if(column > 1 && row > 1)
        {
            if (allDots[column - 1, row].tag == piece.tag &&
                allDots[column - 2, row].tag == piece.tag)
            {
                return true;
            }
            if (allDots[column, row - 1].tag == piece.tag &&
                allDots[column, row - 2].tag == piece.tag)
            {
                return true;
            }
        }
        else if(column <= 1 ||row <= 1)
        {
            if(row > 1)
            {
                if (allDots[column, row - 1].tag == piece.tag &&
                    allDots[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }
            if (column > 1)
            {
                if (allDots[column - 1, row].tag == piece.tag &&
                    allDots[column - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }
        }
        return false;
    }
    private bool ColumnOrRow()
    {
        int numberHor = 0;
        int numberVer = 0;

        Dot firstPiece = matchFinder.currentMatches[0].GetComponent<Dot>();
        if(firstPiece != null)
        {
            foreach (var currentPiece in matchFinder.currentMatches)
            {
                Dot dot = currentPiece.GetComponent<Dot>();
                if (dot.row == firstPiece.row)
                    numberHor++;
                if (dot.column == firstPiece.column)
                    numberVer++;
            }
        }
        return (numberVer == 5 || numberHor == 5);
    }
    private void CheckToMakeBombs()
    {
        if (matchFinder.currentMatches.Count == 4
         || matchFinder.currentMatches.Count == 7)
        {
            matchFinder.CheckBombs();
        }
        if (matchFinder.currentMatches.Count == 5
         || matchFinder.currentMatches.Count == 8)
        {
            if(ColumnOrRow())
            {
                //Make a color bomb
                if(currentDot != null)
                {
                    if(currentDot.isMatched)
                    {
                        if(!currentDot.isColorBomb)
                        {
                            currentDot.isMatched = false;
                            currentDot.MakeColorBomb();
                        }
                    }
                    else 
                    {
                        if(currentDot.otherDot != null)
                        {
                            Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                            if (otherDot.isMatched)
                            {
                                if (!otherDot.isColorBomb)
                                {
                                    otherDot.isMatched = false;
                                    otherDot.MakeColorBomb();
                                }
                            }
                        }
                    }
                }                
            }
            else
            {
                //Make a adjacent bomb
                if (currentDot != null)
                {
                    if (currentDot.isMatched)
                    {
                        if (!currentDot.isAdjacentBomb)
                        {
                            currentDot.isMatched = false;
                            currentDot.MakeAdjacentBomb();
                        }
                    }
                    else
                    {
                        if (currentDot.otherDot != null)
                        {
                            Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                            if (otherDot.isMatched)
                            {
                                if (!otherDot.isAdjacentBomb)
                                {
                                    otherDot.isMatched = false;
                                    otherDot.MakeAdjacentBomb();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    private void DestroyMatchesAt(int column, int row)
    {
        if (allDots[column, row].GetComponent<Dot>().isMatched)
        {
            //if(matchFinder.currentMatches.Contains(allDots[column, row]))
            //{
            //    matchFinder.currentMatches.Remove(allDots[column, row]);
            //}
            if (matchFinder.currentMatches.Count >= 4)
            {
                CheckToMakeBombs();
            }
            GameObject effect = Instantiate(destroyEffect, allDots[column, row].transform.position, Quaternion.identity);
            Destroy(effect, 1f);
            //matchFinder.currentMatches.Remove(allDots[column, row]);
            Destroy(allDots[column, row]);
            allDots[column, row] = null;
        }       
    }
    public void DestroyMatches()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(allDots[x,y] != null)
                {
                    DestroyMatchesAt(x, y);
                }
            }
        }
        matchFinder.currentMatches.Clear();
        StartCoroutine(nameof(DecreaseRowCo));
    }
    private IEnumerator DecreaseRowCo()
    {
        yield return new WaitForSeconds(1f);
        int nullCount = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allDots[x, y] == null)
                    nullCount++;
                else if (nullCount > 0)
                {
                    allDots[x, y].GetComponent<Dot>().row -= nullCount;
                    allDots[x, y] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(0.4f);        
        StartCoroutine(nameof(FillBoardCo));
    }
    private void RefillBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allDots[x,y] == null)
                {
                    Vector2 tempPos = new Vector2(x, y + offset);
                    int dotToUse = Random.Range(0, dotPrefabs.Length);
                    GameObject piece = Instantiate(dotPrefabs[dotToUse], tempPos, Quaternion.identity);
                    allDots[x, y] = piece;

                    piece.GetComponent<Dot>().column = x;
                    piece.GetComponent<Dot>().row = y;
                }
            }
        }
    }
    private bool MatchesOnBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allDots[x,y] != null)
                {
                    if (allDots[x,y].GetComponent<Dot>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    private IEnumerator FillBoardCo()
    {
        RefillBoard();

        yield return new WaitForSeconds(0.5f);

        while(MatchesOnBoard())
        {
            yield return new WaitForSeconds(0.5f);
            DestroyMatches();
        }
        matchFinder.currentMatches.Clear();
        currentDot = null;
        yield return new WaitForSeconds(0.5f);
        gameState = GameState.move;
       
    }
}
