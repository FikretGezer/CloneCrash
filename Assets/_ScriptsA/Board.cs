using Assets._ScriptsA;
using System.Collections;
using System.Collections.Generic;
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
    Lock,
    Concrete,
    Slime,
    Normal
}

[System.Serializable]
public class MatchType
{
    public int type;
    public string color;
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
    [Header("Scriptable Stuff")]
    public World world;
    public int level;

    [Header("Other Stuff")]
    public GameState gameState = GameState.move;
    public float lerpSpeed = 1f;
    public int width;
    public int height;
    public int offset;
    public int basePieceValue = 20;

    public GameObject destroyEffect;
    public GameObject tilePrefab;
    public GameObject breakableTilePrefab;
    public GameObject lockTilePrefab;
    public GameObject concreteTilePrefab;

    public GameObject[] dotPrefabs;
    public TileType[] boardLayout;
    public Dot currentDot;

    public GameObject[,] allDots;
    public MatchType matchType;

    public float refillDelay = 0.5f;

    private bool[,] blankSpaces;
    private int streakValue = 1;
    private BackgroundTile[,] breakableTiles;
    public BackgroundTile[,] lockTiles;
    public BackgroundTile[,] concreteTiles;

    private MatchFinder matchFinder;
    private ScoreManager scoreManager;
    private SoundManager soundManager;

    private void Start()
    {
        if (PlayerPrefs.HasKey("Selected Level"))
            level = PlayerPrefs.GetInt("Selected Level");
        SelectLevel();   
        
        matchFinder = FindObjectOfType<MatchFinder>();
        scoreManager = FindObjectOfType<ScoreManager>();
        soundManager = FindObjectOfType<SoundManager>();

        breakableTiles = new BackgroundTile[width, height];
        lockTiles = new BackgroundTile[width, height];
        concreteTiles = new BackgroundTile[width, height];
        blankSpaces = new bool[width, height];
        allDots = new GameObject[width, height];
        Setup();
    }
    private void SelectLevel()
    {
        if(world != null)
        {
            if(world.levels.Length > 0)
            {
                if (world.levels[level] != null)
                {
                    Level currentLevel = world.levels[level];
                    //Board size
                    width = currentLevel.width;
                    height = currentLevel.height;

                    //Board objects
                    dotPrefabs = currentLevel.dotPrefabs;

                    //Board layouts
                    boardLayout = currentLevel.boardLayout;
                }
            }
        }
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
    private void GenerateLockTiles()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.Lock)
            {
                var tempPos = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(lockTilePrefab, tempPos, Quaternion.identity);
                lockTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
        }
    }
    private void GenerateConcreteTiles()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.Concrete)
            {
                var tempPos = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(concreteTilePrefab, tempPos, Quaternion.identity);
                concreteTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
        }
    }
    private void Setup()
    {
        GenerateBlankSpaces();
        GenerateBreakableTiles();
        GenerateLockTiles();
        GenerateConcreteTiles();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (!blankSpaces[x, y] /*&& !concreteTiles[x, y]*/)
                {
                    var tempPos = new Vector2(x, y + offset);
                    var tilePos = new Vector2(x, y);
                    GameObject tile = Instantiate(tilePrefab, tilePos, Quaternion.identity);
                    tile.transform.parent = transform;
                    tile.name = $"( {x}, {y} )";

                    if(!concreteTiles[x, y])
                    {
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
    private MatchType ColumnOrRow()
    {/*
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
        matchFinder.currentMatches.Clear();
        return (numberVer == 5 || numberHor == 5);*/

        var matches = matchFinder.currentMatches;

        matchType.type = 0;
        matchType.color = "";

        for (int i = 0; i < matches.Count; i++)
        {
            Dot thisDot = matches[i].GetComponent<Dot>();
            string color = matches[i].tag;

            int column = thisDot.column;
            int row = thisDot.row;

            int columnMatch = 0;
            int rowMatch = 0;

            for (int j = 0; j < matches.Count; j++)
            {
                Dot nextDot = matches[j].GetComponent<Dot>();

                if (nextDot == thisDot) continue;
                if (nextDot.column == thisDot.column && nextDot.CompareTag(color))
                    columnMatch++;
                if (nextDot.row == thisDot.row && nextDot.CompareTag(color))
                    rowMatch++;
            }
            //Return 3 if column or row match
            if (columnMatch == 4 || rowMatch == 4)
            {
                matchType.type = 1;
                matchType.color = color;
                return matchType;
            }

            //Return 2 if adjacent
            else if (columnMatch == 2 || rowMatch == 2)
            {
                matchType.type = 2;
                matchType.color = color;
                return matchType;
            }

            //Return 1 if it's a color bomb
            else if (columnMatch == 3 || rowMatch == 3)
            {
                matchType.type = 3;
                matchType.color = color;
                return matchType;
            }

        }
        matchType.type = 0;
        matchType.color = "";

        return matchType;
    }
    private void CheckToMakeBombs()
    {/*
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
        }*/
        if(matchFinder.currentMatches.Count > 3)
        {
            //Type of match
            MatchType typeOfMatch = ColumnOrRow();

            if(typeOfMatch.type != 0)
            {
                if(typeOfMatch.type == 1)
                {
                    //Make a color bomb
                    if (currentDot != null)
                    {
                        if (currentDot.isMatched && currentDot.tag == typeOfMatch.color)
                        {
                            //if (!currentDot.isColorBomb)
                            //{
                            //}
                            currentDot.isMatched = false;
                            currentDot.MakeColorBomb();
                        }
                        else
                        {
                            if (currentDot.otherDot != null)
                            {
                                Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                                if (otherDot.isMatched && otherDot.tag == typeOfMatch.color)
                                {
                                    //if (!otherDot.isColorBomb)
                                    //{
                                    //}
                                    otherDot.isMatched = false;
                                    otherDot.MakeColorBomb();
                                }
                            }
                        }
                    }
                }
                else if(typeOfMatch.type == 2)
                {
                    //Make a adjacent bomb
                    if (currentDot != null)
                    {
                        if (currentDot.isMatched && currentDot.tag == typeOfMatch.color)
                        {
                            //if (!currentDot.isAdjacentBomb)
                            //{
                            //}
                            currentDot.isMatched = false;
                            currentDot.MakeAdjacentBomb();
                        }
                        else
                        {
                            if (currentDot.otherDot != null)
                            {
                                Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                                if (otherDot.isMatched && otherDot.tag == typeOfMatch.color)
                                {
                                    //if (!otherDot.isAdjacentBomb)
                                    //{
                                    //}
                                    otherDot.isMatched = false;
                                    otherDot.MakeAdjacentBomb();
                                }
                            }
                        }
                    }
                }
                else if(typeOfMatch.type == 3)
                {
                    matchFinder.CheckBombs(typeOfMatch);
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


            if (breakableTiles[column, row] != null)
            {
                breakableTiles[column, row].TakeDamage(1);
                if (breakableTiles[column, row].hitPoints <= 0)
                    breakableTiles[column, row] = null;
            }

            if (lockTiles[column, row] != null)
            {
                lockTiles[column, row].TakeDamage(1);
                if (lockTiles[column, row].hitPoints <= 0)
                    lockTiles[column, row] = null;
            }
            DamageConcrete(column, row);
            if (soundManager != null)
            {
                soundManager.PlaySound();
                //soundManager.AddCount();
                //soundManager.CreateSFX();
            }

            GameObject effect = Instantiate(destroyEffect, allDots[column, row].transform.position, Quaternion.identity);
            var name = allDots[column, row].gameObject.tag.Split(" ");
            if (name.Length > 1)
            {
                var p = effect.GetComponent<ParticleSystem>().main;
                p.startColor = new ParticleSystem.MinMaxGradient(StringToColor(name[1]));
            }
            Destroy(effect, 1f);
            //matchFinder.currentMatches.Remove(allDots[column, row]);
            Destroy(allDots[column, row]);
            scoreManager.IncreaseScore(basePieceValue * streakValue);
            scoreManager.CheckObjectives(allDots[column, row]);
            allDots[column, row] = null;
        }       
    }
    Color StringToColor(string colorName)
    {
        colorName = colorName.ToLower(); // Convert the color name to lowercase for case insensitivity

        switch (colorName)
        {
            case "red":
                return Color.red;
            case "blue":
                return Color.cyan;
            case "green":
                return Color.green;
            case "orange":
                return new Color(1, 0.498f, 0.314f, 1f);
            case "purple":
                return new Color(0.5f, 0f, 0.5f, 1f); 
            // Add more cases as needed
            default:
                return Color.white; // Return a default color if the input is not recognized
        }
    }
    public void DestroyMatches()
    {
        if (matchFinder.currentMatches.Count >= 4)
        {
            CheckToMakeBombs();
        }
        matchFinder.currentMatches.Clear();
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
        soundManager.SpawnSound();
        StartCoroutine(nameof(DecreaseRowCo2));
    }
    private void DamageConcrete(int column, int row)
    {
        if(column > 0)
        {
            if (concreteTiles[column - 1, row])
            {
                concreteTiles[column - 1, row].TakeDamage(1);
                if (concreteTiles[column - 1, row].hitPoints <= 0)
                {
                    concreteTiles[column - 1, row] = null;
                }
            }
        }
        if (column < width - 1)
        {
            if (concreteTiles[column + 1, row])
            {
                concreteTiles[column + 1, row].TakeDamage(1);
                if (concreteTiles[column + 1, row].hitPoints <= 0)
                {
                    concreteTiles[column + 1, row] = null;
                }
            }
        }
        if (row > 0)
        {
            if (concreteTiles[column, row - 1])
            {
                concreteTiles[column, row - 1].TakeDamage(1);
                if (concreteTiles[column, row - 1].hitPoints <= 0)
                {
                    concreteTiles[column, row - 1] = null;
                }
            }
        }
        if (row < height - 1)
        {
            if (concreteTiles[column, row + 1])
            {
                concreteTiles[column, row + 1].TakeDamage(1);
                if (concreteTiles[column, row + 1].hitPoints <= 0)
                {
                    concreteTiles[column, row + 1] = null;
                }
            }
        }
    }
    private IEnumerator DecreaseRowCo2()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //if the current sport isn't blank and is empty
                if (!blankSpaces[x, y] && allDots[x, y] == null && !concreteTiles[x, y])
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
        yield return new WaitForSeconds(refillDelay * 0.5f);
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
        yield return new WaitForSeconds(refillDelay * 0.5f);        
        StartCoroutine(nameof(FillBoardCo));
    }
    private void RefillBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allDots[x,y] == null && !blankSpaces[x, y] && !concreteTiles[x, y])
                {
                    Vector2 tempPos = new Vector2(x, y + offset);
                    int dotToUse = Random.Range(0, dotPrefabs.Length);
                    int maxIterations = 0;
                    while(MatchesAt(x, y, dotPrefabs[dotToUse]) && maxIterations < 100)
                    {
                        maxIterations++;
                        dotToUse = Random.Range(0, dotPrefabs.Length);                        
                    }

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
        matchFinder.FindMatches();
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

        yield return new WaitForSeconds(refillDelay);
        RefillBoard();

        while(MatchesOnBoard())
        {
            streakValue++;
            DestroyMatches();
            yield break;
            //yield return new WaitForSeconds(2 * refillDelay);

        }
        //matchFinder.currentMatches.Clear();
        currentDot = null;
        yield return new WaitForSeconds(refillDelay);
        if (IsDeadLocked())
            ShuffleBoard();
        gameState = GameState.move;
        streakValue = 1;
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
    public bool SwitchAndCheck(int column, int row, Vector2 dir)
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
                if (!blankSpaces[x, y] && !concreteTiles[x, y])
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
    public void BombRow(int row)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (concreteTiles[x, y])
                {
                    concreteTiles[x, row].TakeDamage(1);
                    if (concreteTiles[x, row].hitPoints <= 0)
                    {
                        concreteTiles[x, row] = null;
                    }
                }
            }
        }
    }
    public void BombColumn(int column)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (concreteTiles[x, y])
                {
                    concreteTiles[column, y].TakeDamage(1);
                    if (concreteTiles[column, y].hitPoints <= 0)
                    {
                        concreteTiles[column, y] = null;
                    }
                }
            }
        }
    }
}
