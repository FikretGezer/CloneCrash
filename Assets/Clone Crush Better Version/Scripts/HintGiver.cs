using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintGiver : MonoBehaviour
{
    private bool showHint;
    public float flashingSpeed = 1f;
    private List<List<GameObject>> hintMatches = new List<List<GameObject>>();


    SpriteRenderer imageA, imageB, imageC;

    public bool hintShowing;
    public float current = 0f, target = 1f;
    private List<GameObject> a;
    private List<GameObject> selectedList;
    public static HintGiver Instance;
    private void Awake() {
        if(Instance == null) Instance = this;
        Debug.Log(hintMatches.Count);
    }
    private void Update() {
        if(!showHint)
        {
            showHint = true;
            a = new List<GameObject>(FindAllMatches());
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            if(hintMatches.Count > 0)
            {
                int rnd = Random.Range(0, hintMatches.Count);
                selectedList = hintMatches[rnd];

                hintShowing = hintShowing == false ? true : false;
                if(hintShowing)
                {

                }
                else
                {
                    current = 0f;
                    target = 1f;

                    if(selectedList.Count > 0)
                    {
                        foreach(var item in selectedList)
                        {
                            item.GetComponent<SpriteRenderer>().color = Color.white;
                        }
                        selectedList = null;
                    }
                }
            }
        }
    }
    //Find matches when switching pieces
    private List<GameObject> FindAllMatches()
    {
        var possibleMatches = new List<GameObject>();

        int height = ItemSpawnManager.Instance.boardHeight;
        int width = ItemSpawnManager.Instance.boardWidth;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(ItemSpawnManager.Instance.pieceList[x, y] != null)
                {
                    if(x < width - 1)
                    {
                        //Swap and check horizontal
                        if(FindMatches.Instance.SwitchAndCheck(x, y, Vector2.right))
                        {
                            if(!possibleMatches.Contains(ItemSpawnManager.Instance.pieceList[x, y]))
                                possibleMatches.Add(ItemSpawnManager.Instance.pieceList[x, y]);
                        }
                    }
                    if(y < height - 1)
                    {
                        //Swap and check vertical
                        if(FindMatches.Instance.SwitchAndCheck(x, y, Vector2.right))
                        {
                            if(!possibleMatches.Contains(ItemSpawnManager.Instance.pieceList[x, y]))
                                possibleMatches.Add(ItemSpawnManager.Instance.pieceList[x, y]);
                        }
                    }
                }
            }
        }
        return possibleMatches;
    }
    public void AddPossibleMatches(List<GameObject> matches)
    {
        hintMatches.Add(matches);
    }
}
