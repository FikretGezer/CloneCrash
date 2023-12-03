using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum TypeOfTile
{
    Empty,
    Breakable,
    Ice
}
[Serializable]
public class ExtraPiece
{
    public int column, row;
    public TypeOfTile type;
}
public class ItemSpawnManager : MonoBehaviour
{
    // [SerializeField] public BoardLayoutScriptable level;
    [SerializeField] private WorldScriptable world;
    [field:SerializeField] public int CurrentLevel { get; private set; }
    [Header("Board Size")]
    public int boardWidth;
    public int boardHeight;

    [Header("Board Spawn Objects")]
    [SerializeField] private GameObject bgTilePrefab; //BG tiles
    [SerializeField] private GameObject[] piecePrefabs; // Pieces
    [SerializeField] private GameObject breakableTile; // Pieces
    [SerializeField] private GameObject iceTile; // Pieces
    [SerializeField] private ExtraPiece[] specialTiles; // Empty, lock or concrete tiles

    [Header("Arrays To Hold Pieces")]
    public GameObject[,] pieceList; //Holds piece
    public GameObject[,] breakableTiles; // Holds breakables: this type of tiles can't contain piece
    public GameObject[,] iceTiles; // Holds iceTiles: this type of tiles can contain piece
    public bool[,] blankTiles; // holds blanks parts

    //Parent for spawns
    GameObject parentTile;
    GameObject parentPieces;
    GameObject parentSpecials;

    public static ItemSpawnManager Instance;
    private void Awake()
    {
        if(Instance == null) Instance = this;
        CurrentLevel = PlayerPrefs.GetInt("Current Level");

        var level = world.allLevels[CurrentLevel];
        #region SCRIPTABLE
        if(level != null)
        {
            boardWidth = level.boardWidth;
            boardHeight = level.boardHeight;

            bgTilePrefab = level.bgTilePrefab;
            breakableTile = level.breakableTilePrefab;
            iceTile = level.iceTilePrefab;

            piecePrefabs = level.piecePrefabs;

            specialTiles = level.specialTiles;
        }
        #endregion


        pieceList = new GameObject[boardWidth, boardHeight];
        breakableTiles = new GameObject[boardWidth, boardHeight];
        iceTiles = new GameObject[boardWidth, boardHeight];
        blankTiles = new bool[boardWidth, boardHeight];

        //Create parent for the bgTile
        parentTile = new GameObject();
        parentTile.name = "Board Tiles";

        //Create parent for the objects
        parentPieces = new GameObject();
        parentPieces.name = "Board Pieces";

        //Create parent for the specials
        parentSpecials = new GameObject();
        parentSpecials.name = "Special Pieces";


        CreateTheBoard();
    }
    // private void CreateBlankSpaces()
    // {
    //     for (int i = 0; i < specialTiles.Length; i++)
    //     {
    //         if (specialTiles[i].type == TypeOfTile.Empty)
    //         {
    //             int column = specialTiles[i].column;
    //             int row = specialTiles[i].row;
    //             blankTiles[column, row] = true;
    //         }
    //     }
    // }
    private void CreateSpecialTiles()
    {
        for (int i = 0; i < specialTiles.Length; i++)
        {
            int column = specialTiles[i].column;
            int row = specialTiles[i].row;
            if (specialTiles[i].type == TypeOfTile.Empty)
            {
                blankTiles[column, row] = true;
            }
            else if(specialTiles[i].type == TypeOfTile.Breakable)
                CreateASpecial(column, row, breakableTiles, breakableTile);
            else if(specialTiles[i].type == TypeOfTile.Ice)
                CreateASpecial(column, row, iceTiles, iceTile);
        }
    }
    private void CreateTheBoard()
    {
        //Create extra tiles
        //CreateBlankSpaces();
        CreateSpecialTiles();

        //Create the board
        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                if(!blankTiles[x, y] && breakableTiles[x, y] == null)
                {
                    Vector2 instantiatePosition = new Vector2(x, y);

                    //Create tiles
                    if(iceTiles[x, y] == null)
                    {
                        var tile = Instantiate(bgTilePrefab, instantiatePosition, Quaternion.identity);
                        tile.transform.SetParent(parentTile.transform);
                    }

                    //Create Actual Pieces
                    int rnd = Random.Range(0, piecePrefabs.Length);
                    GameObject selectedPrefab = piecePrefabs[rnd];

                    int counter = 0;
                    while(counter < 100 && (IsMatchingHorizontally(x, y, selectedPrefab) || IsMatchingVertically(x, y, selectedPrefab)))
                    {
                        rnd = Random.Range(0, piecePrefabs.Length);
                        selectedPrefab = piecePrefabs[rnd];
                        counter++;
                    }

                    var piece = Instantiate(selectedPrefab, instantiatePosition, Quaternion.identity);
                    pieceList[x, y] = piece;
                    piece.name = $"{x}, {y}";
                    piece.GetComponent<Piece>().column = x;
                    piece.GetComponent<Piece>().row = y;
                    piece.transform.SetParent(parentPieces.transform);
                }
            }
        }
    }
    //Checks is there a match while creating a row
    private bool IsMatchingHorizontally(int x, int y, GameObject selectedPrefab)
    {
        //Variables' tags
        string leftTag, leftLeftTag;
        leftTag = leftLeftTag = string.Empty;

        //Get the tags
        if(x - 1 > 0 && pieceList[x - 1, y] != null /*&& !blankTiles[x - 1, y]*/)
            leftTag = pieceList[x - 1, y].tag;

        if(x - 2 >= 0 && pieceList[x - 2, y] != null /*&& !blankTiles[x - 2, y]*/)
            leftLeftTag = pieceList[x - 2, y].tag;

        //Compare the tags
        if(leftTag == selectedPrefab.tag && leftLeftTag == selectedPrefab.tag)
            return true;

        return false;
    }
    //Checks is there a match while creating a column
    private bool IsMatchingVertically(int x, int y, GameObject selectedPrefab)
    {
        //Tag variables
        string downDownTag, downTag;
        downDownTag = downTag = string.Empty;

        //Get the tags
        if(y - 1 > 0 && pieceList[x, y - 1] != null /*&& !blankTiles[x, y - 1]*/)
            downTag = pieceList[x, y - 1].tag;

        if(y - 2 >= 0 && pieceList[x, y - 2] != null /*&& !blankTiles[x, y - 2]*/)
            downDownTag = pieceList[x, y - 2].tag;

        //Compare the tags
        if(downDownTag == selectedPrefab.tag && downTag == selectedPrefab.tag)
            return true;

        return false;
    }
    public void FillTheBoard()
    {
        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                if(pieceList[x, y] == null && !blankTiles[x, y] && breakableTiles[x, y] == null)
                {
                    CreateNewPiece(x, y);
                }
            }
        }
    }
    private void CreateASpecial(int x, int y, GameObject[,] specialsArray, GameObject specialTilePrefab)
    {
        Vector2 instantiatePosition = new Vector2(x, y);

        //Create tiles
        var tile = Instantiate(bgTilePrefab, instantiatePosition, Quaternion.identity);
        tile.transform.SetParent(parentTile.transform);

        var special = Instantiate(specialTilePrefab, instantiatePosition, Quaternion.identity);
        specialsArray[x, y] = special;
        special.name = $"{x}, {y}";
        special.GetComponent<SpecialTile>().column = x;
        special.GetComponent<SpecialTile>().row = y;
        special.transform.SetParent(parentSpecials.transform);
    }
    private void CreateNewPiece(int x, int y)
    {
        Vector2 instantiatePosition = new Vector2(x, boardHeight + 3);

        //Create Actual Pieces
        int rnd = Random.Range(0, piecePrefabs.Length);
        GameObject selectedPrefab = piecePrefabs[rnd];

        int counter = 0;
        while(counter < 100 && (IsMatchingHorizontally(x, y, selectedPrefab) || IsMatchingVertically(x, y, selectedPrefab)))
        {
            rnd = Random.Range(0, piecePrefabs.Length);
            selectedPrefab = piecePrefabs[rnd];
            counter++;
        }

        var piece = Instantiate(selectedPrefab, instantiatePosition, Quaternion.identity);
        pieceList[x, y] = piece;
        piece.name = $"{x}, {y}";
        piece.GetComponent<Piece>().column = x;
        piece.GetComponent<Piece>().row = y;
        piece.transform.SetParent(parentPieces.transform);
        // piece.GetComponent<Piece>().current = 0f;
        // piece.GetComponent<Piece>().isMoving = true;
        piece.GetComponent<Piece>().ResetMovingValues();
    }
}