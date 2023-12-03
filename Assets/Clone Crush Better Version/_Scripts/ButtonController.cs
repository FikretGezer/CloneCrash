using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour
{
    [SerializeField] private Transform pageContainer;
    [SerializeField] private bool isAtLevelSelectScreen;
    private List<GameObject> pageList;
    private int currentPage = 0;
    private void Start() {
        if(isAtLevelSelectScreen)
        {
            currentPage = GameSaver.Instance.dataSaver.currentPageForLevels;

            pageList = Pages();
            for(int i = 0; i < pageList.Count; i++)
            {
                if(i == currentPage)
                    pageList[i].SetActive(true);
                else
                    pageList[i].SetActive(false);
            }
        }
    }
    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }
    public void PreviousPage()
    {
        if(currentPage - 1 >= 0)
        {
            pageList[currentPage].SetActive(false);
            pageList[currentPage - 1].SetActive(true);
            currentPage--;
        }
    }
    public void NextPage()
    {
        if(currentPage + 1 < pageList.Count)
        {
            pageList[currentPage].SetActive(false);
            pageList[currentPage + 1].SetActive(true);
            currentPage++;
        }
    }
    List<GameObject> Pages()
    {
        var pageList = new List<GameObject>();
        foreach(Transform page in pageContainer)
        {
            pageList.Add(page.gameObject);
        }
        return pageList;
    }
}
