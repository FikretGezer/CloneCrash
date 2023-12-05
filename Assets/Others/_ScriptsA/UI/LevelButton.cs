using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButton : MonoBehaviour
{
    public TMP_Text levelText;
    public int level;
    public bool isActive;
    public Sprite[] cover;

    private Image buttonImage;
    private Button btn;
    
    private void Start()
    {       
        buttonImage = GetComponent<Image>();
        btn = GetComponent<Button>();
        LoadData();
        CheckIsActive();
    }
    public void CheckIsActive()
    {
        if(isActive)
        {
            levelText.gameObject.SetActive(true);
            levelText.text = $"{level:00}";
            buttonImage.sprite = cover[1];
            btn.enabled = true;
        }
        else
        {
            levelText.gameObject.SetActive(false);
            buttonImage.sprite = cover[0];
            btn.enabled = false;
        }
    }
    void LoadData()
    {
        if (GameData.Instance.saveData.isActive[level - 1])
            isActive = true;
        else
            isActive = false;
    }    
}
