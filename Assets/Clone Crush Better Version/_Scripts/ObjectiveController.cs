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
    [SerializeField] private TMP_Text t_MoveCount;
    [SerializeField] private Transform inGameMenuContainer;
    [SerializeField] private Transform endingMenus;
    //UNSERIALIZE LATER
    [SerializeField] private List<CurrentObjectives> objectives;

    public int moveAmount;
    public static ObjectiveController Instance;
    private void Awake() {
        if(Instance == null) Instance = this;
    }
    private void Start() {
        var currentLevel = PlayerPrefs.GetInt("Current Level");
        moveAmount = levels.allLevels[currentLevel].moveAmount;
        t_MoveCount.text = moveAmount.ToString();

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
    public void ReduceMoveCount()
    {
        if(moveAmount - 1 >= 0)
        {
            moveAmount--;
            t_MoveCount.text = moveAmount.ToString();
            if(moveAmount <= 0)
            {
                ActivateEndingMenus("lose");
                Debug.Log("<color=green>FAILED!!!</color>");
            }
        }
    }
    private void ActivateEndingMenus(string result)
    {
        foreach(Transform menu in inGameMenuContainer)
        {
            if(menu != endingMenus)
                menu.gameObject.SetActive(false);
        }
        endingMenus.gameObject.SetActive(true);
        if(result == "lose")
        {
            endingMenus.GetChild(0).gameObject.SetActive(false);
            endingMenus.GetChild(1).gameObject.SetActive(true);
        }
        else if(result == "win")
        {
            endingMenus.GetChild(1).gameObject.SetActive(false);
            endingMenus.GetChild(0).gameObject.SetActive(true);
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
                            ActivateEndingMenus("win");
                            Debug.Log("<color=red>WIN!!!</color>");
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
