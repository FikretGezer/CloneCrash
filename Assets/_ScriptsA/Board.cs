using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public float lerpSpeed = 1f;
    public int width;
    public int height;
    public GameObject tilePrefab;
    public GameObject[] dotPrefabs;

    public GameObject[,] allDots;
    private BackgroundTile[,] allTiles;
    

    private void Start()
    {
        allTiles = new BackgroundTile[width, height];
        allDots = new GameObject[width, height];
        Setup();
    }
    private void Setup()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var tempPos = new Vector2(x, y);
                GameObject tile = Instantiate(tilePrefab, tempPos, Quaternion.identity);
                tile.transform.parent = transform;
                tile.name = $"( {x}, {y} )";

                GameObject selectedPrefab = dotPrefabs[Random.Range(0, dotPrefabs.Length)];
                var dot = Instantiate(selectedPrefab, tempPos, Quaternion.identity);
                dot.transform.parent = transform;
                dot.name = tile.name;
                allDots[x, y] = dot;
            }
        }
    }
}
