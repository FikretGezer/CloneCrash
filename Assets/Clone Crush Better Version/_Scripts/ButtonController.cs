using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour
{
    [Header("Menu and Level Selection")]
    [SerializeField] private Transform pageContainer;
    [SerializeField] private bool isAtLevelSelectScreen;
    private List<GameObject> pageList;
    private int currentPage = 0;

    [Header("In Game")]
    [SerializeField] private GameObject inGameUI;
    [SerializeField] private GameObject inGamePause;
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
    #region Menu and Level Selection Buttons
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
    #endregion
    #region In Game Button
    public void StopTheGame()
    {
        //1-> We can stop the time
        if(Time.timeScale == 0f)
        {
            Time.timeScale = 1f;
            ItemController.Instance.enabled = true;
            inGamePause.SetActive(false);
            inGameUI.SetActive(true);
        }
        else
        {
            Time.timeScale = 0f;
            ItemController.Instance.enabled = false;
            inGameUI.SetActive(false);
            inGamePause.SetActive(true);
        }
        //2-> we can disable scripts
    }
    #endregion
}
