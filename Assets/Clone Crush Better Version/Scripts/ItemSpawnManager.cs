using System;
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
    private bool[,] blackSpaces;

    public static ItemSpawnManager Instance;
    private void Awake()
    {
        if (Instance == null) Instance = this;

        itemList = new GameObject[boardWidth, boardHeight];
        blackSpaces = new bool[boardWidth, boardHeight];

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
                blackSpaces[column, row] = true;
            }
        }
    }
    private void CreateTheBoard()
    {
        //Create extra tiles
        CreateBlankSpaces();

        //Create parent for the bgTile
        GameObject parentTile = new GameObject();
        parentTile.name = "Board Tiles";

        //Create parent for the objects
        GameObject parentObjects = new GameObject();
        parentObjects.name = "Board Pieces";

        //Create the board
        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                if (!blackSpaces[x, y])
                {
                    var pos = new Vector2(x, y);

                    //Create BG Tiles
                    var tile = Instantiate(bgTilePrefab, pos, Quaternion.identity);
                    tile.transform.SetParent(parentTile.transform);
                    tile.name = bgTilePrefab.name;

                    //Create Moving Pieces
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
        }
    }
}
