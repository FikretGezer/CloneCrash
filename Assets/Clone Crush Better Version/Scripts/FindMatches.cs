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
    public void DestroyMatches()
    {
        foreach(var item in matches)
        {
            Destroy(item);
        }
    }
}
