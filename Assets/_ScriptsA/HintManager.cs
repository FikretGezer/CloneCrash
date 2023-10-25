using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintManager : MonoBehaviour
{
    public float hintDelay;
    public GameObject hintParticle;
    public GameObject currentHint;

    private float hintDelaySeconds;

    private Board board;
    private void Awake()
    {
        board = FindObjectOfType<Board>();
        hintDelaySeconds = hintDelay;
    }
    private void Update()
    {
        hintDelaySeconds -= Time.deltaTime;
        if(hintDelaySeconds <= 0 && currentHint == null)
        {
            MarkHint();
            hintDelaySeconds = hintDelay;
        }
    }

    //find all possible matches
    private List<GameObject> FindAllMatches()
    {
        var possibleMoves = new List<GameObject>();
        for (int x = 0; x < board.width; x++)
        {
            for (int y = 0; y < board.height; y++)
            {
                if (board.allDots[x, y] != null)
                {
                    if (x < board.width - 1)
                    {
                        if (board.SwitchAndCheck(x, y, Vector2.right))
                        {
                            possibleMoves.Add(board.allDots[x, y]);
                        }
                    }
                    if (y < board.height - 1)
                    {
                        if (board.SwitchAndCheck(x, y, Vector2.up))
                        {
                            possibleMoves.Add(board.allDots[x, y]);                            
                        }
                    }
                }
            }
        }
        return possibleMoves;
    }
    //pick one of those matches randomly
    private GameObject PickOneRandomly()
    {
        var possibleMoves = new List<GameObject>();
        possibleMoves = FindAllMatches();
        if(possibleMoves.Count > 0)
        {
            int pieceToUse = Random.Range(0, possibleMoves.Count);
            return possibleMoves[pieceToUse];
        }
        return null;
    }
    //create the hint
    private void MarkHint()
    {
        GameObject move = PickOneRandomly();
        if(move != null)
        {
            currentHint = Instantiate(hintParticle, move.transform.position, Quaternion.identity);
        }
    }
    //destroy the hint
    public void DestroyHint()
    {
        hintDelaySeconds = hintDelay;
        if(currentHint != null)
        {
            Destroy(currentHint);
            currentHint = null;
        }
    }
}
