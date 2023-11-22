using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Board/New Level")]
public class BoardLayoutScriptable : ScriptableObject
{
    [Header("Board Size")]
    public int boardWidth;
    public int boardHeight;

    [Header("Object Prefabs")]
    public GameObject bgTilePrefab;
    public GameObject breakableTilePrefab;
    public GameObject iceTilePrefab;
    public GameObject[] piecePrefabs;

    [Header("Board Layout")]
    public ExtraPiece[] specialTiles;

}
