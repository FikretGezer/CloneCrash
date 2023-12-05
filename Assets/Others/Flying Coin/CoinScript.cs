using System;
using System.Collections.Generic;
using UnityEngine;

public class CoinScript : MonoBehaviour
{
    public Vector2 targetPos;
    private float current = 0f, target = 1f;
    private float lerpSpeed = 1f;
    private bool isItFirst;
    private Animator imageAnim;
    private void Start() {
        lerpSpeed = FlyingCoinsScript.Instance.lerpSpeed;
        targetPos = FlyingCoinsScript.Instance.flyingPosition;
        imageAnim = FlyingCoinsScript.Instance.imageAnim;
    }
    private void OnEnable() {
        current = 0f;
        isItFirst = false;
    }
    void Update()
    {
        current = Mathf.MoveTowards(current, target, lerpSpeed * Time.deltaTime);
        transform.position = Vector2.Lerp(transform.position, targetPos, current);
        if(current == target)
        {
            gameObject.SetActive(false);
        }
        if(current >= target * 0.2f && !isItFirst)
        {
            isItFirst = true;
            FlyingCoinsScript.Instance.IncreaseCount();
            PlayAnim();
        }

    }
    private void PlayAnim()
    {
        if(imageAnim != null)
        {
            imageAnim.SetTrigger("resizeCoin");
        }
    }
}
