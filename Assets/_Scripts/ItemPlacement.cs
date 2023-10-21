using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemPlacement : MonoBehaviour
{
    [SerializeField] private Transform blocksParent;
    [SerializeField] private Sprite[] items;
    [SerializeField] private GameObject blockPrefab;

    [Header("X values")]
    [SerializeField] private float minX;
    [SerializeField] private float maxX;

    [Header("X values")]
    [SerializeField] private float minY;
    [SerializeField] private float maxY;

    public int[,] placementAvailablity;
    public GameObject[,] placedObjects;
    public List<string> emptyIndices = new List<string>();

    public static ItemPlacement _instance;
    private void Awake()
    {
        if (_instance == null) _instance = this;
    }

    private void Start()
    {
        BlockPositions();
        CheckPlacements();
    }
    private void CheckPlacements()
    {
        float totalCount = 0;
        int horizontalCount = (int)((maxX - minX) / 1.3f) + 1;
        int verticalCount = (int)((maxY - minY) / 1.3f) + 1;

        float xPos = minX, yPos = minY;
        //placementAvailablity = new int[horizontalCount, verticalCount];
        placedObjects = new GameObject[horizontalCount, verticalCount];


        for (int y = 0; y < verticalCount; y++)
        {
            for (int x = 0; x < horizontalCount; x++)
            {
                totalCount++;
                bool isEmpty = IsEmpty(new Vector3(xPos, yPos, 0));
                if (isEmpty)
                {
                    emptyIndices.Add($"{x},{y}");
                    //placementAvailablity[x, y] = 2;//2 means cant replaceable
                                                   //1 means can replaceable but it's full rn
                                                   //0 means can replaceable
                }
                else
                {
                    var item = Instantiate(blockPrefab, new Vector3(xPos, yPos), Quaternion.identity);
                    //placementAvailablity[x, y] = 1;
                    placedObjects[x, y] = item;
                    item.name = $"{x},{y}";
                    item.GetComponent<SpriteRenderer>().sprite = items[Random.Range(0, items.Length)];
                }
                xPos += 1.3f;
            }
            yPos += 1.3f;
            xPos = minX;
        }     
    }
    private bool IsEmpty(Vector3 checkedPos)
    {
        if(positions.Count > 0)
        { 
            foreach (var blockPos in positions)
            {
                if (blockPos == checkedPos)
                    return false;
            }
        }
        return true;
    }
    List<Vector3> positions = new List<Vector3>();
    private void BlockPositions()
    {        
        foreach (Transform block in blocksParent)
        {
            if(block.gameObject.activeInHierarchy)
                positions.Add(block.position);
        }
    }
}
