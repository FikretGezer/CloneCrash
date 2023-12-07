using System.Collections.Generic;
using UnityEngine;

public enum BombType{
    NoBomb,
    RowBomb,
    ColumnBomb,
    AdjacentBomb,
    ColorBomb
}
public class BombController : MonoBehaviour
{
    [Header("Bomb Prefabs")]
    [SerializeField] private GameObject colorBombPrefab;
    [SerializeField] private GameObject adjacentBombPrefab;
    [SerializeField] private GameObject columnBombPrefab;
    [SerializeField] private GameObject rowBombPrefab;
    private List<GameObject> bombs = new List<GameObject>();
    public static BombController Instance;
    private void Awake()
    {
        if(Instance == null) Instance = this;
    }

    public void CanBombBeCreated(int column, int row, string swapDirection)
    {
        var pieceList = ItemSpawnManager.Instance.pieceList;
        int matchedCount = 0;
        BombType bombType = BombType.NoBomb;

        if(IsItAdjacent(column, row))
        {
            //Make Row Bomb
            // Debug.Log("Adjacent bomb is made.");
            bombType = BombType.AdjacentBomb;
            MakeTheActualBomb(column, row, bombType);
            return;
        }

        if(swapDirection == "Horizontal")
        {
            //Check rows or columns according to giving direction, for this check row
            for (int y = 0; y < ItemSpawnManager.Instance.boardHeight; y++)
            {
                if(pieceList[column, y] != null)
                {
                    //If object is matched consecutive increase the count of matched objects, else break the loop
                    if(pieceList[column, y].GetComponent<Piece>().isMatched)
                    {
                        matchedCount++;
                    }
                    else if(matchedCount >= 3 && pieceList[column, y].GetComponent<Piece>().isMatched)
                    {
                        break;
                    }
                }
            }
        }
        else if(swapDirection == "Vertical")
        {
            //Check rows or columns according to giving direction, for this check row
            for (int x = 0; x < ItemSpawnManager.Instance.boardWidth; x++)
            {
                if(pieceList[x, row] != null)
                {
                    //If object is matched consecutive increase the count of matched objects, else break the loop
                    if(pieceList[x, row].GetComponent<Piece>().isMatched)
                    {
                        matchedCount++;
                    }
                    else if(matchedCount >= 3 && pieceList[x, row].GetComponent<Piece>().isMatched)
                    {
                        break;
                    }
                }
            }
        }



        if(matchedCount == 4 || matchedCount == 8)
        {
            if(swapDirection == "Horizontal")
            {
                //Make Row Bomb
                // Debug.Log("Row bomb is made.");
                bombType = BombType.RowBomb;
            }
            else
            {
                //Make Column Bomb
                // Debug.Log("Column bomb is made.");
                bombType = BombType.ColumnBomb;
            }
        }
        else if(matchedCount == 5 || matchedCount == 10)
        {
            //Make Color Bomb
            // Debug.Log("Color bomb is made.");
            bombType = BombType.ColorBomb;
        }
        else
        {
            // Debug.Log("No bomb is made.");
            bombType = BombType.NoBomb;
        }
        MakeTheActualBomb(column, row, bombType);
    }
    private bool IsItAdjacent(int column, int row)
    {
        // var pieceList = ItemSpawnManager.Instance.pieceList;

        // if(column - 2 >= 0 && column + 2 < ItemSpawnManager.Instance.boardWidth
        // && row - 2 >= 0 && row + 2 < ItemSpawnManager.Instance.boardHeight)
        // {
        //     //LEFT
        //     var currentObj = pieceList[column, row];
        //     var leftObj = pieceList[column - 1, row];
        //     var leftLeftObj = pieceList[column - 2, row];
        //     //RIGHT
        //     var rightObj = pieceList[column + 2, row];
        //     var rightRightObj = pieceList[column + 2, row];
        //     //UP
        //     var upObj = pieceList[column, row + 1];
        //     var upUpObj = pieceList[column, row + 2];
        //     //DOWN
        //     var downObj = pieceList[column, row - 1];
        //     var downDownObj = pieceList[column, row - 2];

        //     if((leftObj != null && leftLeftObj != null && currentObj.tag == leftObj.tag && currentObj.tag == leftLeftObj.tag)
        //     || (rightObj != null && rightRightObj != null && currentObj.tag == rightObj.tag && currentObj.tag == rightRightObj.tag))//Horizontal match
        //     {
        //         if(currentObj != null)
        //         {
        //             if((upObj != null && upUpObj != null && currentObj.tag == upObj.tag && currentObj.tag == upUpObj.tag)
        //             || (downObj != null && downDownObj != null && currentObj.tag == downObj.tag && currentObj.tag == downDownObj.tag))//Vertical Match
        //             {
        //                 return true;
        //             }
        //             else
        //             {
        //                 return false;
        //             }
        //         }
        //         else
        //             return false;
        //     }
        //     else
        //     {
        //         return false;
        //     }
        // }
        return false;
    }
    private void MakeTheActualBomb(int column, int row, BombType bombType)
    {
        if(bombType != BombType.NoBomb)
        {
            GameObject bomb = null;
            if(bombType == BombType.RowBomb)
                bomb = Instantiate(rowBombPrefab, new Vector2((int)column, (int)row), Quaternion.identity);
            else if(bombType == BombType.ColumnBomb)
                bomb = Instantiate(columnBombPrefab, new Vector2((int)column, (int)row), Quaternion.Euler(new Vector3(0,0,90f)));
            else if(bombType == BombType.AdjacentBomb)
                bomb = Instantiate(adjacentBombPrefab, new Vector2((int)column, (int)row), Quaternion.identity);
            else
                bomb = Instantiate(colorBombPrefab, new Vector2((int)column, (int)row), Quaternion.identity);

            bomb.SetActive(false);
            bombs.Add(bomb);
            ItemSpawnManager.Instance.pieceList[column, row] = bomb;
            // bomb.name = $"{column}, {row}";
            bomb.GetComponent<Piece>().column = column;
            bomb.GetComponent<Piece>().row = row;
        }
    }
    public void SpawnBombs()
    {
        if(bombs.Count > 0)
        {
            foreach (var bomb in bombs)
            {
                bomb.SetActive(true);
            }
        }
        bombs.Clear();
    }
    public void ActivateRowBomb(int column)
    {
        for (int y = 0; y < ItemSpawnManager.Instance.boardHeight; y++)
        {
            if(ItemSpawnManager.Instance.pieceList[column, y] != null)
            {
                FindMatches.Instance.AddToTheList(ItemSpawnManager.Instance.pieceList[column, y]);
            }
        }
        //FindMatches.Instance.DestroyMatches();
    }
    public void ActivateColumnBomb(int row)
    {
        for (int x = 0; x < ItemSpawnManager.Instance.boardWidth; x++)
        {
            if(ItemSpawnManager.Instance.pieceList[x, row] != null)
            {
                FindMatches.Instance.AddToTheList(ItemSpawnManager.Instance.pieceList[x, row]);
            }
        }
    }
    public void ActivateColorBomb(int targetColumn, int targetRow)
    {
        var pieceList = ItemSpawnManager.Instance.pieceList;
        //Get tag to find similar objects
        string tag = pieceList[targetColumn, targetRow].tag;
        //Find similar objects
        for (int x = 0; x < ItemSpawnManager.Instance.boardWidth; x++)
        {
            for (int y = 0; y < ItemSpawnManager.Instance.boardHeight; y++)
            {
                //Add similat objects to the list
                if(pieceList[x, y] != null && pieceList[x, y].tag == tag)
                {
                    FindMatches.Instance.AddToTheList(pieceList[x, y]);
                }
            }
        }
    }
    public void ActivateAdjacentBomb(int column, int row)
    {
        var startColumn = column - 1;
        var startRow = row - 1;
        var pieceList = ItemSpawnManager.Instance.pieceList;

        for (int x = startColumn; x < startColumn + 3; x++)
        {
            for (int y = startRow; y < startRow + 3; y++)
            {
                if(x >= 0 && x < ItemSpawnManager.Instance.boardWidth && y >= 0 && y < ItemSpawnManager.Instance.boardHeight)
                {
                    if(pieceList[x, y] != null)
                    {
                        FindMatches.Instance.AddToTheList(pieceList[x, y]);
                    }
                }
            }
        }
    }
}
