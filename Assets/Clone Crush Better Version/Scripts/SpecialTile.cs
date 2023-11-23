using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialTile : MonoBehaviour
{
    public int column;
    public int row;
    [SerializeField] private int health;
    public void TakeDamage()
    {
        health--;
        if(health <= 0)
            Destroy(this.gameObject);
    }
}
