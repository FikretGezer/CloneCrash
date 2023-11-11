using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindMatches : MonoBehaviour
{
    public List<GameObject> matches;
    public static FindMatches Instance;
    private void Awake() {
        if(Instance == null) Instance = this;
    }
    public bool MatchFinding()
    {
        var pieceList = ItemSpawnManager.Instance.pieceList;
        bool isThereAMatch = false;
        for (int x = 0; x < ItemSpawnManager.Instance.boardWidth; x++)
        {
            for (int y = 0; y < ItemSpawnManager.Instance.boardHeight; y++)
            {
                if(x - 2 >= 0)
                {
                    if(pieceList[x, y] != null && pieceList[x - 1, y] != null && pieceList[x - 2, y] != null)
                    {
                        var currentObj = pieceList[x, y];
                        var leftObj = pieceList[x - 1, y];
                        var leftLeftObj = pieceList[x - 2, y];

                        if(currentObj.CompareTag(leftObj.tag) && currentObj.CompareTag(leftLeftObj.tag))
                        {
                            Debug.Log("Horizontal Match");
                            AddToTheList(currentObj);
                            AddToTheList(leftObj);
                            AddToTheList(leftLeftObj);

                            isThereAMatch = true;
                            //return true;
                        }
                    }
                }
                if(y - 2 >= 0)
                {
                    if(pieceList[x, y] != null && pieceList[x, y - 1] != null && pieceList[x, y - 2] != null)
                    {
                        var currentObj = pieceList[x, y];
                        var downObj = pieceList[x, y - 1];
                        var downDownObj = pieceList[x, y - 2];

                        if(currentObj.CompareTag(downObj.tag) && currentObj.CompareTag(downDownObj.tag))
                        {
                            Debug.Log("Vertical Match");
                            AddToTheList(currentObj);
                            AddToTheList(downObj);
                            AddToTheList(downDownObj);

                            isThereAMatch = true;
                            //return true;
                        }
                    }
                }
            }
        }
        if(matches.Count > 0)
        {
            Debug.Log("It works");
            MatchCounter();
        }

        if(isThereAMatch) return true;
        return false;
    }
    private void AddToTheList(GameObject obj)
    {
        if(!matches.Contains(obj))
        {
            matches.Add(obj);
        }
    }
    private void MatchCounter()
    {
        int prevX = matches[0].GetComponent<Piece>().column;
        int prevY = matches[0].GetComponent<Piece>().row;

        int horCount = 1, verCount = 1;

        for (int i = 1; i < matches.Count; i++)
        {
            int currentX = matches[i].GetComponent<Piece>().column;
            int currentY = matches[i].GetComponent<Piece>().row;

            //Vertically Match
            if(currentX == prevX)
            {
                horCount++;
            }
            else
            {
                if(horCount > 2)
                {
                    Debug.Log("Ver Match Count: " + horCount);
                    horCount = 1;
                }
            }
            //Horizontally Match
            if(currentY == prevY)
            {
                verCount++;
            }
            else
            {
                if(verCount > 2)
                {
                    Debug.Log("Hor Match Count: " + verCount);
                    verCount = 1;
                }
            }
            prevX = currentX;
            prevY = currentY;
        }
    }
    public void DestroyMatches()
    {
        if(matches.Count > 0)
        {
            ItemController.Instance.moveState = MoveState.Stop;
            foreach(var item in matches)
            {
                Destroy(item);
            }
            matches.Clear();
            StartCoroutine(nameof(MoveTheBoardCo));
        }
    }
    private IEnumerator MoveTheBoardCo()
    {
        ItemController.Instance.moveState = MoveState.Stop;
        yield return new WaitForSeconds(0.4f);

        for (int x = 0; x < ItemSpawnManager.Instance.boardWidth; x++)
        {
            for (int y = 0; y < ItemSpawnManager.Instance.boardHeight; y++)
            {
                if(ItemSpawnManager.Instance.pieceList[x, y] == null)
                {
                    for (int i = y + 1; i < ItemSpawnManager.Instance.boardHeight; i++)
                    {
                        if(ItemSpawnManager.Instance.pieceList[x, i] != null)
                        {
                            //Get moving obj
                            var movingObj = ItemSpawnManager.Instance.pieceList[x, i];
                            //Move obj to empty position in array
                            ItemSpawnManager.Instance.pieceList[x, i] = null;//moving obj pos, 1br above (y + 1) from target pos
                            ItemSpawnManager.Instance.pieceList[x, y] = movingObj;//empty pos

                            //Change row
                            movingObj.GetComponent<Piece>().row = y;

                            //Set position
                            // movingObj.transform.position = new Vector2((int)x, (int)y);
                            //movingObj.GetComponent<Piece>().IsMoving = true;
                            //movingObj.GetComponent<Piece>().MoveObjectCor();
                            movingObj.GetComponent<Piece>().current = 0f;
                            movingObj.GetComponent<Piece>().IsMoving = true;
                            break;
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(0.4f);
        //Check is there is any matches
        MatchFinding();
        yield return new WaitForSeconds(0.1f);
        if(matches.Count > 0)
        {
            DestroyMatches();
        }
        else
        {
            ItemSpawnManager.Instance.FillTheBoard();
            yield return new WaitForSeconds(0.4f);
            ItemController.Instance.moveState = MoveState.Move;
        }
    }
    private void SolveDeadLock()
    {

    }
    private void SwitchPieces(int column, int row, Vector2 dir)
    {
        int newColumn = column + (int)dir.x;
        int newRow = row + (int)dir.y;
        var pieceList = ItemSpawnManager.Instance.pieceList;
        if(newColumn < ItemSpawnManager.Instance.boardWidth
        && newRow < ItemSpawnManager.Instance.boardHeight)
        {
            if(pieceList[newColumn, newRow] != null)
            {
                GameObject holder = pieceList[newColumn, newRow];
                pieceList[newColumn, newRow] = pieceList[column, row];
                pieceList[column, row] = holder;
            }
        }
    }
    public bool SwitchAndCheck(int column, int row, Vector2 dir)
    {
        SwitchPieces(column, row, dir);

        if(OnlyCheckMatches())
        {
            SwitchPieces(column, row, dir);
            return true;
        }
        SwitchPieces(column, row, dir);
        return false;
    }
    public bool OnlyCheckMatches()
    {
        var pieceList = ItemSpawnManager.Instance.pieceList;
        for (int x = 0; x < ItemSpawnManager.Instance.boardWidth; x++)
        {
            for (int y = 0; y < ItemSpawnManager.Instance.boardHeight; y++)
            {
                if(x - 2 >= 0)
                {
                    if(pieceList[x, y] != null && pieceList[x - 1, y] != null && pieceList[x - 2, y] != null)
                    {
                        var currentObj = pieceList[x, y];
                        var leftObj = pieceList[x - 1, y];
                        var leftLeftObj = pieceList[x - 2, y];

                        if(currentObj.CompareTag(leftObj.tag) && currentObj.CompareTag(leftLeftObj.tag))
                        {
                            HintGiver.Instance.AddPossibleMatches(new List<GameObject> {currentObj, leftObj, leftLeftObj});
                            return true;
                        }
                    }
                }
                if(y - 2 >= 0)
                {
                    if(pieceList[x, y] != null && pieceList[x, y - 1] != null && pieceList[x, y - 2] != null)
                    {
                        var currentObj = pieceList[x, y];
                        var downObj = pieceList[x, y - 1];
                        var downDownObj = pieceList[x, y - 2];

                        if(currentObj.CompareTag(downObj.tag) && currentObj.CompareTag(downDownObj.tag))
                        {
                            HintGiver.Instance.AddPossibleMatches(new List<GameObject> {currentObj, downObj, downDownObj});
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }
}
