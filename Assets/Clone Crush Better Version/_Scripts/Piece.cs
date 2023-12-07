using System.Collections;
using UnityEngine;

public class Piece : MonoBehaviour
{
    [SerializeField] private float lerpSpeed = 2f;

    public int column;
    public int row;
    public bool isShuffled;
    public bool isMatched;

    private bool isMoving;
    private float current = 1f;
    private float target = 1f;
    private void Update() {
        if(isMoving)
        {
            if(current != target)
            {
                var targetPos = new Vector2((int)column, (int)row);

                current = Mathf.MoveTowards(current, target, Time.deltaTime * lerpSpeed);
                transform.position = Vector2.Lerp(transform.position, targetPos, current);
            }
            else
            {
                isMoving = false;
                transform.position = new Vector2((int)column, (int)row);
            }
        }
    }
    public void ResetMovingValues()
    {
        current = 0f;
        isMoving = true;
    }
    public void MoveObjectCor()
    {
        StartCoroutine(MoveObject());
        if(isMoving)
        {
            isMoving = false;
        }
    }
    private IEnumerator MoveObject()
    {
        float current = 0f, target = 1f;
        var targetPos = new Vector2((int)column, (int)row);
        while(current < target)
        {
            current = Mathf.MoveTowards(current, target, Time.deltaTime * lerpSpeed);
            transform.position = Vector2.Lerp(transform.position, targetPos, current);
            yield return null;
        }
        transform.position = targetPos;
    }

    private void OnEnable() {
        isMatched = false;
    }
}
