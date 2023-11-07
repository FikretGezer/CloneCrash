using System.Collections;
using UnityEngine;
public enum MoveState{
    Move,
    Stop
}
public class ItemController : MonoBehaviour
{
    [SerializeField] private LayerMask pieceMask;
    [SerializeField] private float lerpSpeed = 1f;
    private Camera cam;
    private Vector2 startPos, endPos;
    private bool isThereAMatch;
    private bool isSwapStarted;

    private Piece selectedPiece;
    private MoveState moveState;
    private void Awake() {
        cam = Camera.main;
        moveState = MoveState.Move;
    }
    private void Update() {
        var pos = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(pos.origin, Vector2.zero, Mathf.Infinity, pieceMask);
        if(moveState == MoveState.Move)
        {
            if(Input.GetMouseButtonDown(0))
            {
                if(hit.collider != null)
                {
                    startPos = pos.origin;
                    selectedPiece = hit.collider.GetComponent<Piece>();
                }
            }
            if(Input.GetMouseButtonUp(0))
            {
                if(selectedPiece != null)
                {
                    endPos = pos.origin;
                    var dir = endPos - startPos;
                    if(dir.magnitude > 0.1f)
                    {
                        checkCount = 0;
                        isSwapStarted = true;
                        isThereAMatch = false;

                        var actualDir = CheckDirection(dir.normalized);
                        SwapObjects(actualDir);
                    }
                }
            }
        }
    }
    private int column, row, targetColumn, targetRow;
    private void SwapObjects(string direction)
    {
        column = selectedPiece.column;
        row = selectedPiece.row;

        //isThereAMatch = true;
        if(direction == "Right")
        {
            if(column + 1 < ItemSpawnManager.Instance.boardWidth && ItemSpawnManager.Instance.pieceList[column + 1, row] != null)
            {
                //Debug.LogWarning(ItemSpawnManager.Instance.pieceList[selectedPiece.column + 1, selectedPiece.row]);
                moveState = MoveState.Stop;
                Swapping(column, row, column + 1, row);
                //Get targets to swap back pieces, if there is no match
                targetColumn = column + 1;
                targetRow = row;
            }
        }
        else if(direction == "Up")
        {
            if(row + 1 < ItemSpawnManager.Instance.boardHeight && ItemSpawnManager.Instance.pieceList[column, row + 1] != null)
            {
                moveState = MoveState.Stop;
                Swapping(column, row, column, row + 1);
                //Get targets to swap back pieces, if there is no match
                targetColumn = column;
                targetRow = row + 1;
            }
        }
        else if(direction == "Left")
        {
            if(column - 1 >= 0 && ItemSpawnManager.Instance.pieceList[column - 1, row] != null){
                moveState = MoveState.Stop;
                Swapping(column, row, column - 1, row);
                //Get targets to swap back pieces, if there is no match
                targetColumn = column - 1;
                targetRow = row;
            }
        }
        else if(direction == "Down")
        {
            if(row - 1 >= 0 && ItemSpawnManager.Instance.pieceList[column, row - 1] != null){
                moveState = MoveState.Stop;
                Swapping(column, row, column, row - 1);
                //Get targets to swap back pieces, if there is no match
                targetColumn = column;
                targetRow = row - 1;
            }
        }
    }
    private void Swapping(int currentColumn, int currentRow, int targetColumn, int targetRow)
    {
        //Change positions in pieceList
        GameObject targetObj = ItemSpawnManager.Instance.pieceList[targetColumn, targetRow];
        ItemSpawnManager.Instance.pieceList[targetColumn, targetRow] = selectedPiece.gameObject;
        ItemSpawnManager.Instance.pieceList[currentColumn, currentRow] = targetObj;

        //Change column and rows
        targetObj.GetComponent<Piece>().column = currentColumn;
        targetObj.GetComponent<Piece>().row = currentRow;

        selectedPiece.column = targetColumn;
        selectedPiece.row = targetRow;

        //Set positions for actual swap
        var tempPos = targetObj.transform.position;
        var currentPos = selectedPiece.transform.position;

        //Swap
        StartCoroutine(ActualSwapping(selectedPiece.transform, targetObj.transform, currentPos, tempPos));
    }
    IEnumerator ActualSwapping(Transform currentObj, Transform targetObj, Vector2 currentPos, Vector2 targetPos)
    {
        float current = 0f, target = 1f;
        //Start swapping
        while(current < target)
        {
            current = Mathf.MoveTowards(current, target, Time.deltaTime * lerpSpeed);
            currentObj.position = Vector2.Lerp(currentObj.position, targetPos, current);
            targetObj.position = Vector2.Lerp(targetObj.position, currentPos, current);

            yield return null;
        }

        //Check is there a match
        if(isSwapStarted)
        {
            isThereAMatch = FindMatches.Instance.MatchFinding();
            isSwapStarted = false;
            if(!isThereAMatch)//No match
            {
                //Swap back pieces
                Swapping(targetColumn, targetRow, column, row);
                checkCount++;
                //isThereAMatch = false;
            }
            else //There is a match
            {
                //Complete the swapping
                FindMatches.Instance.DestroyMatches();
                selectedPiece = null;
                moveState = MoveState.Move;
            }
        }
        //This means; there was not match when pieces swapped so pieces swapped back to their originals
        if(checkCount > 0)
        {
            selectedPiece = null;
            moveState = MoveState.Move;
        }

    }
    private int checkCount = 0;
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