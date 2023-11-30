using UnityEngine;

[CreateAssetMenu(fileName = "Level Manager", menuName = "Board/Level Manager")]
public class WorldScriptable : ScriptableObject
{
    public BoardLayoutScriptable[] allLevels;
}
