using UnityEngine;

public class Piece : MonoBehaviour
{
    [HideInInspector] public int column;
    [HideInInspector] public int row;

    private ItemSpawnManager ItemSpawnManager;
    private void Awake() {
        ItemSpawnManager = FindObjectOfType<ItemSpawnManager>();
    }
    private void Update() {
        if(row - 1 >= 0)
        {
            if(!ItemSpawnManager.blankSpaces[column, row - 1] && ItemSpawnManager.itemList[column, row - 1] == null)
            {
                transform.position = new Vector2(column, row - 1);
                ItemSpawnManager.itemList[column, row - 1] = gameObject;
                ItemSpawnManager.itemList[column, row] = null;
            }
        }
    }
}
