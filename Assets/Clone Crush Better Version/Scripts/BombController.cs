using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BombType{
    NoBomb,
    RowBomb,
    ColumnBomb,
    ColorBomb
}
public class BombController : MonoBehaviour
{
    [Header("Bomb Prefabs")]
    [SerializeField] private GameObject colorBombPrefab;
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
                Debug.Log("Row bomb is made.");
                bombType = BombType.RowBomb;
            }
            else
            {
                //Make Column Bomb
                Debug.Log("Column bomb is made.");
                bombType = BombType.ColumnBomb;
            }
        }
        else if(matchedCount == 5 || matchedCount == 10)
        {
            //Make Color Bomb
            Debug.Log("Color bomb is made.");
            bombType = BombType.ColorBomb;
        }
        else
        {
            Debug.Log("No bomb is made.");
            bombType = BombType.NoBomb;
        }
        MakeTheActualBomb(column, row, bombType);
    }
    private void MakeTheActualBomb(int column, int row, BombType bombType)
    {
        if(bombType != BombType.NoBomb)
        {
            GameObject bomb = null;
            if(bombType == BombType.RowBomb)
                bomb = Instantiate(rowBombPrefab, new Vector2((int)column, (int)row), Quaternion.identity);
            else if(bombType == BombType.ColumnBomb)
                bomb = Instantiate(columnBombPrefab, new Vector2((int)column, (int)row), Quaternion.identity);
            else
                bomb = Instantiate(colorBombPrefab, new Vector2((int)column, (int)row), Quaternion.identity);


            bomb.SetActive(false);
            bombs.Add(bomb);
            ItemSpawnManager.Instance.pieceList[column, row] = bomb;
            bomb.name = $"{column}, {row}";
            bomb.GetComponent<Piece>().column = column;
            bomb.GetComponent<Piece>().row = row;
        }
    }
    public void ActivateBombs()
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
    public void IsRowBomb(int column)
    {
        for (int y = 0; y < ItemSpawnManager.Instance.boardHeight; y++)
        {
            if(ItemSpawnManager.Instance.pieceList[column, y] != null)
            {
                FindMatches.Instance.AddToTheList(ItemSpawnManager.Instance.pieceList[column, y]);
            }
        }
    }
    public void IsColumnBomb(int row)
    {
        for (int x = 0; x < ItemSpawnManager.Instance.boardHeight; x++)
        {
            if(ItemSpawnManager.Instance.pieceList[x, row] != null)
            {
                FindMatches.Instance.AddToTheList(ItemSpawnManager.Instance.pieceList[x, row]);
            }
        }
    }
}
