using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour
{
    public AudioSource _audioSource {get; private set;}

    [Header("Menu and Level Selection")]
    [SerializeField] private Transform pageContainer;
    [SerializeField] private bool isAtLevelSelectScreen;
    private List<GameObject> pageList;
    private int currentPage = 0;

    [Header("In Game")]
    [SerializeField] private GameObject inGameUI;
    [SerializeField] private GameObject inGamePause;

    [Header("Image Containers")]
    [SerializeField] private Image musicImage;
    [SerializeField] private Image sfxImage;

    public static ButtonController Instance;
    private void Awake() {
        if(Instance == null) Instance = this;
        _audioSource = GetComponent<AudioSource>();

        if(musicImage != null)
            SoundController.Instance.SetMusicImage(musicImage, GameSaver.Instance.dataSaver.isMusicMuted);
        if(sfxImage != null)
            SoundController.Instance.SetSFXImage(sfxImage, GameSaver.Instance.dataSaver.isSFXMuted);

    }
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
        SoundController.Instance.PlaySFX(SFXs.buttonClick);
        SceneManager.LoadScene(levelName);
    }
    public void PreviousPage()
    {
        SoundController.Instance.PlaySFX(SFXs.buttonClick);
        if(currentPage - 1 >= 0)
        {
            pageList[currentPage].SetActive(false);
            pageList[currentPage - 1].SetActive(true);
            currentPage--;
        }
    }
    public void NextPage()
    {
        SoundController.Instance.PlaySFX(SFXs.buttonClick);
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
        SoundController.Instance.PlaySFX(SFXs.buttonClick);
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
    #region Sound Buttons
    public void MuteMusic(Image _image) => SoundController.Instance.MuteMusic(_image);
    public void MuteSFX(Image _image) => SoundController.Instance.MuteSFX(_image);

    #endregion
}
