using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectManager : MonoBehaviour
{
    public List<GameObject> levelPages;
    private int currentPage = 0;
    private void Start()
    {
        currentPage = GameData.Instance.saveData.currentPageForLevels;

        AddPagesToTheList();
        if (levelPages.Count > 0)
            LoadPage();
    }
    private void AddPagesToTheList()
    {
        foreach (Transform page in transform)
        {
            levelPages.Add(page.gameObject);
        }
    }
    private void LoadPage()    
    {        
        for (int i = 0; i < levelPages.Count; i++)
        {
            if(currentPage == i)
            {
                levelPages[i].SetActive(true);
            }
            else
                levelPages[i].SetActive(false);
        }
    }
    public void NextPage()
    {
        if(currentPage + 1 < levelPages.Count)
        {
            currentPage++;
            LoadPage();
        }
    }
    public void PreviousPage()
    {
        if (currentPage - 1 >= 0)
        {
            currentPage--;
            LoadPage();
        }
    }
}
