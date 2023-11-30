using System.Collections.Generic;
using UnityEngine;

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
    // public Sprite objectiveImage;
    public ObjectiveTag tagg;
    public int objectiveAmount;
}
[System.Serializable]
public class CurrentObjectives
{
    public string currentTag;
    public int currentObjective;
}
public class ObjectiveController : MonoBehaviour
{
    [SerializeField] private WorldScriptable levels;
    //UNSERIALIZE LATER
    [SerializeField] private List<CurrentObjectives> objectives;
    [SerializeField] private int currentLevel;
    private void Awake() {
        if(levels != null && levels.allLevels.Length > 0)
        {
            CurrentObjectives objective  = new CurrentObjectives();
            for (int i = 0; i < levels.allLevels[currentLevel].objectives.Length; i++)
            {
                objective.currentTag = levels.allLevels[currentLevel].objectives[i].tagg.ToString().Replace("_"," ");
                objective.currentObjective = levels.allLevels[currentLevel].objectives[i].objectiveAmount;

                objectives.Add(objective);
            }
        }
    }
}
