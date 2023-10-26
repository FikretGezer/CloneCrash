using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GeneralButtonFunctions : MonoBehaviour
{
    public void LoadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void SetLevel(GameObject obj)
    {
        PlayerPrefs.SetInt("Selected Level", obj.GetComponent<LevelButton>().level - 1);
    }    
}
