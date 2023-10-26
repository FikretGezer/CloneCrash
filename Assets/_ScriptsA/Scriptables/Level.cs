using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
