using System.Collections.Generic;
using UnityEngine;

public class HintGiver : MonoBehaviour
{
    private bool showHint;
    [SerializeField] private float flashingSpeed = 1f;
    [SerializeField] private float hintShowingTime = 1f;
    private float currentHintTime;
    private List<List<GameObject>> hintMatches = new List<List<GameObject>>();
    private bool hintShowing;
    private bool canShowHint;
    private bool hintReseted;
    private float current = 0f, target = 1f;
    private List<GameObject> a;
    private List<GameObject> selectedList;
    public static HintGiver Instance;
    private void Awake() {
        if(Instance == null) Instance = this;
        hintReseted = true;
    }
    private void Update()
    {
        if(ItemController.Instance.moveState == MoveState.Move)
        {
            if(hintReseted)
                HintTimer();
            if(canShowHint)
            {
                canShowHint = false;
                ShowTheHint();
            }
            if(Input.GetKeyDown(KeyCode.S))
            {
                ResetHint();
            }
            PlayHintAnimation();
        }
        else
            ResetHint();
    }
    private void HintTimer()
    {
        currentHintTime += Time.deltaTime;
        if(currentHintTime >= hintShowingTime)
        {
            FindPossibleMatches();

            currentHintTime = 0f;
            hintShowing = true;
            canShowHint = true;
            hintReseted = false;
        }
    }
    public void ResetHint()
    {
        currentHintTime = 0f;
        //hintShowing = false;
        hintReseted = true;

        if(selectedList != null && selectedList.Count > 0)
        {
            foreach(var piece in selectedList)
            {
                if(piece == null)
                    break;
                piece.GetComponent<SpriteRenderer>().color = Color.white;
            }
            selectedList = null;
        }
    }
    private void ShowTheHint()
    {
        //Reset previous values
        current = 0f;
        target = 1f;

        if(selectedList != null && selectedList.Count > 0)
        {
            foreach(var piece in selectedList)
            {
                piece.GetComponent<SpriteRenderer>().color = Color.white;
            }
            selectedList = null;
        }
        //There are matches to give a hint
        if(hintMatches.Count > 0)
        {
            //Hint animation play check
            if(hintShowing)
            {
                //Selected random hint
                int rnd = Random.Range(0, hintMatches.Count);
                selectedList = hintMatches[rnd];
            }
        }
    }
    private void PlayHintAnimation()
    {
        if(hintShowing)
        {
            if(selectedList != null && selectedList.Count > 0)
            {
                current = Mathf.MoveTowards(current, target, flashingSpeed * Time.deltaTime);
                foreach(var piece in selectedList)
                {
                    if(piece == null)
                        break;
                    piece.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.white, new Color(0.5f, 0.5f, 0.5f, 1f), current);
                }

                if(current == target) target = target == 1f ? 0f : 1f;
            }
        }
    }

    //Find possible matches when switching pieces
    private void FindPossibleMatches()
    {
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
                        FindMatches.Instance.SwitchAndCheck(x, y, Vector2.right, true);
                    }
                    if(y < height - 1)
                    {
                        //Swap and check vertical
                        FindMatches.Instance.SwitchAndCheck(x, y, Vector2.up, true);
                    }
                }
            }
        }
    }
    public void AddPossibleMatches(List<GameObject> matches)
    {
        hintMatches.Add(matches);
    }
    // private List<GameObject> FindAllMatches()
    // {
    //     var possibleMatches = new List<GameObject>();

    //     int height = ItemSpawnManager.Instance.boardHeight;
    //     int width = ItemSpawnManager.Instance.boardWidth;
    //     for (int x = 0; x < width; x++)
    //     {
    //         for (int y = 0; y < height; y++)
    //         {
    //             if(ItemSpawnManager.Instance.pieceList[x, y] != null)
    //             {
    //                 if(x < width - 1)
    //                 {
    //                     //Swap and check horizontal
    //                     if(FindMatches.Instance.SwitchAndCheck(x, y, Vector2.right))
    //                     {
    //                         if(!possibleMatches.Contains(ItemSpawnManager.Instance.pieceList[x, y]))
    //                             possibleMatches.Add(ItemSpawnManager.Instance.pieceList[x, y]);
    //                     }
    //                 }
    //                 if(y < height - 1)
    //                 {
    //                     //Swap and check vertical
    //                     if(FindMatches.Instance.SwitchAndCheck(x, y, Vector2.right))
    //                     {
    //                         if(!possibleMatches.Contains(ItemSpawnManager.Instance.pieceList[x, y]))
    //                             possibleMatches.Add(ItemSpawnManager.Instance.pieceList[x, y]);
    //                     }
    //                 }
    //             }
    //         }
    //     }
    //     return possibleMatches;
    // }
}
