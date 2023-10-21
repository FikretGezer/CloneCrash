using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockFiller : MonoBehaviour
{
    [SerializeField] private Transform blocksParent;

    [Header("X values")]
    [SerializeField] private float minX;
    [SerializeField] private float maxX;

    [Header("X values")]
    [SerializeField] private float minY;
    [SerializeField] private float maxY;

    private List<Vector3> positions = new List<Vector3>();  
    private void GetPositions()//Identifies which positions to spawn items
    {
        foreach (Transform block in blocksParent)
        {
            if(block.gameObject.activeInHierarchy)
            {
                positions.Add(block.position);
            }
        }
    }
    private void InstantiateOnBlocks()
    {
        float totalCount = 0;
        int horizontalCount = (int)((maxX - minX) / 1.3f) + 1;
        int verticalCount = (int)((maxY - minY) / 1.3f) + 1;

        float xPos = minX, yPos = minY;

        
    }
}
