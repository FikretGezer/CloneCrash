using UnityEngine;

public enum States {
    Move,
    Stop
}
public class ItemController : MonoBehaviour
{
    [SerializeField] private LayerMask pieceMask;

    //Variable for direction
    private Vector2 startPos;
    private Vector2 endPos;

    private bool itemSelected;
    private int column, row;
    ItemSpawnManager itemSpawnManager;
    private void Awake()
    {
        itemSpawnManager = FindObjectOfType<ItemSpawnManager>();
    }
    private void Update()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, Vector2.zero, Mathf.Infinity, pieceMask);

        if(Input.GetMouseButtonDown(0))
        {
            if (hit.collider != null)
            {
                Piece p = hit.collider.GetComponent<Piece>();
                if (p != null)
                {
                    itemSelected = true;
                    startPos = hit.point;
                    column = p.column;
                    row = p.row;
                }
            }
        }
        if(Input.GetMouseButtonUp(0) && itemSpawnManager != null)
        {
            if(itemSelected) //This means there is an item at startPosition, column and row rearrange using that item
            {
                itemSelected = false;
                endPos = ray.origin;
                Vector2 dir = endPos - startPos;

                //Checking does player swipe towards any direction
                if(dir.magnitude > 0.02f)
                {
                    string direction = string.Empty;
                    direction = CheckDirection(dir.normalized);

                    //First if maybe useless, i can remove it
                    if (itemSpawnManager.itemList[column, row] != null)//
                    {
                        if(direction == "Right")
                        {
                            //check if movement exceed the board
                            if(column + 1 < itemSpawnManager.boardWidth)
                            {
                                if (itemSpawnManager.itemList[column + 1, row] != null)
                                {
                                    MovePiecesSec(column, row, column + 1, row);
                                }
                            }
                        }
                        if(direction == "Up")
                        {
                            //check if movement exceed the board
                            if(row + 1 < itemSpawnManager.boardHeight)
                            {
                                if (itemSpawnManager.itemList[column, row + 1] != null)
                                {
                                    /*Debug.Log(direction);

                                    //Get spawing objects
                                    GameObject startObj = itemSpawnManager.itemList[column, row];
                                    GameObject endObj = itemSpawnManager.itemList[column, row + 1];

                                    //Swap their postions
                                    var tempPos = endObj.transform.position;
                                    endObj.transform.position = startObj.transform.position;
                                    startObj.transform.position = tempPos;

                                    //Swap their positions in array
                                    itemSpawnManager.itemList[column, row] = endObj;
                                    itemSpawnManager.itemList[column, row + 1] = startObj;

                                    //Swap their inside columns and rows
                                    Piece start = startObj.GetComponent<Piece>();
                                    Piece end = endObj.GetComponent<Piece>();

                                    start.row = row + 1;
                                    end.row = row;

                                    //Debug.Log(itemSpawnManager.itemList[column + 1, row].gameObject.name);*/
                                    MovePiecesSec(column, row, column, row + 1);
                                }
                            }
                        }
                        if(direction == "Left")
                        {
                            //check if movement exceed the board
                            if(column - 1 >= 0)
                            {
                                if (itemSpawnManager.itemList[column - 1, row] != null)
                                {
                                    MovePiecesSec(column, row, column - 1, row);
                                }
                            }
                        }
                        if(direction == "Down")
                        {
                            //check if movement exceed the board
                            if(row - 1 >= 0)
                            {
                                if (itemSpawnManager.itemList[column, row - 1] != null)
                                {
                                    MovePiecesSec(column, row, column, row - 1);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    private void MovePiecesSec(int currentColumn, int currentRow, int otherColumn, int otherRow)
    {
            //Get spawing objects
            GameObject startObj = itemSpawnManager.itemList[currentColumn, currentRow];
            GameObject endObj = itemSpawnManager.itemList[otherColumn, otherRow];

            //Swap their postions
            var tempPos = endObj.transform.position;
            endObj.transform.position = startObj.transform.position;
            startObj.transform.position = tempPos;

            //Swap their positions in array
            itemSpawnManager.itemList[currentColumn, currentRow] = endObj;
            itemSpawnManager.itemList[otherColumn, otherRow] = startObj;

            //Swap their inside columns and rows
            Piece start = startObj.GetComponent<Piece>();
            Piece end = endObj.GetComponent<Piece>();

            start.column = otherColumn;
            start.row = otherRow;

            end.column = currentColumn;
            end.row = currentRow;
    }
    private void MovePieces(int currentColumn, int currentRow)
    {
        if (itemSpawnManager.itemList[currentColumn, currentRow + 1] != null)
        {
            //Get spawing objects
            GameObject startObj = itemSpawnManager.itemList[currentColumn, currentRow];
            GameObject endObj = itemSpawnManager.itemList[currentColumn, currentRow + 1];

            //Swap their postions
            var tempPos = endObj.transform.position;
            endObj.transform.position = startObj.transform.position;
            startObj.transform.position = tempPos;

            //Swap their positions in array
            itemSpawnManager.itemList[currentColumn, currentRow] = endObj;
            itemSpawnManager.itemList[currentColumn, currentRow + 1] = startObj;

            //Swap their inside columns and rows
            Piece start = startObj.GetComponent<Piece>();
            Piece end = endObj.GetComponent<Piece>();

            start.row = currentRow + 1;
            end.row = currentRow;
        }
    }
    private string CheckDirection(Vector2 dir)
    {
        float angleDeg = DirToAngle(dir);
        if ((angleDeg < 45f && angleDeg >= 0f) || (angleDeg >= 315f && angleDeg < 360f))
        {
            return "Right";
        }
        else if (angleDeg >= 45f && angleDeg < 135f)
        {
            return "Up";
        }
        else if (angleDeg >= 135f && angleDeg < 215f)
        {
            return "Left";
        }
        else
            return "Down";
        // else if (angleDeg >= 215f && angleDeg < 315f)
        // {
        //     return "Down";
        // }
    }
    private float DirToAngle(Vector2 dir)
    {
        float angleRad = Mathf.Atan2(dir.y, dir.x);
        float angleDeg = Mathf.Rad2Deg * angleRad;

        if(angleDeg < 0)
        {
            angleDeg += 360f;
        }
        return angleDeg;
    }
}
