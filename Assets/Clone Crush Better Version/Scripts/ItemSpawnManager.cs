using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum TypeOfTile
{
    Empty
}
[Serializable]
public class ExtraPiece
{
    public int column, row;
    public TypeOfTile type;
}
public class ItemSpawnManager : MonoBehaviour
{
    [Header("Board Size")]
    public int boardWidth;
    public int boardHeight;

    [Header("Board Spawn Objects")]
    [SerializeField] private GameObject bgTilePrefab; //BG tiles
    [SerializeField] private GameObject[] piecePrefabs; // Pieces
    [SerializeField] private ExtraPiece[] specialTiles; // Empty, lock or concrete tiles

    [Header("Arrays To Hold Pieces")]
    public GameObject[,] pieceList; //Holds piece
    public bool[,] blankTiles; // holds blanks parts

    //Parent for spawns
    GameObject parentTile;
    GameObject parentPieces;

    public static ItemSpawnManager Instance;
    private void Awake()
    {
        if(Instance == null) Instance = this;

        pieceList = new GameObject[boardWidth, boardHeight];
        blankTiles = new bool[boardWidth, boardHeight];

        //Create parent for the bgTile
        parentTile = new GameObject();
        parentTile.name = "Board Tiles";

        //Create parent for the objects
        parentPieces = new GameObject();
        parentPieces.name = "Board Pieces";

        CreateTheBoard();
    }
    private void CreateBlankSpaces()
    {
        for (int i = 0; i < specialTiles.Length; i++)
        {
            if (specialTiles[i].type == TypeOfTile.Empty)
            {
                int column = specialTiles[i].column;
                int row = specialTiles[i].row;
                blankTiles[column, row] = true;
            }
        }
    }
    private void CreateTheBoard()
    {
        //Create extra tiles
        //CreateBlankSpaces();

        //Create the board
        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                Vector2 instantiatePosition = new Vector2(x, y);

                //Create tiles
                var tile = Instantiate(bgTilePrefab, instantiatePosition, Quaternion.identity);
                tile.transform.SetParent(parentTile.transform);

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
    //Checks is there a match while creating a row
    private bool IsMatchingHorizontally(int x, int y, GameObject selectedPrefab)
    {
        //Variables' tags
        string leftTag, leftLeftTag;
        leftTag = leftLeftTag = string.Empty;

        //Get the tags
        if(x - 1 > 0 && pieceList[x - 1, y] != null)
            leftTag = pieceList[x - 1, y].tag;

        if(x - 2 >= 0 && pieceList[x - 2, y] != null)
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
        if(y - 1 > 0 && pieceList[x, y - 1] != null)
            downTag = pieceList[x, y - 1].tag;

        if(y - 2 >= 0 && pieceList[x, y - 2] != null)
            downDownTag = pieceList[x, y - 2].tag;

        //Compare the tags
        if(downDownTag == selectedPrefab.tag && downTag == selectedPrefab.tag)
            return true;

        return false;
    }

}