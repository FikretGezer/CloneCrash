using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum GameState
{
    wait,
    move
}
public enum TileKind
{
    Blank,
    Breakable,
    Normal
}
[System.Serializable]
public class TileType
{
    public int x;
    public int y;
    public TileKind tileKind;
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
    public GameObject breakableTilePrefab;
    public GameObject[] dotPrefabs;
    public TileType[] boardLayout;
    public Dot currentDot;

    public GameObject[,] allDots;

    private bool[,] blankSpaces;
    private BackgroundTile[,] breakableTiles;

    private MatchFinder matchFinder;
    

    private void Start()
    {
        matchFinder = FindObjectOfType<MatchFinder>();

        breakableTiles = new BackgroundTile[width, height];
        blankSpaces = new bool[width, height];
        allDots = new GameObject[width, height];
        Setup();
    }

    public void GenerateBlankSpaces()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.Blank)
            {
                blankSpaces[boardLayout[i].x, boardLayout[i].y] = true;
            }
        }
    }
    public void GenerateBreakableTiles()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.Breakable)
            {                
                var tempPos = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(breakableTilePrefab, tempPos, Quaternion.identity);
                breakableTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
        }
    }
    private void Setup()
    {
        GenerateBlankSpaces();
        GenerateBreakableTiles();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (!blankSpaces[x, y])
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
    }
    private bool MatchesAt(int column, int row, GameObject piece)
    {
        if(column > 1 && row > 1)
        {
            if (allDots[column - 1, row] != null && allDots[column - 2, row] != null)
            {
                if (allDots[column - 1, row].tag == piece.tag &&
                    allDots[column - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }
            if (allDots[column, row - 1] != null && allDots[column, row - 2] != null)
            {
                if (allDots[column, row - 1].tag == piece.tag &&
                    allDots[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }
        }
        else if(column <= 1 ||row <= 1)
        {
            if(row > 1)
            {
                if(allDots[column, row - 1] != null && allDots[column, row - 2] != null)
                {
                    if (allDots[column, row - 1].tag == piece.tag &&
                        allDots[column, row - 2].tag == piece.tag)
                    {
                        return true;
                    }
                }
            }
            if (column > 1)
            {
                if(allDots[column - 1, row] != null && allDots[column - 2, row] != null)
                {
                    if (allDots[column - 1, row].tag == piece.tag &&
                        allDots[column - 2, row].tag == piece.tag)
                    {
                        return true;
                    }
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

            if (breakableTiles[column, row] != null)
            {
                breakableTiles[column, row].TakeDamage(1);
                if (breakableTiles[column, row].hitPoints <= 0)
                    breakableTiles[column, row] = null;
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
        StartCoroutine(nameof(DecreaseRowCo2));
    }
    private IEnumerator DecreaseRowCo2()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //if the current sport isn't blank and is empty
                if (!blankSpaces[x, y] && allDots[x, y] == null)
                {
                    //loop from the space above to the top of the column
                    for (int i = y + 1; i < height; i++)
                    {
                        //if a dot is found
                        if (allDots[x, i] != null)
                        {
                            allDots[x, i].GetComponent<Dot>().row = y;
                            allDots[x, i] = null;
                            break;
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(0.4f);
        StartCoroutine(nameof(FillBoardCo));
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
                if (allDots[x,y] == null && !blankSpaces[x, y])
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
        if (IsDeadLocked())
            ShuffleBoard();
        gameState = GameState.move;       
    }
    

    private bool CheckForMatches()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allDots[x, y] != null)
                {
                    if(x < width - 2)
                    {
                        if (allDots[x + 1, y] != null && allDots[x + 2, y] != null)
                        {
                            if (allDots[x + 1, y].tag == allDots[x, y].tag 
                             && allDots[x + 2, y].tag == allDots[x, y].tag)
                            {
                                return true;
                            }
                        }
                    }
                    if(y < height - 2)
                    {
                        if (allDots[x, y + 1] != null && allDots[x, y + 2] != null)
                        {
                            if (allDots[x, y + 1].tag == allDots[x, y].tag
                             && allDots[x, y + 2].tag == allDots[x, y].tag)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }

        return false;
    }
    private void SwitchPieces(int column, int row, Vector2 dir)
    {
        if (column + (int)dir.x < width && row + (int)dir.y < height)
        {
            if (allDots[column + (int)dir.x, row + (int)dir.y] != null)
            {
                GameObject holder = allDots[column + (int)dir.x, row + (int)dir.y];
                allDots[column + (int)dir.x, row + (int)dir.y] = allDots[column, row];
                allDots[column, row] = holder;
            }
        }
    }
    private bool SwitchAndCheck(int column, int row, Vector2 dir)
    {
        SwitchPieces(column, row, dir);
        if(CheckForMatches())
        {
            SwitchPieces(column, row, dir);
            return true;
        }
        SwitchPieces(column, row, dir);
        return false;
    }
    private bool IsDeadLocked()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allDots[x, y] != null)
                {
                    if(x < width - 1)
                    {
                        if(SwitchAndCheck(x, y, Vector2.right))
                        {
                            return false;
                        }
                    }
                    if (y < height - 1)
                    {
                        if (SwitchAndCheck(x, y, Vector2.up))
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }
    private void ShuffleBoard()
    {
        List<GameObject> newBoard = new List<GameObject>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allDots[x, y] != null)
                {
                    newBoard.Add(allDots[x, y]);
                }
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (!blankSpaces[x, y])
                {
                    int pieceToUse = Random.Range(0, newBoard.Count);

                    int maxIterations = 0;
                    while (MatchesAt(x, y, newBoard[pieceToUse]) && maxIterations < 100)
                    {
                        pieceToUse = Random.Range(0, newBoard.Count);
                        maxIterations++;
                    }

                    Dot piece = newBoard[pieceToUse].GetComponent<Dot>();
                    piece.column = x;
                    piece.row = y;
                    allDots[x, y] = newBoard[pieceToUse];
                    newBoard.Remove(newBoard[pieceToUse]);
                }
            }
        }

        if(IsDeadLocked())
        {
            ShuffleBoard();
        }
    }
}
