using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Level", menuName = "Level Creation/Level", order = 1)]
public class Level : ScriptableObject
{
    [Header("Board Dimensions")]
    public int width;
    public int height;

    [Header("Starting Tiles")]
    public TileType[] boardLayout;

    [Header("Available Dots")]
    public GameObject[] dotPrefabs;

    [Header("Score Goals")]
    public int scoreGoal;
    public Goal[] goals;
}


[System.Serializable]
public class Goal
{
    public Sprite goalSprite;
    public int goalCount;
    public string tag;
}
