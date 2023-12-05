using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameButtonFunctions : MonoBehaviour
{
    private Board board;
    private void Awake()
    {
        board = FindObjectOfType<Board>();
    }
    public void SaveAndGetBackToMenu()
    {
        if(board != null)
        {
            //GameData.Instance.saveData.isActive[board.level + 1] = true;
            //GameData.Instance.Save();
            GameData.Instance.IncreaseLastLevel(board);
            SceneManager.LoadScene("Menu");
        }
    }
}
