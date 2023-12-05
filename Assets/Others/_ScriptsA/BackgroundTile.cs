using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTile : MonoBehaviour
{
    public int hitPoints;

    private Board board;
    private SpriteRenderer spriteRnd;
    private void Awake()
    {
        board = FindObjectOfType<Board>();
        spriteRnd = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        if(hitPoints <= 0 && board != null)
        {            
            Destroy(this.gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        hitPoints -= damage;
        MakeLighter();
    }
    private void MakeLighter()
    {
        Color clr = spriteRnd.color;
        float newAlpha = clr.a * 0.5f;
        spriteRnd.color = new Color(clr.r, clr.g, clr.b, newAlpha);  
    }
}
