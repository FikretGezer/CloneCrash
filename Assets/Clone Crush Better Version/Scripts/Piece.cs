using System;
using System.Collections;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public int column;
    public int row;
    public float lerpSpeed = 2f;
    public bool IsMoving { get; set; }
    public float current = 1f, target = 1f;
    private void Update() {
        if(IsMoving)
        {
            if(current != target)
            {
                var targetPos = new Vector2((int)column, (int)row);

                current = Mathf.MoveTowards(current, target, Time.deltaTime * lerpSpeed);
                transform.position = Vector2.Lerp(transform.position, targetPos, current);
            }
            else
            {
                IsMoving = false;
                transform.position = new Vector2((int)column, (int)row);
            }
        }
        // if(IsMoving)
        // {
        //     //IsMoving = false;
        //     //StartCoroutine(MoveObject());
        // }
        // else{
        //     current = 0f;
        //     IsMoving = false;
        // }
    }
    public void MoveObjectCor()
    {
        StartCoroutine(MoveObject());
        if(IsMoving)
        {
            IsMoving = false;
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
}
