using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintGiver : MonoBehaviour
{
    //Find matches when switching pieces
    private void FindAllMatches()
    {
        int height = ItemSpawnManager.Instance.boardHeight;
        int width = ItemSpawnManager.Instance.boardWidth;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(ItemSpawnManager.Instance.pieceList[x, y] != null)
                {
                    if(x < width - 1)
                    {
                        //Swap and check horizontal

                    }
                    if(y < height - 1)
                    {
                        //Swap and check vertical
                    }
                }
            }
        }
    }

}
