using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum PieceType
{
    Empty
}
[Serializable]
public class PieceSelect
{
    public int column, row;
    public PieceType type;
}
public class ItemSpawnManager : MonoBehaviour
{
    [Header("Board Size")]
    public int boardWidth;
    public int boardHeight;

    [Header("Board Objects")]
    [SerializeField] private GameObject bgTilePrefab;
    [SerializeField] private GameObject[] piecePrefabs;//This will be itemPrefabs later
    [SerializeField] private PieceSelect[] specialTiles;

    public GameObject[,] itemList;
    public bool[,] blankSpaces;
    private bool[,] fillingSpaces;
    public List<GameObject> matchedPieces = new List<GameObject>();

    //Parent for spawns
    GameObject parentTile;
    GameObject parentObjects;

    public static ItemSpawnManager Instance;
    private void Awake()
    {
        if (Instance == null) Instance = this;

        itemList = new GameObject[boardWidth, boardHeight];
        blankSpaces = new bool[boardWidth, boardHeight];
        fillingSpaces = new bool[boardWidth, boardHeight];


        //Create parent for the bgTile
        parentTile = new GameObject();
        parentTile.name = "Board Tiles";

        //Create parent for the objects
        parentObjects = new GameObject();
        parentObjects.name = "Board Pieces";

        CreateTheBoard();
    }
    private void CreateBlankSpaces()
    {
        for (int i = 0; i < specialTiles.Length; i++)
        {
            if (specialTiles[i].type == PieceType.Empty)
            {
                int column = specialTiles[i].column;
                int row = specialTiles[i].row;
                blankSpaces[column, row] = true;
            }
        }
    }
    private void CreateTheBoard()
    {
        //Create extra tiles
        CreateBlankSpaces();

        //Create the board
        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                if (!blankSpaces[x, y])
                {
                    var pos = new Vector2(x, y);

                    //Create BG Tiles
                    var tile = Instantiate(bgTilePrefab, pos, Quaternion.identity);
                    tile.transform.SetParent(parentTile.transform);
                    tile.name = bgTilePrefab.name;

                    //Create Moving Pieces
                    var selectedIndex = Random.Range(0, piecePrefabs.Length);
                    var selectedPrefab = piecePrefabs[selectedIndex];

                    //Check is there any matches vertically or horizontally, true means there is a match
                    int recreateCount = 0;
                    while((CheckHorizontalMatches(x, y, selectedPrefab) || CheckVerticalMatches(x, y, selectedPrefab)) && recreateCount < 100)
                    {
                        selectedIndex = Random.Range(0, piecePrefabs.Length);
                        selectedPrefab = piecePrefabs[selectedIndex];
                        recreateCount++;
                    }

                    var piece = Instantiate(selectedPrefab, pos, Quaternion.identity);
                    piece.transform.SetParent(parentObjects.transform);
                    piece.name = $"{x}, {y}";

                    piece.GetComponent<Piece>().column = x;
                    piece.GetComponent<Piece>().row = y;

                    itemList[x, y] = piece;
                }
            }
        }
    }

    private bool CheckHorizontalMatches(int x, int y, GameObject selectedPrefab)
    {
        //Tag variables
        string leftTag, leftLeftTag;
        leftTag = leftLeftTag = string.Empty;

        //Get the tags
        if(x - 1 >= 0 && itemList[x - 1, y] != null)
            leftTag = itemList[x - 1, y].tag;

        if(x - 2 >= 0 && itemList[x - 2, y] != null)
            leftLeftTag = itemList[x - 2, y].tag;

        //Compare the tags
        if(leftTag == selectedPrefab.tag && leftLeftTag == selectedPrefab.tag)
            return true;

        return false;
    }
    private bool CheckVerticalMatches(int x, int y, GameObject selectedPrefab)
    {
        //Tag variables
        string downDownTag, downTag;
        downDownTag = downTag = string.Empty;

        //Get the tags
        if(y - 1 >= 0 && itemList[x, y - 1] != null)
            downTag = itemList[x, y - 1].tag;

        if(y - 2 >= 0 && itemList[x, y - 2] != null)
            downDownTag = itemList[x, y - 2].tag;

        //Compare the tags
        if(downDownTag == selectedPrefab.tag && downTag == selectedPrefab.tag)
            return true;

        return false;
    }
    public void MatchCheck()
    {
        //Clear the list before adding new matches
        matchedPieces.Clear();

        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                if(!blankSpaces[x, y])
                {
                    GameObject currentPiece = itemList[x, y];

                    //we should control x - 1 and x + 1 is inside the board at the same time for horizontal
                    if(x > 0 && x < boardWidth - 1)
                    {
                        //Get left and right pieces of currentPiece
                        GameObject leftPiece = itemList[x - 1, y];
                        GameObject rightPiece = itemList[x + 1, y];

                        if(leftPiece != null && rightPiece != null)
                        {
                            //Check are they matches
                            if(leftPiece.tag == currentPiece.tag && rightPiece.tag == currentPiece.tag)
                            {
                                AddToTheMatches(currentPiece);
                                AddToTheMatches(leftPiece);
                                AddToTheMatches(rightPiece);

                                ThereIsAMatch = true;
                            }
                        }
                    }
                    //we should control y - 1 and y + 1 is inside the board at the same time for vertical
                    if(y > 0 && y < boardHeight - 1)
                    {
                        //Get up and down pieces of currentPiece
                        GameObject downPiece = itemList[x, y - 1];
                        GameObject upPiece = itemList[x, y + 1];

                        if(downPiece != null && upPiece != null)
                        {
                            //Check are they matches
                            if(downPiece.tag == currentPiece.tag && upPiece.tag == currentPiece.tag)
                            {
                                AddToTheMatches(currentPiece);
                                AddToTheMatches(downPiece);
                                AddToTheMatches(upPiece);

                                ThereIsAMatch = true;
                            }
                        }
                    }
                }
            }
        }
        //DestroyMatches();
    }
    private void AddToTheMatches(GameObject obj)
    {
        if(!matchedPieces.Contains(obj)) matchedPieces.Add(obj);
    }
    public bool ThereIsAMatch {get; set;}
    public bool IsThereAMatch()
    {
        if(matchedPieces.Count > 0)
            return true;

        return false;
    }
    public void DestroyMatches()
    {
        //Remove matched pieces
        foreach (var piece in matchedPieces)
        {
            var p = piece.GetComponent<Piece>();
            int column = p.column;
            int row = p.row;
            itemList[column, row] = null;
            fillingSpaces[column, row] = true;

            Destroy(piece);
        }
        //Clear the list contains matched pieces
        matchedPieces.Clear();

        //Fill The Board with new pieces
        //StartCoroutine(nameof(FillingTheBoard));
    }
    IEnumerator FillingTheBoard()
    {
        yield return new WaitForSeconds(0.2f);
        ThereIsAMatch = false;
        FillTheBoard();
    }
    // private void FillTheBoardAdvanced()
    // {
    //     for (int x = 0; x < boardWidth; x++)
    //     {
    //         for (int y = 0; y < boardHeight; y++)
    //         {
    //             if(!blankSpaces[x, y] && )
    //         }
    //     }
    // }
    private void FillTheBoard()
    {
        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                if(fillingSpaces[x, y])
                {
                    CreateNewPiece(x, y);
                    fillingSpaces[x, y] = false;
                }
            }
        }
        // //If there is any matches check and destroy them
        // MatchCheck();
    }
    private void CreateNewPiece(int x, int y)
    {
        //Create position to spawn
        var pos = new Vector2(x, y);

        //Create moving pieces
        var selectedIndex = Random.Range(0, piecePrefabs.Length);
        var selectedPrefab = piecePrefabs[selectedIndex];

        var piece = Instantiate(selectedPrefab, pos, Quaternion.identity);
        piece.transform.SetParent(parentObjects.transform);
        piece.name = $"{x}, {y}";

        piece.GetComponent<Piece>().column = x;
        piece.GetComponent<Piece>().row = y;

        itemList[x, y] = piece;
    }





}