using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelO : MonoBehaviour
{
    [SerializeField] private Sprite lockSprite;
    [SerializeField] private Sprite unlockedSprite;
    string loadingLvl;
    private void Awake()
    {
        loadingLvl = gameObject.name.Split("_")[1];
    }
    private void Start() {
        Debug.Log("Length: " + GameSaver.Instance.dataSaver.isActive.Length + "<color=red>INSIDE OF THE LEVEL</color>");
        if(GameSaver.Instance.dataSaver.isActive[int.Parse(loadingLvl) - 1])
        {
            GetComponent<Image>().sprite = unlockedSprite;
            GetComponent<Button>().enabled = true;
            transform.GetChild(0).GetComponent<TMP_Text>().enabled = true;
        }
        else
        {
            GetComponent<Image>().sprite = lockSprite;
            GetComponent<Button>().enabled = false;
            transform.GetChild(0).GetComponent<TMP_Text>().enabled = false;
        }

    }
    private void OnEnable() {
        transform.GetChild(0).GetComponent<TMP_Text>().text = loadingLvl.ToString();
    }
    public void LoadLevel()
    {
        Debug.Log($"Loading Level {loadingLvl}");
        PlayerPrefs.SetInt("Current Level", int.Parse(loadingLvl) - 1);
        ButtonController.Instance.LoadLevel("GameScene");
        // SceneManager.LoadScene("GameScene");
    }
}
