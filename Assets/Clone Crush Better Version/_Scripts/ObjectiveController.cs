using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum ObjectiveTag
{
    Dot_Cat,
    Dot_Lizard,
    Dot_Octopus,
    Dot_Owl,
    Dot_Pig,
    Dot_Rabbit,
    Breakable,
    Ice
}
[System.Serializable]
public class Objective
{
    public Sprite objectiveSprite;
    public ObjectiveTag tagg;
    public int objectiveAmount;
}
[System.Serializable]
public class CurrentObjectives
{
    public GameObject objectiveHolder;
    public Sprite objectiveSprite;
    public string tag;
    public int amount;
    public TMP_Text amountHolder;
}
public class ObjectiveController : MonoBehaviour
{
    [SerializeField] private WorldScriptable levels;
    [SerializeField] private Transform objectiveParent;
    [SerializeField] private GameObject objectivePrefab;
    //UNSERIALIZE LATER
    [SerializeField] private List<CurrentObjectives> objectives;
    public static ObjectiveController Instance;
    private void Awake() {
        if(Instance == null) Instance = this;
    }
    private void Start() {
        var currentLevel = PlayerPrefs.GetInt("Current Level");
        if(levels != null && levels.allLevels.Length > 0)
        {
            for (int i = 0; i < levels.allLevels[currentLevel].objectives.Length; i++)
            {
                //Gets current Objective from the current level
                CurrentObjectives curObjective  = new CurrentObjectives();
                curObjective.objectiveSprite = levels.allLevels[currentLevel].objectives[i].objectiveSprite;
                curObjective.tag = levels.allLevels[currentLevel].objectives[i].tagg.ToString().Replace("_"," ");
                curObjective.amount = levels.allLevels[currentLevel].objectives[i].objectiveAmount;

                //Instantiate objective holder UIs
                var obj = Instantiate(objectivePrefab);
                obj.transform.SetParent(objectiveParent);

                obj.transform.GetChild(0).GetComponent<Image>().sprite = curObjective.objectiveSprite;
                obj.transform.GetChild(1).GetComponent<TMP_Text>().text = curObjective.tag;
                obj.transform.GetChild(2).GetComponent<TMP_Text>().text = curObjective.amount.ToString();

                curObjective.amountHolder = obj.transform.GetChild(2).GetComponent<TMP_Text>();
                curObjective.objectiveHolder = obj;
                //Add the objective to the list
                objectives.Add(curObjective);
            }
        }
    }
    public void ReduceObjectiveAmount(GameObject destroyedObj)
    {
        var tag = destroyedObj.tag;
        if(objectives.Count > 0)
        {
            foreach(var objective in objectives)
            {
                if(objective.tag == tag)
                {
                    if(objective.amount > 0)
                    {
                        objective.amount--;
                        if(objective.amount <= 0)
                        {
                            objective.amount = 0;
                            objective.objectiveHolder.SetActive(false);
                            //Play animation for removed objective
                        }
                        objective.amountHolder.text = objective.amount.ToString();

                        if(IsGameDone())
                        {
                            Debug.Log("<color=red>GAME IS OVER</color>");
                            GameSaver.Instance.IncreaseLevel(PlayerPrefs.GetInt("Current Level"));
                            SceneManager.LoadScene("Menu");
                        }
                    }
                }
            }

        }
    }
    private bool IsGameDone()
    {
        if(objectives.Count > 0)
        {
            foreach (var objective in objectives)
            {
                if(objective.amount > 0)
                    return false;
            }
        }
        return true;
    }
}
